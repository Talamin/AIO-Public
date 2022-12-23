using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Settings;
using System;
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
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Barkskin"), 1.5f, (s, t) => Me.HealthPercent <= 35, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Dire Bear Form"), 2f, (s, t) => Me.Level >= 40, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Bear Form"), 2.1f, (s, t) => Me.Level >= 40 && !SpellManager.KnowSpell("Dire Bear Form") , RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Bear Form"), 2.2f, (s, t) => Me.Level > 9 && Me.Level < 40, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Frenzied Regeneration"), 2.3f, (s, t) => Me.HealthPercent < 60 && Me.Rage > 25, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Berserk"), 2.4f, (s,t) => t.IsElite || RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <= 20) >=2, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Feral Charge - Bear"), 2.5f, (s,t) => t.GetDistance > 7 && Settings.Current.FeralCharge, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Bash"), 3f, (s, t) => t.IsCasting(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Demoralizing Roar"), 4f, (s, t) => !t.HaveMyBuff("Demoralizing Roar"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Growl"), 6f, (s, t) => RotationFramework.Enemies.Count(o => o.IsAttackable && !o.IsTargetingMe && o.IsTargetingPartyMember) >= 1 , RotationCombatUtil.FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell("Faerie Fire (Feral)"), 7f, (s, t) => !t.HaveMyBuff("Faerie Fire (Feral)") && Settings.Current.FFF, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Mangle (Bear)"), 8f, (s, t) => !t.HaveMyBuff("Mangle") && (Me.HaveBuff("Dire Bear Form") || Me.HaveBuff("Bear Form")), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Swipe (Bear)"), 9f, (s, t) => RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 8) >=3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Maul"), 10f, (s, t) => t.GetDistance < 8 && !RotationCombatUtil.IsCurrentSpell("Maul"), RotationCombatUtil.BotTarget),                        
            new RotationStep(new RotationSpell("Enrage"), 11f, (s, t) =>t.HealthPercent >= 35 && !Me.HaveBuff("Enrage"), RotationCombatUtil.FindMe),
        };       
    }
}