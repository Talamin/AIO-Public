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
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class GroupBeastMastery : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations", ignoresGlobal: true), 0.0f,(action,unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Auto Shoot"), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Feign Death"), 2f, (s,t) => Me.CHealthPercent() < 50 && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.CIsTargetingMe(), 1) && Settings.Current.GroupBeastMasteryFD, RotationCombatUtil.FindMe, checkLoS: false),
            new RotationStep(new RotationSpell("Deterrence"), 2.1f, (s,t) => Me.CHealthPercent() < 80 && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 25 && u.CIsTargetingMe(), 1) && Settings.Current.GroupBeastMasteryDeterrence, RotationCombatUtil.FindMe, checkLoS: false),
            new RotationStep(new RotationSpell("Intimidation"), 3f, (s,t) => Pet.Target != 0 && Pet.CGetPosition().DistanceTo(t.CGetPosition()) <= 6 && t.CIsCast(), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Concussive Shot"), 3.1f, (s,t) => t.Fleeing && !t.CHaveBuff("Concussive Shot"), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Misdirection"), 3.3f, 
                (action, tank) => Settings.Current.GroupBeastMasteryMisdirection && !Me.CHaveBuff("Misdirection") && tank.CInCombat() && tank.CIsAlive() , RotationCombatUtil.FindTank, checkLoS: true), 
            //Push Aggro to Tank
            new RotationStep(new RotationSpell("Multi-Shot"), 3.4f, (s,t) => Me.CHaveBuff("Misdirection"), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Volley"), 4f,
                (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 45 && !EnemiesAttackingGroup.Any(ene => ene.CIsTargetingMe()), Settings.Current.GroupBeastMasteryAOECount) && Settings.Current.GroupBeastMasteryUseAOE, FindVolleyCluster, checkLoS:true),
            new RotationStep(new RotationSpell("Kill Shot"), 5f, (s,t) => t.CGetDistance() >= 5 && t.CHealthPercent() < 20, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Hunter's Mark"), 9f, (s,t) => t.CGetDistance() >= 5 && !t.CHaveMyBuff("Hunter's Mark") && t.CIsAlive() &&  t.CHealthPercent() > 60, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Bestial Wrath"), 10f, (s,t) => t.IsBoss || EnemiesAttackingGroup.Count() > 3, RotationCombatUtil.BotTargetFast, checkLoS: true), 
            new RotationStep(new RotationSpell("Rapid Fire"), 11f, (s,t) => t.IsBoss || EnemiesAttackingGroup.Count() > 3, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Kill Command"), 12f, (s,t) => !Me.CHaveBuff("Kill Command"), RotationCombatUtil.BotTargetFast, checkLoS: false),            
            new RotationStep(new RotationSpell("Serpent Sting"), 13f, (s,t) => t.CGetDistance() >= 5 && !t.CHaveMyBuff("Serpent Sting") && (t.CHealthPercent() >= 70 || (t.IsBoss && t.CHealthPercent() >= 20)), RotationCombatUtil.BotTargetFast, checkLoS:true),
            new RotationStep(new RotationSpell("Arcane Shot"), 14f, (s,t) => t.CGetDistance() >= 5, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Multi-Shot"), 15f, (s,t) => t.CGetDistance() >= 5 && Settings.Current.GroupBeastMasteryMultiShot && EnemiesAttackingGroup.Count() >= Settings.Current.GroupBeastMasteryMultiShotCount, RotationCombatUtil.BotTargetFast, checkLoS:true),                   
            new RotationStep(new RotationSpell("Steady Shot"), 15.1f, (s,t) => !Me.GetMove && t.CGetDistance() >= 5, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Raptor Strike"), 16f, (s,t) => t.CGetDistance() < 5, RotationCombatUtil.BotTargetFast, checkLoS: true),

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

        private static WoWUnit FindVolleyCluster(Func<WoWUnit, bool> predicate)
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

