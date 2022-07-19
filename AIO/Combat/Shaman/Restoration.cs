using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Shaman
{
    using Settings = ShamanLevelSettings;
    internal class Restoration : BaseRotation
    {
        private static WoWUnit _tank => RotationCombatUtil.FindTank(unit => true);
        public Restoration() : base(useCombatSynthetics: Settings.Current.UseSyntheticCombatEvents) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Nature's Swiftness"), 4f, RotationCombatUtil.Always, s => RotationFramework.PartyMembers.Count(o => o.IsAlive && o.HealthPercent <= 50 && o.GetDistance <= 40) >= 1, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Healing Wave"), 5f, (s,t) => t.HealthPercent <= 50, s => Me.HaveBuff("Nature's Swiftness"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Earth Shield"), 6f, RotationCombatUtil.Always, RotationCombatUtil.FindTank),
            new RotationStep(new RotationBuff("Tidal Force"), 7f, RotationCombatUtil.Always, s => RotationFramework.PartyMembers.Count(o => o.IsAlive && o.HealthPercent <= 80 && o.GetDistance <= 40) >= 2 || BossList.isboss, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Tidal Force"), 8f, RotationCombatUtil.Always, s => RotationFramework.AllUnits.Count(o => o.IsAlive && o.Target == _tank?.Guid && BossList.BossListInt.Contains(o.Entry) && o.GetDistance <= 40) >= 1, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Cleanse Spirit"), 9f, (s,t) => !Me.IsInGroup && t.HasDebuffType("Disease", "Poison", "Curse"), s => Me.ManaPercentage > 25, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Cleanse Spirit"), 9.1f, (s,t) => Me.IsInGroup && (t.HaveImportantCurse() || t.HaveImportantDisease() || t.HaveImportantPoison()), s => Me.ManaPercentage > 25, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Riptide"), 11f, (s,t) => t.HealthPercent <= Settings.Current.RestorationRiptideGroup, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Chain Heal"), 12f, RotationCombatUtil.Always, s => RotationFramework.PartyMembers.Count(o => o.IsAlive && o.HealthPercent <= Settings.Current.RestorationChainHealGroup && o.GetDistance <= 40) >= Settings.Current.RestorationChainHealCountGroup && _tank?.HealthPercent >= 50.0, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Lesser Healing Wave"), 13f, (s,t) => t.HealthPercent <= Settings.Current.RestorationLesserHealingWaveGroup, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Healing Wave"), 14f, (s,t) => t.HealthPercent <= Settings.Current.RestorationHealingWaveGroup, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Cure Toxins"), 14.1f, (s,t) => t.HaveImportantPoison() || t.HaveImportantDisease(), s => Me.ManaPercentage > 25, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Cure Toxins"), 15f, (s,t) => !Me.IsInGroup && t.HasDebuffType("Disease", "Poison"), s => Me.ManaPercentage > 25, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Earth Shock"), 19f, (s,t) => !Me.IsInGroup && !t.HaveMyBuff("Earth Shock"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Lightning Bolt"), 20f, (s,t) => !Me.IsInGroup, RotationCombatUtil.BotTarget),
        };

    }
}