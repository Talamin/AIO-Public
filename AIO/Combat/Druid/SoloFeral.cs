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
    internal class SoloFeral : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            // Rotation 11-20
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Innervate"), 1.1f, (s, t) => !Me.IsInGroup && !Me.HaveBuff("Bear Form") && !Me.HaveBuff("Dire Bear Form") && !Me.HaveBuff("Cat Form") && Me.ManaPercentage <= Settings.Current.Innervate, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Innervate"), 1.2f, (s, t) =>Me.IsInGroup && t.ManaPercentage <= Settings.Current.Innervate, RotationCombatUtil.FindHeal),
            new RotationStep(new RotationSpell("Remove Curse"), 1.3f, (s,t) => t.HaveBuff("Veil of Shadow") && Settings.Current.SoloFeralDecurse, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Barkskin"), 1.4f, (s, t) => Me.HealthPercent <= 35, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Regrowth"), 1.50f, (s, t) => Settings.Current.SoloFeralRegrowthIC && Me.ManaPercentage > 15 && Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && TalentsManager.HaveTalent(2,25) && (Me.Mana > ((DruidBehavior.TransformValue * 0.6) + DruidBehavior.RegrowthValue)) && !Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Regrowth"), 1.51f, (s, t) => Settings.Current.SoloFeralRegrowthIC && Me.ManaPercentage > 15 && Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && Me.Mana > (DruidBehavior.TransformValue + DruidBehavior.RegrowthValue) && !Me.HaveBuff("Regrowth") , RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Rejuvenation"), 1.60f, (s, t) => Settings.Current.SoloFeralRejuvenationIC && Me.ManaPercentage > 15 && Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && TalentsManager.HaveTalent(2,25) && Me.Mana > ((DruidBehavior.TransformValue * 0.6) + DruidBehavior.RejuvenationValue) && Me.HaveBuff("Regrowth") && Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Rejuvenation"), 1.61f, (s, t) => Settings.Current.SoloFeralRejuvenationIC && Me.ManaPercentage > 15 && Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && Me.Mana > (DruidBehavior.TransformValue + DruidBehavior.RejuvenationValue) && Me.HaveBuff("Regrowth") && !Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Healing Touch"), 1.70f, (s, t) => Settings.Current.SoloFeralHealingTouchIC && !Me.IsInGroup && Me.ManaPercentage > 15 &&  Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && Me.Mana > ((DruidBehavior.TransformValue * 0.6) + DruidBehavior.HealingTouchValue) && TalentsManager.HaveTalent(2,25) && Me.HaveBuff("Regrowth") && Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Healing Touch"), 1.71f, (s, t) => Settings.Current.SoloFeralHealingTouchIC && !Me.IsInGroup && Me.ManaPercentage > 15 &&  Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && Me.Mana > (DruidBehavior.TransformValue + DruidBehavior.HealingTouchValue) && Me.HaveBuff("Regrowth") && Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindMe),                        
            new RotationStep(new RotationSpell("Berserk"), 1.8f, (s,t) => (Me.HaveBuff("Bear Form") || Me.HaveBuff("Dire Bear Form") || Me.HaveBuff("Cat Form")) && (t.IsElite || RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <= 20) >=2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Bear Form"), 2f, (s, t) => Me.Level > 9 && Me.Level < 20, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Bash"), 2.1f, (s, t) => t.IsCasting() && Me.Level > 9 && Me.Level < 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Demoralizing Roar"), 3f, (s, t) => !t.HaveMyBuff("Demoralizing Roar")&& Me.Level > 9 && Me.Level < 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Swipe (Bear)"), 5f, (s, t) => Me.Level > 9 && Me.Level < 20 && t.GetDistance < 8, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Growl"), 6f, (s, t) => !Me.IsInGroup && Me.Level > 9 && Me.Level < 20 && t.GetDistance > 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Maul"), 7f, (s, t) => Me.Rage >= 16 && Me.Level > 9 && Me.Level < 20 && t.GetDistance < 8, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Moonfire"), 8f, (s, t) => !SpellManager.KnowSpell("Bear Form") && Me.Level > 9 && Me.Level < 20 && t.HealthPercent == 100 && !t.IsTargetingMe, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Wrath"), 9f, (s, t) => !SpellManager.KnowSpell("Bear Form") && Me.Level > 9 && Me.Level < 20, RotationCombatUtil.BotTarget),
            // Rotation 20-80
            new RotationStep(new RotationSpell("Regrowth"), 10f, (s, t) => Me.HaveBuff("Predator's Swiftness") && Me.HealthPercent < 70 && Me.ManaPercentage > 40, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Dire Bear Form"), 11f, (s, t) =>!Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <= 20) >= Settings.Current.SoloFeralBearCount, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Bear Form"), 12f, (s, t) =>!Me.IsInGroup && !SpellManager.KnowSpell("Dire Bear Form") && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <= 20) >= Settings.Current.SoloFeralBearCount, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Faerie Fire (Feral)"), 12.3f, (s, t) => (Me.HaveBuff("Bear Form") || Me.HaveBuff("Dire Bear Form")) && !Me.HaveBuff("Prowl") && !t.HaveMyBuff("Faerie Fire (Feral)") && Settings.Current.SoloFeralFaerieFire, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Bash"), 12.1f, (s, t) => t.IsCasting(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Enrage"), 12.2f, (s, t) =>t.HealthPercent >= 35 && !Me.HaveBuff("Enrage"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Frenzied Regeneration"), 13f, (s, t) => Me.HealthPercent < 60 && Me.Rage > 25, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Mangle (Bear)"), 14f, (s, t) => !t.HaveMyBuff("Mangle") && (Me.HaveBuff("Dire Bear Form") || Me.HaveBuff("Bear Form")), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Maul"), 15f, (s, t) => Me.Rage > 16 && (Me.HaveBuff("Dire Bear Form") || Me.HaveBuff("Bear Form")) && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 20) >= Settings.Current.SoloFeralBearCount, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Demoralizing Roar"), 16f, (s, t) =>!t.HaveBuff("Demoralizing Shout") && !t.HaveMyBuff("Demoralizing Roar") && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 20) >= Settings.Current.SoloFeralBearCount, RotationCombatUtil.BotTarget),
            // Cat Rotation
            new RotationStep(new RotationBuff("Cat Form"), 19f, (s, t) =>!Me.IsInGroup && (!Me.HaveBuff("Bear Form") || !Me.HaveBuff("Dire Bear Form")) && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <=12) <= (Settings.Current.SoloFeralBearCount - 1), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Cat Form"), 19.1f, (s, t) =>!Me.IsInGroup && (Me.HaveBuff("Bear Form") || Me.HaveBuff("Dire Bear Form")) && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <=12) <= (Settings.Current.SoloFeralBearCount - 2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Cat Form"), 19.2f, (s, t) => Me.IsInGroup, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Prowl"), 20f, (s, t) => t.HealthPercent > 99 && !Me.HaveBuff("Prowl") && !t.IsTargetingMe && Settings.Current.SoloFeralProwl, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Feral Charge - Cat"), 21f, (s, t) => t.GetDistance >= 15 && t.GetDistance <= 25 && Me.HaveBuff("Prowl") && !t.IsTargetingMe && Me.HaveBuff("Cat Form"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Pounce"), 22f, (s, t) => Me.HaveBuff("Prowl") && t.GetDistance <=5 && !t.IsTargetingMe, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Faerie Fire (Feral)"), 22.1f, (s, t) => Settings.Current.SoloFeralForceFaerie && !t.HaveMyBuff("Faerie Fire (Feral)"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Ravage"), 23f, (s, t) => Me.HaveBuff("Prowl") && Me.IsBehind(t.Position, 1.8f) && t.GetDistance <=6, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Faerie Fire (Feral)"), 23.1f, (s, t) => !Me.HaveBuff("Prowl") && !t.HaveMyBuff("Faerie Fire (Feral)") && Settings.Current.SoloFeralFaerieFire, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Dash"), 24f, (s, t) => Me.HaveBuff("Prowl") && !Me.HaveBuff("Dash") && Settings.Current.SoloFeralDash && Me.HaveBuff("Cat Form"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Tiger's Fury"), 25f, (s, t) => Me.HealthPercent < 92 && Me.ManaPercentage > 40 && !TalentsManager.HaveTalent(2,25), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Tiger's Fury"), 26f, (s, t) =>Me.ComboPoint <=4 && t.HealthPercent>=40 && Settings.Current.SoloFeralTigersFury && Me.HaveBuff("Cat Form") && !TalentsManager.HaveTalent(2,25), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Tiger's Fury"), 26.1f, (s, t) => Me.HealthPercent < 92 && Me.ManaPercentage > 40 && TalentsManager.HaveTalent(2,25)&& Me.HaveBuff("Cat Form") && Me.Rage <= 40, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Rake"), 27f, (s, t) =>!Me.HaveBuff("Prowl") && Me.ComboPoint <=4 && !t.HaveBuff("Rake") && (t.HealthPercent >= 35 || t.HealthPercent >= 20 && BossList.isboss) && !t.IsCreatureType("Elemental"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Rip"), 28f, (s,t) => Me.ComboPoint >= Settings.Current.SoloFeralFinisherComboPoints && !t.HaveMyBuff("Rip") && t.HealthPercent >= Settings.Current.SoloFeralRipHealth && !t.IsCreatureType("Elemental"),  RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Ferocious Bite"), 29f, (s, t) => Me.ComboPoint >= Settings.Current.SoloFeralFinisherComboPoints, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Mangle (Cat)"), 30f, (s, t) => !Me.HaveBuff("Prowl") && Me.ComboPoint <=4 && Settings.Current.SoloFeralTigersFury && Me.HaveBuff("Cat Form"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Claw"), 31f,(s,t)  => !Me.HaveBuff("Prowl"), RotationCombatUtil.BotTarget)
        };
    }
}