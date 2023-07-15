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
        private static readonly string Intercept = "Intercept";
        private readonly bool KnowIntercept = SpellManager.KnowSpell(Intercept);

        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations", ignoresGlobal: true), 0.0f,(action,unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Charge"), 1.5f, (s,t) => Settings.Current.GroupFuryCharge && t.CGetDistance() > 7, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Pummel"), 2f, (s,t) => t.CIsCast(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Slam"), 2.5f, (s,t) => Me.CHaveBuff("Slam!"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Hamstring"), 3f, (s,t) => !t.CHaveBuff("Hamstring") && t.CHealthPercent() < 40 && t.CreatureTypeTarget=="Humanoid" && !BossList.isboss && Settings.Current.Hamstring, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Bloodrage"), 4f, (s,t) => !Me.HaveBuff("Enraged Regeneration") && Me.CRage() < 50, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Berserker Rage"), 5f, (s,t) => !Me.HaveBuff("Enraged Regeneration") && Me.CRage() < 50, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Bloodthirst"), 7f, (s,t) => Me.CRage() > 35, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Death Wish"), 8f, (s,t) => Me.CRage() > 10, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Recklessness"), 8.5f, (s,t) => Me.CRage() > 10, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Execute"), 9f, (s1,t) => t.CHealthPercent() < 20, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Victory Rush"), 10f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Rend"), 11f, (s,t) => (!t.CHaveMyBuff("Rend") && !t.IsCreatureType("Elemental") && t.HealthPercent > 75) || (BossList.isboss && !t.IsCreatureType("Elemental")), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Thunder Clap"), 14f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10, 2), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Whirlwind"), 15f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10, 2), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Cleave"), 16f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10, 2), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Enraged Regeneration"), 17f, (s,t) => Me.HealthPercent < 40 && (Me.HaveBuff("Berserker Rage") || Me.HaveBuff("Bloodrage")), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Heroic Strike"), 18f, (s,t) => Me.CRage() > 40, RotationCombatUtil.BotTarget),
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
