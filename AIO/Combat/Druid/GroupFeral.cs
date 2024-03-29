using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Class;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;

    internal class GroupFeral : BaseRotation
    {
        private readonly Spell _catFormSpell = new Spell("Cat Form");
        private readonly Spell _bearFormSpell = new Spell("Bear Form");
        private readonly Spell _direBearFormSpell = new Spell("Dire Bear Form");
        private readonly int _nbPointsKotJTalent = TalentsManager.GetNbPointsTalent(2, 25);

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            // Rotation 11-20
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTargetFast),

            // High prio
            new RotationStep(new RotationBuff("Innervate"), 1.2f, (s, t) => t.ManaPercentage <= 25 && Settings.Current.GroupFeralInnervateHealer, RotationCombatUtil.FindHeal),            
            new RotationStep(new RotationBuff("Barkskin"), 1.4f, (s, t) => Me.HealthPercent <= 35, RotationCombatUtil.FindMe),
            
            // bear
            new RotationStep(new RotationBuff("Bear Form"), 2f, (s, t) => !_catFormSpell.KnownSpell && !_direBearFormSpell.KnownSpell, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Dire Bear Form"), 3f, (s, t) => !_catFormSpell.KnownSpell, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Frenzied Regeneration"), 4f, (s, t) => Me.HealthPercent < 60 && Me.Rage > 25, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Faerie Fire (Feral)"), 5f, (s, t) => !t.HaveMyBuff("Faerie Fire (Feral)") && Settings.Current.GroupFeralUseFaerieFire, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Mangle (Bear)"), 6f, (s, t) => !t.HaveMyBuff("Mangle"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Enrage"), 7f, (s, t) => t.HealthPercent >= 35, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Bash"), 8f, (s, t) => t.IsCasting(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Demoralizing Roar"), 9f, (s, t) => !t.HaveMyBuff("Demoralizing Roar"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Swipe (Bear)"), 10f, (s, t) => RotationFramework.Enemies.Count(o => Me.IsFacing(o.Position, 3) && o.HasTarget && !o.IsTargetingMe && o.Position.DistanceTo(Me.Position) <= 8) >= 2, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Maul"), 11f, (s, t) => Me.Rage >= 16, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Demoralizing Roar"), 12f, (s, t) => !t.HaveBuff("Demoralizing Shout") && Settings.Current.GroupFeralUseDemoralizingRoar, RotationCombatUtil.BotTargetFast),

            // human
            new RotationStep(new RotationSpell("Moonfire"), 13f, (s, t) => !_direBearFormSpell.KnownSpell && !_bearFormSpell.KnownSpell && !_catFormSpell.KnownSpell && !t.HaveBuff("Moonfire"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Wrath"), 14f, (s, t) => !_direBearFormSpell.KnownSpell && !_bearFormSpell.KnownSpell && !_catFormSpell.KnownSpell, RotationCombatUtil.BotTargetFast),

            // meow
            new RotationStep(new RotationBuff("Cat Form"), 15f, (s, t) => Me.IsInGroup, RotationCombatUtil.FindMe),

            // stealth
            new RotationStep(new RotationSpell("Pounce"), 16f, (s, t) => true, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Ravage"), 17f, (s, t) => true, RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell("Feral Charge - Cat"), 18f, (s, t) => t.GetDistance > 7 && RotationFramework.PartyMembers.Any(m => m.Position.DistanceTo(t.Position) < 7), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Dash"), 19f, (s, t) => t.GetDistance > 10 && RotationFramework.PartyMembers.Any(m => m.Position.DistanceTo(t.Position) < 7), RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Berserk"), 20f, (s,t) => Me.HaveBuff("Cat Form") && (RotationFramework.Enemies.Count(o => Me.IsFacing(o.Position, 3) && o.Position.DistanceTo(Me.Position) <= 8) >= 2 || t.IsBoss), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Tiger's Fury"), 21f, (s, t) => Me.Energy < 100 - (_nbPointsKotJTalent * 20) && t.GetDistance < 10, RotationCombatUtil.BotTargetFast),

            // finisher
            new RotationStep(new RotationSpell("Savage Roar"), 22f, (s,t) => !Me.HaveBuff("Savage Roar") && Me.ComboPoint >= Settings.Current.GroupFeralFinisherComboPoints, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Rip"), 23f, (s,t) => Me.ComboPoint >= Settings.Current.GroupFeralFinisherComboPoints && t.HealthPercent > 50 && !t.HaveMyBuff("Rip") && !t.IsCreatureType("Elemental"),  RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Ferocious Bite"), 24f, (s, t) => Me.ComboPoint >= Settings.Current.GroupFeralFinisherComboPoints, RotationCombatUtil.BotTargetFast),

            // combo points
            new RotationStep(new RotationSpell("Rake"), 27f, (s, t) => !t.HaveBuff("Rake") && !t.IsCreatureType("Elemental"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shred"), 29f, (s, t) => !t.IsFacing(Me.Position, 4), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Mangle (Cat)"), 30f, (s, t) => (t.IsFacing(Me.Position, 3) || !t.HaveBuff("Mangle (Cat)")) && !Me.HaveBuff("Prowl"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Claw"), 31f,(s,t)  => t.IsFacing(Me.Position, 3) && !Me.HaveBuff("Prowl"), RotationCombatUtil.BotTargetFast)
        };
    }
}