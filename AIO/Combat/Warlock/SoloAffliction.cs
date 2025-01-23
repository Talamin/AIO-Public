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
    internal class SoloAffliction : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Drain Soul"), 2.5f, (s,t) => t.HealthPercent <= 25 && ItemsHelper.GetItemCount("Soul Shard") <= 3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Demonic Empowerment"), 3f, (s,t) => !Pet.HaveBuff("Demonic Empowerment") && Pet.IsAlive && Pet.IsMyPet, RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell("Life Tap"), 4f, (s,t) => Me.HealthPercent > 50 && Me.ManaPercentage < Settings.Current.SoloAfflictionLifetap,RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Life Tap"), 4.1f, (s,t) => Settings.Current.GlyphLifeTap && !Me.HaveBuff("Life Tap") && Me.HealthPercent > 25, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Seed of Corruption"), 4.4f, (s,t) => 
                Settings.Current.SoloAfflictionUseSeedGroup &&  !t.HaveMyBuff("Seed of Corruption") && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloAfflictionAOECount && Settings.Current.SoloAfflictionUseAOE, RotationCombatUtil.FindEnemyAttackingGroupAndMe),
            new RotationStep(new RotationSpell("Corruption"), 8f, (s,t) => 
                Settings.Current.SoloAfflictionUseCorruptionGroup &&  !t.HaveMyBuff("Corruption") && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloAfflictionAOECount && Settings.Current.SoloAfflictionUseAOE, RotationCombatUtil.FindEnemyAttackingGroupAndMe),
            new RotationStep(new RotationSpell("Rain of Fire"), 4.5f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloAfflictionAOECount && Settings.Current.SoloAfflictionUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 5f, (s,t) => Me.HaveBuff("Shadow Trance"),RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Health Funnel"), 6f, (s,t) => !Pet.HaveBuff("Health Funnel") && Pet.HealthPercent < Settings.Current.SoloAfflictionHealthfunnelPet && Me.HealthPercent > Settings.Current.SoloAfflictionHealthfunnelMe && Pet.IsAlive && Pet.IsMyPet, RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell("Haunt"), 7.5f, (s,t) => !t.HaveMyBuff("Haunt"), RotationCombatUtil.BotTarget),
            //Curses
            new RotationStep(new RotationSpell("Curse of Agony"), 10f, (s,t) => !t.HaveMyBuff("Curse of Agony") && Settings.Current.SoloAfflictionAfflCurse == "Agony", RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of Doom"), 11f, (s,t) => !t.HaveMyBuff("Curse of Doom") && Settings.Current.SoloAfflictionAfflCurse == "Doom", RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of the Elements"), 12f, (s,t) => !t.HaveMyBuff("Curse of the Elements") && Settings.Current.SoloAfflictionAfflCurse == "Elements", RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of Tongues"), 13f, (s,t) => !t.HaveMyBuff("Curse of Tongues") && Settings.Current.SoloAfflictionAfflCurse == "Tongues", RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of Weakness"), 14f, (s,t) => !t.HaveMyBuff("Curse of Weakness") && Settings.Current.SoloAfflictionAfflCurse == "Weakness", RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of Exhaustion"), 15f, (s,t) => !t.HaveMyBuff("Curse of Exhaustion") && Settings.Current.SoloAfflictionAfflCurse == "Exhaustion", RotationCombatUtil.BotTarget),
            //
            new RotationStep(new RotationSpell("Immolate"), 16f, (s,t) => !t.HaveMyBuff("Immolate") && !SpellManager.KnowSpell("Unstable Affliction"), RotationCombatUtil.BotTarget, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Corruption"), 17f, (s,t) => !t.HaveMyBuff("Corruption"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Drain Life"), 18f, (s,t) => !Me.IsInGroup && Me.HealthPercent < Settings.Current.SoloDemonologyDrainlife, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Unstable Affliction"), 19f, (s,t) => !t.HaveMyBuff("Unstable Affliction"), RotationCombatUtil.BotTarget),
            //new RotationStep(new RotationSpell("Drain Soul"), 20f, (s,t) => t.HealthPercent <= 25, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 21f ,(s,t) => t.HealthPercent > Settings.Current.UseWandTresh && !Settings.Current.SoloAfflictionShadowboltWand, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 22f ,(s,t) => Settings.Current.SoloAfflictionShadowboltWand, RotationCombatUtil.BotTarget)
        };
    }
}
