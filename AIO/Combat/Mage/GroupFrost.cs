﻿using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Class;
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
        private readonly bool _knowsFrostFireBolt = SpellManager.KnowSpell("Frostfire Bolt");
        private readonly bool _knowsFrostBolt = SpellManager.KnowSpell("Frostbolt");
        private int _fingersOfFrostStacks;

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations", ignoresGlobal: true), 0.0f,(action,unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.CIsCast() && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Mana Shield"), 1.1f, (s,t) => Me.CHealthPercent() <= 40 && Me.CManaPercentage() >= 30 && !Me.HaveBuff("Mana Shield"), RotationCombatUtil.FindMe),
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
            new RotationStep(new RotationBuff("Ice Barrier"), 3f, (s,t) => t.CHealthPercent() < 60, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Cold Snap"), 3.5f, (s,t) => !Me.CHaveBuff("Ice Barrier") && Me.CHealthPercent() < 60, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Ice Block"), 4f, (s,t) =>  Me.CHealthPercent() < 30 && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.CIsTargetingMe(), 1), RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Counterspell"), 5f, (s,t) => t.CIsCast(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Cone of Cold"), 6f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10, Settings.Current.GroupFrostAOEInstance) && Settings.Current.GroupFrostUseAOE, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Blizzard"), 7f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 45 && !EnemiesAttackingGroup.Any(ene => ene.CIsTargetingMe() ), Settings.Current.GroupFrostAOEInstance) && Settings.Current.GroupFrostUseAOE, FindBlizzardCluster),

            new RotationStep(new RotationSpell("Evocation"), 8f, (s,t) =>  Settings.Current.GlyphOfEvocation && t.CHealthPercent() < 20, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Evocation"), 9f, (s,t) =>  t.CManaPercentage() < 20, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Mirror Image"), 10f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 3) || BossList.MyTargetIsBoss, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Icy Veins"), 11f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 3) || BossList.MyTargetIsBoss, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Summon Water Elemental"), 12f, (s,t) => !Settings.Current.GlyphOfEternalWater && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 3) || BossList.MyTargetIsBoss, RotationCombatUtil.BotTargetFast),
            
            // Brain Freeze
            new RotationStep(new RotationSpell("Frostfire Bolt"), 13f, (s,t) => Me.CHaveBuff("Fireball!"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Fireball"), 14f, (s,t) => !_knowsFrostFireBolt && Me.CHaveBuff("Fireball!"), RotationCombatUtil.BotTargetFast),

            // Fingers of Frost/Frost Nova
            new RotationStep(new RotationSpell("Deep Freeze"), 15f, (s,t) => _fingersOfFrostStacks == 1 || t.CHaveMyBuff("Frost Nova"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Ice Lance"), 16f, (s,t) => _fingersOfFrostStacks == 1 || t.CHaveMyBuff("Frost Nova"), RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell("Fire Blast"), 17f, (s,t) => t.CHealthPercent() < Settings.Current.GroupFrostFrostFireBlast , RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Frostbolt"), 18f, (s,t) =>  true, RotationCombatUtil.BotTargetFast, checkLoS: true),

            new RotationStep(new RotationSpell("Fireball"), 20f, (s,t) => !_knowsFrostBolt, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Shoot"), 25f, (s,t) => Me.CManaPercentage() < 5 && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTargetFast, checkLoS: true),
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
            // Getting auras manually because CBuffStack doesn't seem to return correct values
            foreach (Aura aura in BuffManager.GetAuras(Me.GetBaseAddress).ToList())
            {
                if (aura.SpellId == 74396)
                {
                    _fingersOfFrostStacks = aura.Stack;
                    break;
                }
            }
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
