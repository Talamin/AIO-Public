using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Rogue
{
    using Settings = RogueLevelSettings;
    internal class GroupCombat : BaseRotation
    {

        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f,(action, unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange : false, forceCast : true, ignoreGCD : true),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Sprint"), 2f, RotationCombatUtil.Always, _ => !EnemiesAttackingGroup.Any(unit => unit.CGetDistance() <=10), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Kick"), 3f, (s,t) => t.CIsCast() && t.CGetDistance() < 7, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Evasion"), 3.1f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.ContainsAtLeast(unit => unit.CGetDistance() <=15 && unit.CIsTargetingMe() , Settings.Current.GroupCombatEvasion) && Me.CHealthPercent() < 80, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Riposte"), 4f, (s, t) => !Me.CHaveBuff("Stealth"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Blade Flurry"), 5f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.ContainsAtLeast(unit => unit.CGetDistance() <=10, Settings.Current.GroupCombatBladeFLurry), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Adrenaline Rush"), 6f, (s,t) =>!Me.CHaveBuff("Stealth") && RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=Settings.Current.GroupCombatAdrenalineRush, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Slice and Dice"), 7f, (s, t) => !Me.CHaveBuff("Slice and Dice") && Me.ComboPoint >= 1, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Eviscerate"), 8f, (s, t) => Me.ComboPoint >= Settings.Current.GroupCombatEviscarate, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Killing Spree"), 9f, (s, t) =>!Me.CHaveBuff("Adrenaline Rush") && !Me.CHaveBuff("Blade Flurry") && EnemiesAttackingGroup.ContainsAtLeast(unit => unit.CGetDistance() <=10,Settings.Current.GroupCombatKillingSpree), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Sinister Strike"), 10f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
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