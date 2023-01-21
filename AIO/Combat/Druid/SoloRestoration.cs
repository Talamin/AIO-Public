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
    internal class SoloRestoration : BaseRotation
    {
        private static WoWUnit _tank;
        public SoloRestoration() : base(useCombatSynthetics: Settings.Current.UseSyntheticCombatEvents) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            //Pre Calculations
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f, (action, me) => DoPreCalculations(),
                RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Tree of Life"), 1.1f, (s, t) => !Me.HaveBuff("Tree of Life"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Innervate"), 2f, (s, t) => Me.ManaPercentage <= 15, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Wild Growth"), 2.1f, (s,t) => RotationFramework.PartyMembers.Count(o => o.IsAlive && o.HealthPercent <= Settings.Current.SoloRestorationWildGrowth && o.GetDistance <= 40) >= Settings.Current.SoloRestorationWildGrowthCount, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Abolish Poison"), 5f, (s,t) => Settings.Current.SoloRestorationRemovePoison && !t.HaveMyBuff("Abolish Poison") && t.HasDebuffType("Poison"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Remove Curse"), 6f, (s,t) => Settings.Current.SoloRestorationRemoveCurse && t.HasDebuffType("Curse"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Swiftmend"), 6.1f, (s,t) => t.HealthPercent < 60 && (t.HaveMyBuff("Rejuvenation") || t.HaveMyBuff("Regrowth")),RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Nature's Swiftness"), 8f, (s, t) => RotationFramework.PartyMembers.Count(o => o.IsAlive && o.HealthPercent <= Settings.Current.SoloRestorationHealingTouch && o.GetDistance <= 40) >= 1, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Healing Touch"), 9f, (s, t) => t.HealthPercent <= Settings.Current.SoloRestorationHealingTouch, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Lifebloom", minimumStacks: 3, minimumRefreshTimeLeft: 2000), 10f, (s, t) => t.HealthPercent <= Settings.Current.SoloRestorationLifebloom, RotationCombatUtil.FindTank),
            new RotationStep(new RotationSpell("Nourish"), 11f, (s, t) => t.HealthPercent <= Settings.Current.SoloRestorationNourish, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Regrowth"), 12f, (s, t) => !t.HaveMyBuff("Regrowth") &&  t.HealthPercent <= Settings.Current.SoloRestorationRegrowth, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Rejuvenation"), 12.1f, (s, t) => !t.HaveMyBuff("Rejuventation") && t.HealthPercent <= Settings.Current.SoloRestorationRejuvenation, RotationCombatUtil.FindTank),
            new RotationStep(new RotationBuff("Rejuvenation"), 13f, (s, t) => !t.HaveMyBuff("Rejuventation") && t.HealthPercent <= Settings.Current.SoloRestorationRejuvenation, RotationCombatUtil.FindPartyMember),
        };

        //Find Custom Tank
        private static WoWUnit FindTank(Func<WoWUnit, bool> predicate) =>
        _tank != null && predicate(_tank) ? _tank : null;

        private static WoWUnit FindExplicitPartyMemberByName(string name) =>
        RotationFramework.PartyMembers.FirstOrDefault(partyMember =>
        partyMember.Name.ToLower().Equals(name.ToLower()));

        private static bool DoPreCalculations()
        {
            _tank = FindExplicitPartyMemberByName(Settings.Current.SoloRestoCustomTank) ??
                    RotationCombatUtil.FindTank(unit => true);
            return false;
        }
    }
}