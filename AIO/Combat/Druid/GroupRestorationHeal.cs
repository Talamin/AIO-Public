using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class GroupRestorationHeal : BaseRotation
    {
        private static WoWUnit _tank;
        private List<WoWPlayer> _hurtPartyMembers = new List<WoWPlayer>(0);
        private Stopwatch watch = Stopwatch.StartNew();
        public GroupRestorationHeal() : base(useCombatSynthetics: Settings.Current.UseSyntheticCombatEvents) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            //Pre Calculations
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f, (action, me) => DoPreCalculations(),
                RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Tree of Life"), 1.1f, (s, t) => !Me.CHaveBuff("Tree of Life"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Innervate"), 2f, (s, t) => Me.CManaPercentage() <= 15, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Wild Growth"), 2.1f, RotationCombatUtil.Always, FindWildgrowthCluster, checkLoS:true),
            new RotationStep(new RotationSpell("Abolish Poison"), 5f, (s,t) => Settings.Current.RestorationRemovePoison && !t.CHaveMyBuff("Abolish Poison") && t.HasDebuffType("Poison"), RotationCombatUtil.FindPartyMember, checkLoS:true),
            new RotationStep(new RotationSpell("Remove Curse"), 6f, (s,t) => Settings.Current.RestorationRemoveCurse && t.HasDebuffType("Curse"), RotationCombatUtil.FindPartyMember, checkLoS:true),
            new RotationStep(new RotationSpell("Swiftmend"), 6.1f, (s,t) => t.CHaveMyBuff("Rejuvenation") || t.CHaveMyBuff("Regrowth"),RotationCombatUtil.FindPartyMember, checkLoS:true),
            new RotationStep(new RotationSpell("Nature's Swiftness"), 8f, (s, t) => _hurtPartyMembers.Any(Member => Member.CHealthPercent() < Settings.Current.RestorationHealingTouch), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Healing Touch"), 8.9f, (s, t) => t.CHealthPercent() <= Settings.Current.RestorationHealingTouch, RotationCombatUtil.FindTank, checkLoS:true),
            new RotationStep(new RotationSpell("Healing Touch"), 9f, (s, t) => t.CHealthPercent() <= Settings.Current.RestorationHealingTouch, RotationCombatUtil.FindPartyMember, checkLoS:true),
            new RotationStep(new RotationBuff("Lifebloom", minimumStacks: 3, minimumRefreshTimeLeft: 2000), 10f, (s, t) => t.CHealthPercent() <= Settings.Current.RestorationLifebloom, RotationCombatUtil.FindTank, checkLoS:true),
            new RotationStep(new RotationSpell("Nourish"), 10.9f, (s, t) => t.CHealthPercent() <= Settings.Current.RestorationNourish, RotationCombatUtil.FindTank, checkLoS:true),
            new RotationStep(new RotationSpell("Nourish"), 11f, (s, t) => t.CHealthPercent() <= Settings.Current.RestorationNourish, RotationCombatUtil.FindPartyMember, checkLoS:true),
            new RotationStep(new RotationSpell("Regrowth"), 11.9f, (s, t) => !t.CHaveMyBuff("Regrowth") &&  t.CHealthPercent() <= Settings.Current.RestorationRegrowth, RotationCombatUtil.FindTank, checkLoS:true),
            new RotationStep(new RotationSpell("Regrowth"), 12f, (s, t) => !t.CHaveMyBuff("Regrowth") &&  t.CHealthPercent() <= Settings.Current.RestorationRegrowth, RotationCombatUtil.FindPartyMember, checkLoS:true),
            new RotationStep(new RotationBuff("Rejuvenation"), 12.1f, (s, t) => !t.CHaveMyBuff("Rejuventation") && t.CHealthPercent() <= Settings.Current.RestorationRejuvenation, RotationCombatUtil.FindTank, checkLoS:true),
            new RotationStep(new RotationBuff("Rejuvenation"), 13f, (s, t) => !t.CHaveMyBuff("Rejuventation") && t.CHealthPercent() <= Settings.Current.RestorationRejuvenation, RotationCombatUtil.FindPartyMember, checkLoS:true),
        };

        private bool DoPreCalculations()
        {
            if (LimitExecutionSpeed(100))
            {
                return true;
            }
            Cache.Reset();
            ClearLists();
            _tank = FindExplicitPartyMemberByName(Settings.Current.RestoCustomTank) ??
                    RotationCombatUtil.FindTank(unit => true);
            BuildLists();
            return false;
        }
        private bool LimitExecutionSpeed(int delay)
        {
            if (watch.ElapsedMilliseconds > delay)
            {
                watch.Restart();
                return false;
            }
            return true;
        }

        //build Lists
        private void BuildLists()
        { 
            for (int i = 0; i < RotationFramework.PartyMembers.Count(); i++)
            {
                WoWPlayer Partymember = RotationFramework.PartyMembers[i];
                if(Partymember.CHealthPercent()<99)
                {
                    _hurtPartyMembers.Add(Partymember);
                }
            }
        }

        //clear prebuilded Lists
        private void ClearLists()
        {
            _hurtPartyMembers.Clear();
        }

        //build custom Cluster for WildGrowth
        private WoWPlayer FindWildgrowthCluster(Func<WoWUnit, bool> predicate)
        {
            WoWPlayer largestCenter = null;
            int largestCount = Settings.Current.RestorationWildGrowth;
            for (var i = 0; i < _hurtPartyMembers.Count(); i++)
            {
                WoWPlayer originUnit = _hurtPartyMembers[i];
                if (!predicate(originUnit))
                {
                    continue;
                }
                Vector3 originPos = originUnit.CGetPosition();
                int localCount = _hurtPartyMembers.Count(unit => unit.CIsAlive() && unit.CGetPosition().DistanceTo(originPos) <= 15);

                if (localCount > largestCount)
                {
                    largestCenter = originUnit;
                    largestCount = localCount;
                }
            }
            return largestCenter;
        }

        //Find  Custom  Tank
        private static WoWUnit FindTank(Func<WoWUnit, bool> predicate)
        {
            return _tank != null && predicate(_tank) ? _tank : null;
        }

        private static WoWUnit FindExplicitPartyMemberByName(string name) => RotationFramework.PartyMembers.FirstOrDefault(partyMember => partyMember.Name.ToLower().Equals(name.ToLower()));

    }
}