using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    using Settings = WarlockLevelSettings;
    internal class SoloDemonology : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Drain Soul"), 2.5f, (s,t) => t.HealthPercent <= 25 && ItemsHelper.GetItemCount("Soul Shard") <= 3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Metamorphosis"), 3f, (s,t) => !Me.HaveBuff("Metamorphosis") && Settings.Current.SoloDemonologyMetamorphosis =="OnCooldown", RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Metamorphosis"), 3.1f, (s,t) => !Me.HaveBuff("Metamorphosis") && Settings.Current.SoloDemonologyMetamorphosis =="OnBosses" && BossList.MyTargetIsBoss, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Demonic Empowerment"), 4f, (s,t) => !Pet.HaveBuff("Demonic Empowerment") && Pet.IsAlive && Pet.IsMyPet, RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell("Life Tap"), 5f, (s,t) => Me.HealthPercent > 50 && Me.ManaPercentage < Settings.Current.SoloDemonologyLifetap,RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Life Tap"), 5.1f, (s,t) => Settings.Current.GlyphLifeTap && !Me.HaveBuff("Life Tap") && Me.HealthPercent > 25, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Corruption"), 8f, (s,t) => Me.IsInGroup &&  !t.HaveMyBuff("Seed of Corruption") && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloDemonologyAOECount && Settings.Current.SoloDemonologyUseAOE, RotationCombatUtil.FindEnemy),
            new RotationStep(new RotationSpell("Rain of Fire"), 6f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloDemonologyAOECount && Settings.Current.SoloDemonologyUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Health Funnel"), 7f, (s,t) => !Pet.HaveBuff("Health Funnel") && Pet.HealthPercent < Settings.Current.SoloDemonologyHealthfunnelPet && Me.HealthPercent > Settings.Current.SoloDemonologyHealthfunnelMe && Pet.IsAlive && Pet.IsMyPet, RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell("Immolate"), 8f, (s,t) => !t.HaveMyBuff("Immolate") && !SpellManager.KnowSpell("Unstable Affliction"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Corruption"), 9f, (s,t) => !t.HaveMyBuff("Corruption"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of Agony"), 10f, (s,t) => !t.HaveMyBuff("Curse of Agony"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Drain Life"), 12f, (s,t) => !Me.IsInGroup && Me.HealthPercent < Settings.Current.SoloDemonologyDrainlife, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 16f ,(s,t) => t.HealthPercent > Settings.Current.UseWandTresh && !Settings.Current.SoloDemonologyShadowboltWand, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 17f ,(s,t) => Settings.Current.SoloDemonologyShadowboltWand, RotationCombatUtil.BotTarget)
        };
    }
}
