using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class GroupFire : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations", ignoresGlobal: true), 0.0f,(action,unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Frost Nova"), 2f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.IsElite, 2), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Ice Block"), 3f, (s,t) => Me.CHealthPercent() < 30 && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.CIsTargetingMe(), 1), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Evocation"), 4f, (s,t) => Settings.Current.GlyphOfEvocation && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Pyroblast"), 4.5f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh && Me.HaveBuff("Hot Streak") && t.HealthPercent > 10, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Living Bomb"), 5f, (s,t) => !t.CHaveMyBuff("Living Bomb") && RotationFramework.Enemies.Count() >= 2, RotationCombatUtil.FindEnemyAttackingGroup, checkLoS: true),
            new RotationStep(new RotationSpell("Flamestrike"), 6f, (s,t) => Settings.Current.GroupFireFlamestrikeWithoutFire && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.GroupFireFlamestrikeWithoutCountFire && Settings.Current.GroupFireUseAOE, RotationCombatUtil.BotTargetFast, checkLoS: true, forcedTimerMS: 10000),
            new RotationStep(new RotationSpell("Blizzard"), 7f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 45 && !EnemiesAttackingGroup.Any(ene => ene.CIsTargetingMe() ), Settings.Current.GroupFireAOEInstance) && Settings.Current.GroupFireUseAOE, FindBlizzardCluster, checkLoS: true),
            new RotationStep(new RotationSpell("Scorch"), 9f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh && TalentsManager.HaveTalent(2,11) && !t.HaveMyBuff("Improved Scorch"), RotationCombatUtil.BotTarget, forcedTimerMS: 100),
            new RotationStep(new RotationSpell("Combustion"), 10f, (s,t) => t.HaveMyBuff("Combustion"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Blast Wave"), 11f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh && t.CGetDistance() < 7 && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 15) > 1, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Dragon's Breath"), 12f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh && t.CGetDistance() < 7 && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 15) > 1, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Living Bomb"), 13f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh && !t.CHaveMyBuff("Living Bomb"), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Fire Blast"), 14f, (s,t) => Settings.Current.GroupFireUseFireBlast && Me.ManaPercentage > Settings.Current.UseWandTresh && t.CHealthPercent() < 10, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Fireball"), 15f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh && (t.CHealthPercent() >= 10 || BossList.isboss), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Scorch"), 16f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh && (t.CHealthPercent() < 10 || BossList.isboss) , RotationCombatUtil.BotTargetFast, checkLoS: true),
            
            //// Only cast Polymorph if Sheep is enabled in settings
            //new RotationStep(new RotationSpell("Polymorph"), 2.1f, (s,t) => Settings.Current.Sheep 
            //// Only cast Polymorph if more than one enemy is targeting the Mage
            //&& !t.IsMyTarget && RotationFramework.Enemies.Count(o => o.IsTargetingMe) > 1 
            //// Make sure no enemies in 30 yard casting range are polymorphed right now
            //&& RotationFramework.Enemies.Count(o => o.GetDistance <= 30 && o.HaveBuff("Polymorph")) < 1
            //// Only polymorph a valid target
            //&& (t.IsCreatureType("Humanoid") || t.IsCreatureType("Beast") || t.IsCreatureType("Critter")),
            //    RotationCombatUtil.FindEnemyTargetingMe),
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

        private static WoWUnit FindBlizzardCluster(Func<WoWUnit, bool> predicate)
        {
            WoWUnit largestCenter = null;
            int largestCount = 2;
            for (var i = 0; i < RotationFramework.Enemies.Length; i++)
            {
                WoWUnit originUnit = RotationFramework.Enemies[i];
                if (!predicate(originUnit))
                {
                    continue;
                }
                Vector3 originPos = originUnit.CGetPosition();
                int localCount = RotationFramework.Enemies.Count(enemy => enemy.CIsAlive() && enemy.CGetPosition().DistanceTo(originPos) < 10 && enemy.CIsTargetingMeOrMyPetOrPartyMember());

                if (localCount > largestCount)
                {
                    largestCenter = originUnit;
                    largestCount = localCount;
                }
            }
            return largestCenter;
        }
    }

}
