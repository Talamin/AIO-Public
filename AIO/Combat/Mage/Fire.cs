using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class Fire : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            // Only cast Polymorph if Sheep is enabled in settings
            new RotationStep(new RotationSpell("Polymorph"), 2.1f, (s,t) => Settings.Current.Sheep 
            // Only cast Polymorph if more than one enemy is targeting the Mage
            && !t.IsMyTarget && RotationFramework.Enemies.Count(o => o.IsTargetingMe) > 1 
            // Make sure no enemies in 30 yard casting range are polymorphed right now
            && RotationFramework.Enemies.Count(o => o.GetDistance <= 30 && o.HaveBuff("Polymorph")) < 1
            // Only polymorph a valid target
            && (t.IsCreatureType("Humanoid") || t.IsCreatureType("Beast") || t.IsCreatureType("Critter")),
                RotationCombatUtil.FindEnemyTargetingMe),
            new RotationStep(new RotationSpell("Frost Nova"), 2.2f, (s,t) => t.GetDistance <= 6 && t.HealthPercent > 30 && !Me.IsInGroup, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Ice Block"), 3f, (s,t) => (t.HealthPercent < 15 && !t.HaveMyBuff("Ice Barrier")) || (Me.IsInGroup && Me.HealthPercent < 85), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Evocation"), 3.5f, (s,t) => Me.ManaPercentage < 35, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Pyroblast"), 4f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh && Me.HaveBuff("Hot Streak") && t.HealthPercent > 10, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Living Bomb"), 5f, (s,t) => !t.HaveMyBuff("Living Bomb") && RotationFramework.Enemies.Count() >= 2, RotationCombatUtil.FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell("Flamestrike"), 6f, (s,t) => Settings.Current.FlamestrikeWithoutFire && !t.HaveMyBuff("Flamestrike") && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.FlamestrikeWithoutCountFire && Settings.Current.UseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blizzard"), 7f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.AOEInstance && Settings.Current.UseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Scorch"), 9f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh && TalentsManager.HaveTalent(2,11) &&  !t.HaveMyBuff("Improved Scorch"), RotationCombatUtil.BotTarget),
            //new RotationStep(new RotationSpell("Combustion"), 10f, (s,t) => t.HaveMyBuff("Combustion"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Blast Wave"), 11f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  t.GetDistance < 7 && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 15) > 1, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Dragon's Breath"), 12f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  t.GetDistance < 7 && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 15) > 1, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Living Bomb"), 13f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  !t.HaveMyBuff("Living Bomb"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Fire Blast"), 14f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  t.HealthPercent < 10, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Fireball"), 15f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  (t.HealthPercent > 55 || BossList.isboss), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Scorch"), 16f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  (t.HealthPercent < 35 || BossList.isboss) , RotationCombatUtil.BotTarget),
        };
    }
}
