using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    using Settings = WarlockLevelSettings;
    internal class SoloDestruction : BaseRotation
    {

        const int immolateTimeout = 600;

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Drain Soul"), 2.5f, (s,t) => t.HealthPercent <= 25 && ItemsHelper.GetItemCount("Soul Shard") <= 3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Life Tap"), 3.0f, (s,t) => Me.ManaPercentage < 20 && Me.HealthPercent > 25, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Life Tap"), 3.1f, (s,t) => Settings.Current.GlyphLifeTap && !Me.HaveBuff("Life Tap") && Me.HealthPercent > 25, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Curse of the Elements"), 3.2f, (s,t) => t.HealthPercent > 35 && !t.HaveBuff("Curse of the Elements") && t.IsElite && t.HealthPercent > 75, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadowfury"), 3.5f, (s,t) => RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10 && o.IsElite) >= 2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Rain of Fire"), 3.6f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloDestructionAOECount && Settings.Current.SoloDestructionUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Death Coil"), 3.8f, (s,t) => (Me.IsInGroup && BossList.MyTargetIsBoss && t.HealthPercent > 20 && !t.IsStunned) || (Me.HealthPercent < 35 && !t.IsStunned), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Drain Life"), 3.9f, (s,t) => !Me.IsInGroup && Me.HealthPercent < 75, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Corruption"), 4f, (s,t) => !t.HaveMyBuff("Corruption") && t.HealthPercent > 50 && t.IsElite, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Immolate"), 5f, (s,t) => t.HealthPercent > Settings.Current.UseWandTresh && !t.HaveMyBuff("Immolate"), RotationCombatUtil.BotTarget, forcedTimerMS: immolateTimeout),
            new RotationStep(new RotationSpell("Conflagrate"), 6f, (s,t) => (t.HaveBuff("Immolate") || t.HaveBuff("Shadowflame")) && t.HealthPercent > Settings.Current.UseWandTresh, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Chaos Bolt"), 7f, (s,t) => t.HealthPercent > Settings.Current.UseWandTresh, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Incinerate"), 8f, (s,t) => t.HealthPercent > Settings.Current.UseWandTresh, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 9f, (s,t) => t.HealthPercent > Settings.Current.UseWandTresh, RotationCombatUtil.BotTarget),
        };
    }
}
