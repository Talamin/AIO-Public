using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers.Caching;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Rogue
{
    using Settings = RogueLevelSettings;
    internal class GroupAssassination : BaseRotation
    {
        private bool _knowMutilate = SpellManager.KnowSpell("Mutilate");
        private bool _knowEnvenom = SpellManager.KnowSpell("Envenom");
        private bool _knowOverkill = TalentsManager.HaveTalent(1, 19);
        private bool _knowHungerForBlood = TalentsManager.HaveTalent(1, 27);
        private int _comboPoints;
        private int _nbEnemiesAroundMe;

        protected override List<RotationStep> Rotation => new List<RotationStep>
        {
            new RotationStep(new RotationAction("Precalculations", Precalculations), 0f, 200),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTargetFast),
            // Utility
            new RotationStep(new RotationSpell("Stealth"), 1.5f, (s,t) => _knowOverkill && t.GetDistance < 12, RotationCombatUtil.BotTargetFast), // trigger Overkill
            new RotationStep(new RotationSpell("Feint"), 2f, (s,t) => t.CIsTargetingMe(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Evasion"), 3f, (s,t) => t.CIsTargetingMe() && Me.HealthPercent < Settings.Current.GroupAssassEvasionHealth, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Cloak of Shadows"), 4f, (s,t) => Me.HealthPercent < Settings.Current.GroupAssassCoSHealth, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Blind"), 5f, (s,t) => Settings.Current.GroupAssassBlind && t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 10, RotationCombatUtil.FindEnemyCastingWithLoS),
            // AOE
            new RotationStep(new RotationSpell("Fan Of Knives"), 6f, (s,t) => _nbEnemiesAroundMe >= Settings.Current.GroupAssassFanOfKnives, RotationCombatUtil.BotTargetFast),
            // Single Target Rotation
            new RotationStep(new RotationSpell("Vanish"), 7f, (s,t) => _comboPoints >= 1 && BossList.MyTargetIsBoss && !Me.CHaveBuff("Overkill"), RotationCombatUtil.BotTargetFast), // trigger Overkill
            new RotationStep(new RotationSpell("Cold Blood"), 8f, (s,t) => _comboPoints >= 4, RotationCombatUtil.BotTargetFast, ignoreGCD: true),
            new RotationStep(new RotationSpell("Envenom"), 9f, (s,t) => _comboPoints >= 3, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Hunger For Blood"), 10f, (s,t) => !Me.CHaveBuff("Hunger For Blood"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Slice and Dice"), 11f, (s,t) => !Me.CHaveBuff("Cold Blood") && !Me.CHaveBuff("Slice and Dice"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Rupture"), 12f, (s,t) => _knowHungerForBlood && !Me.CHaveBuff("Hunger For Blood") && !t.CHaveMyBuff("Rupture"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Mutilate"), 13f, (s,t) => _comboPoints < 4, RotationCombatUtil.BotTargetFast),
            // Lower levels
            new RotationStep(new RotationSpell("Eviscerate"), 14f, (s,t) => !_knowEnvenom && _comboPoints >= 2, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Sinister Strike"), 15f, (s,t) => !_knowMutilate && _comboPoints < 4, RotationCombatUtil.BotTargetFast),
        };

        private bool Precalculations()
        {
            Cache.Reset();
            _comboPoints = Me.ComboPoint;
            _nbEnemiesAroundMe = RotationFramework.Enemies
                .Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember() && unit.CGetDistance() < 8)
                .Count();
            return false;
        }
    }
}
