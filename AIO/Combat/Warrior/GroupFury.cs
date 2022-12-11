using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    using Settings = WarriorLevelSettings;
    internal class GroupFury : BaseRotation
    {
        private static readonly string Intercept = "Blessing of Sanctuary";
        private readonly bool KnowIntercept = SpellManager.KnowSpell(Intercept);

        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations", ignoresGlobal: true), 0.0f,(action,unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Pummel"), 2f, (s,t) => t.CIsCast(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Hamstring"), 3f, (s,t) => !t.CHaveBuff("Hamstring") && t.CHealthPercent() < 40 && t.CreatureTypeTarget=="Humanoid" && !BossList.isboss && Settings.Current.Hamstring, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Piercing Howl"), 4f, (s,t) => t.CHealthPercent() < 40 && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10, 2), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Bloodrage"), 5f, (s,t) => t.CGetDistance() < 7, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Slam"), 6f, (s,t) => Me.CHaveBuff("Slam!"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Bloodthirst"), 7f, (s,t) => Me.CRage() > 30 && Me.CHealthPercent() <= 80, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Death Wish"), 8f, (s,t) => Me.CRage()> 10, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Execute"), 9f, (s1,t) => t.CHealthPercent() < 20, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Victory Rush"), 10f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Rend"), 11f, (s,t) => !t.CHaveMyBuff("Rend") && !t.IsCreatureType("Elemental"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Intercept"), 12f, (s,t) => !KnowIntercept && Settings.Current.FuryIntercept && Me.CRage() > 10 && t.CGetDistance() > 7 && t.CGetDistance() <= 24, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Charge"), 13f, (s,t) => !KnowIntercept && Settings.Current.FuryIntercept && t.CGetDistance() > 7, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Thunder Clap"), 14f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10, 2), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Whirlwind"), 15f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10, 2), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Cleave"), 16f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10, 2), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Heroic Strike"), 17f, (s,t) => Me.CRage() > 40, RotationCombatUtil.BotTarget),
        };
        private bool DoPreCalculations()
        {
            if (LimitExecutionSpeed(100))
            {
                return true;
            }
            Cache.Reset();
            EnemiesAttackingGroup = RotationFramework.Enemies.Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember())
                .ToArray();
            return false;
        }

        private bool LimitExecutionSpeed(int delay)
        {
            if (watch.ElapsedMilliseconds > delay)
            {
                watch.Restart();
                return false;
            }
            return true;
        }

        public WoWUnit FindEnemyAttackingGroup(Func<WoWUnit, bool> predicate) => EnemiesAttackingGroup.FirstOrDefault(predicate);
    }
}
