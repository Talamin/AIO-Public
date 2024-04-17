using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers.Caching;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    using Settings = WarriorLevelSettings;

    internal class SoloArms : BaseRotation
    {
        private bool _heroicStrikeOn;
        private bool _cleaveOn;
        private int _nbEnemiesAroundMe;
        private int _nbEnemiesAroundMeCasting;
        private readonly Spell _battleStanceSpell = new Spell("Battle Stance");
        List<WoWUnit> _enemiesAroundWithoutMyRend = new List<WoWUnit>();
        List<WoWUnit> _cleavableEnemies = new List<WoWUnit>();

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationAction("Precalculations", Precalculations), 0f, 500),
            new RotationStep(new RotationAction("Cache on spells", CacheActiveAbilities), 0f, 500),
            new RotationStep(new RotationAction("Check stance", CheckStance), 0f, 5000),

            new RotationStep(new RotationSpell("Charge"), 1f, (s,t) => t.GetDistance > 8 && !RangedPull.HasNearbyEnemies(t, 25), RotationCombatUtil.BotTargetFast, forcedTimerMS: 1000),
            new RotationStep(new RotationSpell("Victory Rush"), 2f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Overpower"), 2.5f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Execute"), 3f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            
            // Rage dumps
            new RotationStep(new RotationSpell("Cleave"), 4f, (s,t) => Me.CRage() > 50 && !_cleaveOn && !_heroicStrikeOn && _cleavableEnemies.Count > 0 && _nbEnemiesAroundMe >= Settings.Current.SoloArmsAoe, RotationCombatUtil.BotTargetFast, ignoreGCD: true),
            new RotationStep(new RotationSpell("Heroic Strike"), 5f, (s,t) => Me.CRage() > 50 && !_cleaveOn &&  !_heroicStrikeOn, RotationCombatUtil.BotTargetFast, ignoreGCD: true),
            
            // AOE
            new RotationStep(new RotationSpell("Bladestorm"), 5.5f, (s,t) => _nbEnemiesAroundMe >= Settings.Current.SoloArmsAoe, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Sweeping Strikes"), 6f, (s,t) => _nbEnemiesAroundMe >= Settings.Current.SoloArmsAoe, RotationCombatUtil.BotTargetFast, ignoreGCD: true),
            new RotationStep(new RotationSpell("Rend"), 7f, (s,t) => _nbEnemiesAroundMe >= Settings.Current.SoloArmsAoe && !t.CHaveMyBuff("Rend"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Rend"), 8.5f, (s,t) => Settings.Current.SoloArmsSpreadRend, p => _enemiesAroundWithoutMyRend.FirstOrDefault()),

            // Utility
            new RotationStep(new RotationSpell("Hamstring"), 9f, (s,t) => !t.CHaveBuff("Hamstring") && t.CHealthPercent() < 40 && t.CreatureTypeTarget == "Humanoid" && !BossList.MyTargetIsBoss && Settings.Current.Hamstring, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Enraged Regeneration"), 10f, (s,t) => Me.CHealthPercent() < Settings.Current.SoloArmsEnragedRegen, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Berserker Rage"), 11f, (s,t) => Me.CHaveBuff("Fear"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Intimidating Shout"), 13f, (s,t) => Settings.Current.SoloArmsIntimShout && _nbEnemiesAroundMeCasting > 0, RotationCombatUtil.BotTargetFast),
            
            // Single Target Rotation
            new RotationStep(new RotationSpell("Heroic Throw"), 15f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Bloodrage"), 16f, (s,t) => Me.CRage() < 50, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Rend"), 17f, (s,t) => !t.CHaveMyBuff("Rend"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Mortal Strike"), 18f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Slam"), 19f, (s,t) => Settings.Current.SoloArmsSlam && Me.CRage() > 20, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shattering Throw"), 20f, (s,t) => BossList.MyTargetIsBoss, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Bladestorm"), 21f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell("Demoralizing Shout"), 22f, (s,t) => Settings.Current.SoloArmsDemoShout && !t.CHaveBuff("Demoralizing Shout"), RotationCombatUtil.BotTargetFast),
        };

        private bool CheckStance()
        {
            if (_battleStanceSpell.KnownSpell && RotationCombatUtil.GetLUAActiveShapeshiftName() != "Battle Stance")
                _battleStanceSpell.Launch();
            return false;
        }

        private bool CacheActiveAbilities()
        {
            bool[] result = Lua.LuaDoString<bool[]>($@"
                local result = {{}};
                local cleavOn = IsCurrentSpell('Cleave') == 1;
                local hsOn = IsCurrentSpell('Heroic Strike') == 1;
                table.insert(result, cleavOn);
                table.insert(result, hsOn);
                return unpack(result);
            ");

            if (result.Length < 2) return false;

            _cleaveOn = result[0];
            _heroicStrikeOn = result[1];

            return false;
        }

        private bool Precalculations()
        {
            Cache.Reset();
            WoWUnit[] _enemiesAroundMe = RotationFramework.Enemies
                .Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember() && unit.CGetDistance() < 7)
                .ToArray();
            _nbEnemiesAroundMe = _enemiesAroundMe.Count();
            _nbEnemiesAroundMeCasting = _enemiesAroundMe
                .Where(enemy => enemy.CIsCast())
                .Count();
            _cleavableEnemies = _enemiesAroundMe
                .Where(enemy => Me.IsFacing(enemy.Position, 3))
                .ToList();
            _enemiesAroundWithoutMyRend = _cleavableEnemies
                .Where(enemy => enemy.CGetDistance() < 6 && !enemy.CHaveMyBuff("Rend") && !enemy.Name.Contains("Totem"))
                .ToList();
            return false;
        }
    }
}
