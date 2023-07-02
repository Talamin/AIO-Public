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
    internal class GroupFrost : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations", ignoresGlobal: true), 0.0f,(action,unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.CManaPercentage() < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.CIsCast() && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Mana Shield"), 1.1f, (s,t) => Me.CHealthPercent() <= 60 && Me.CManaPercentage() >= 30 && !Me.HaveBuff("Mana Shield"), RotationCombatUtil.FindMe),
            //// Only cast Polymorph if Sheep is enabled in settings
            //new RotationStep(new RotationSpell("Polymorph"), 2.1f, (s,t) => Settings.Current.Sheep 
            //// Only cast Polymorph if more than one enemy is targeting the Mage
            //&& !t.IsMyTarget && RotationFramework.Enemies.Count(o => o.IsTargetingMe) > 1 
            //// Make sure no enemies in 30 yard casting range are polymorphed right now
            //&& RotationFramework.Enemies.Count(o => o.GetDistance <= 30 && o.HaveBuff("Polymorph")) < 1
            //// Only polymorph a valid target
            //&& (t.IsCreatureType("Humanoid") || t.IsCreatureType("Beast") || t.IsCreatureType("Critter")),
            //    RotationCombatUtil.FindEnemyTargetingMe),
            //new RotationStep(new RotationSpell("Frost Nova"), 2.2f, (s,t) => t.GetDistance <= 6 && t.HealthPercent > 30 && !Me.IsInGroup, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Ice Barrier"), 3f, (s,t) => t.CHealthPercent() < 99, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Ice Block"), 4f, (s,t) =>  Me.CHealthPercent() < 30 && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.CIsTargetingMe(), 1), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Cold Snap"), 5f, (s,t) => t.CHealthPercent() < 80 && !t.HaveMyBuff("Ice Barrier"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Counterspell"), 6f, (s,t) => t.CIsCast(), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Cone of Cold"), 7f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10, Settings.Current.GroupFrostAOEInstance) && Settings.Current.GroupFrostUseAOE, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Blizzard"), 7f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 45 && !EnemiesAttackingGroup.Any(ene => ene.CIsTargetingMe() ), Settings.Current.GroupFrostAOEInstance) && Settings.Current.GroupFrostUseAOE, FindBlizzardCluster, checkLoS: true),
            new RotationStep(new RotationSpell("Frostfire Bolt"), 8f, (s,t) => Me.CHaveMyBuff("Fireball!"), RotationCombatUtil.BotTargetFast, checkLoS: true),

            new RotationStep(new RotationSpell("Cold Snap"), 9f, (s,t) => !Me.CHaveBuff("Ice Barrier") && Me.CHealthPercent() < 95, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Evocation"), 10f, (s,t) =>  Settings.Current.GlyphOfEvocation && t.CHealthPercent() < 20 && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Mirror Image"), 11f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 3) || BossList.isboss, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Icy Veins"), 12f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 3) || BossList.isboss, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Summon Water Elemental"), 13f, (s,t) => !Settings.Current.GlyphOfEternalWater && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 3) || BossList.isboss, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Deep Freeze"), 14f, (s,t) => Me.CManaPercentage() > Settings.Current.UseWandTresh , RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Ice Lance"), 15f, (s,t) => Me.CManaPercentage() > Settings.Current.UseWandTresh && (Me.CBuffStack("Fingers of Frost") > 0 || t.CHaveMyBuff("Frost Nova")), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Fireball"), 16f, (s,t) => Me.CManaPercentage() > Settings.Current.UseWandTresh  && !SpellManager.KnowSpell("Frostbolt"), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Fire Blast"), 17f, (s,t) => Me.CManaPercentage() > Settings.Current.UseWandTresh  && t.CHealthPercent() < Settings.Current.GroupFrostFrostFireBlast , RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Frostbolt"), 18f, (s,t) =>  Me.CManaPercentage() > Settings.Current.UseWandTresh , RotationCombatUtil.BotTargetFast, checkLoS: true)
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
