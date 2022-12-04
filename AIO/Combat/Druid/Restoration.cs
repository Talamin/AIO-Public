using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class Restoration : BaseRotation
    {
        private static WoWUnit _tank;
        public Restoration() : base(useCombatSynthetics: Settings.Current.UseSyntheticCombatEvents) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            //Pre Calculations
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f, (action, me) => DoPreCalculations(),
                RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Tree of Life"), 1.1f, (s, t) => !Me.HaveBuff("Tree of Life"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Innervate"), 2f, (s, t) => Me.ManaPercentage <= 15, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Wild Growth"), 2.1f, (s,t) => RotationFramework.PartyMembers.Count(o => o.IsAlive && o.HealthPercent <= Settings.Current.RestorationWildGrowth && o.GetDistance <= 40) >= Settings.Current.RestorationWildGrowthCount, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Abolish Poison"), 5f, (s,t) => Settings.Current.RestorationRemovePoison && !t.HaveMyBuff("Abolish Poison") && t.HasDebuffType("Poison"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Remove Curse"), 6f, (s,t) => Settings.Current.RestorationRemoveCurse && t.HasDebuffType("Curse"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Swiftmend"), 6.1f, (s,t) => t.HaveMyBuff("Rejuvenation") || t.HaveMyBuff("Regrowth"),RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Nature's Swiftness"), 8f, (s, t) => RotationFramework.PartyMembers.Count(o => o.IsAlive && o.HealthPercent <= Settings.Current.RestorationHealingTouch && o.GetDistance <= 40) >= 1, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Healing Touch"), 9f, (s, t) => t.HealthPercent <= Settings.Current.RestorationHealingTouch, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Lifebloom", minimumStacks: 3, minimumRefreshTimeLeft: 2000), 10f, (s, t) => t.HealthPercent <= Settings.Current.RestorationLifebloom, RotationCombatUtil.FindTank),
            new RotationStep(new RotationSpell("Nourish"), 11f, (s, t) => t.HealthPercent <= Settings.Current.RestorationNourish, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Regrowth"), 12f, (s, t) => !t.HaveMyBuff("Regrowth") &&  t.HealthPercent <= Settings.Current.RestorationRegrowth, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Rejuvenation"), 13f, (s, t) => !t.HaveMyBuff("Rejuventation") && t.HealthPercent <= Settings.Current.RestorationRejuvenation, RotationCombatUtil.FindPartyMember),
        };

        private static WoWUnit FindTank(Func<WoWUnit, bool> predicate) =>
        _tank != null && predicate(_tank) ? _tank : null;

        private static WoWUnit FindExplicitPartyMemberByName(string name) =>
        RotationFramework.PartyMembers.FirstOrDefault(partyMember =>
        partyMember.Name.ToLower().Equals(name.ToLower()));

        private static bool DoPreCalculations()
        {
            _tank = FindExplicitPartyMemberByName(Settings.Current.RestoCustomTank) ??
                    RotationCombatUtil.FindTank(unit => true);
            return false;
        }
    }
}