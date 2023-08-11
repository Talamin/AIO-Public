using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class GroupFeralTank : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationBuff("Survival Instincts"), 1.5f, (s, t) => Me.HealthPercent <= 35, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Barkskin"), 1.7f, (s, t) => Me.HealthPercent <= 35, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Dire Bear Form"), 2f, (s, t) => true, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Bear Form"), 2.1f, (s, t) => !SpellManager.KnowSpell("Dire Bear Form"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Frenzied Regeneration"), 2.2f, (s, t) => Me.HealthPercent < 60, RotationCombatUtil.FindMe),

            // Berserk Mangle Spam
            new RotationStep(new RotationSpell("Mangle (Bear)"), 2.3f, (s, t) => Me.HaveBuff("Berserk"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Berserk"), 2.4f, (s,t) => RotationFramework.Enemies.Count(o => t.HasTarget && !o.IsTargetingMe && o.Position.DistanceTo(Me.Position) <= 8) >= 2, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Feral Charge - Bear"), 2.5f, (s,t) => t.GetDistance > 7 && t.HasTarget && !t.IsTargetingMe && RotationFramework.PartyMembers.Any(m => m.Position.DistanceTo(t.Position) < 7) && Settings.Current.GroupFeralCharge, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Bash"), 3f, (s, t) => t.IsCasting(), RotationCombatUtil.BotTargetFast),
            
            // Aggro section
            new RotationStep(new RotationSpell("Challenging Roar"), 4f, (s, t) => RotationFramework.Enemies.Count(o => o.HasTarget && !o.IsTargetingMe && o.Position.DistanceTo(Me.Position) <= 10) >= 3, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Growl"), 5f, (s, t) => t.HasTarget && !t.IsTargetingMe, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Swipe (Bear)"), 5.5f, (s, t) => RotationFramework.Enemies.Count(o => o.HasTarget && !o.IsTargetingMe && o.Position.DistanceTo(Me.Position) <= 8) >= 2, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Maul"), 6f, (s, t) => t.GetDistance < 8 && !RotationCombatUtil.IsCurrentSpell("Maul") && (Me.Rage > 30 || !t.IsTargetingMe), RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell("Faerie Fire (Feral)"), 7f, (s, t) => !t.HaveMyBuff("Faerie Fire (Feral)") && Settings.Current.GroupFeralFaerieFire, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Demoralizing Roar"), 7.5f, (s, t) => !t.HaveMyBuff("Demoralizing Roar"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Mangle (Bear)"), 8f, (s, t) => !t.HaveMyBuff("Mangle (Bear)"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Enrage"), 9f, (s, t) => t.GetDistance < 8 && t.HealthPercent >= 35, RotationCombatUtil.FindMe),
            
            // Rage dump
            new RotationStep(new RotationSpell("Swipe (Bear)"), 10f, (s, t) => RotationFramework.Enemies.Count(o => o.HasTarget && o.Position.DistanceTo(Me.Position) <= 8) >= 3, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Mangle (Bear)"), 11f, (s, t) => Me.Rage > 50, RotationCombatUtil.BotTargetFast),
        };
    }
}