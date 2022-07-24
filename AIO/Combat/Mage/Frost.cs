using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class Frost : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Mana Shield"), 1.1f, (s,t) => Me.HealthPercent <= 60 && Me.ManaPercentage >= 30 && !Me.HaveBuff("Mana Shield"), RotationCombatUtil.FindMe),
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
            new RotationStep(new RotationBuff("Ice Barrier"), 3f, (s,t) => t.HealthPercent < 95, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Ice Block"), 4f, (s,t) => (t.HealthPercent < 15 && !t.HaveMyBuff("Ice Barrier")) || (Me.IsInGroup && Me.HealthPercent < 85), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Cold Snap"), 5f, (s,t) => t.HealthPercent < 95 && !t.HaveMyBuff("Ice Barrier"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Counterspell"), 6f, (s,t) => t.IsCast, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell("Blizzard"), 7f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.AOEInstance && Settings.Current.UseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Frostfire Bolt"), 8f, (s,t) => Me.HaveMyBuff("Fireball!"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Cold Snap"), 9f, (s,t) => !Me.HaveBuff("Ice Barrier") && Me.HealthPercent < 95, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Evocation"), 10f, (s,t) => t.HealthPercent < 15 && RotationFramework.Enemies.Count() >= 2, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Mirror Image"), 11f, (s,t) => (!Me.IsInGroup && RotationFramework.Enemies.Count() >= 3) || BossList.isboss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Icy Veins"), 12f, (s,t) => (!Me.IsInGroup && RotationFramework.Enemies.Count() >= 2) || BossList.isboss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Summon Water Elemental"), 13f, (s,t) => (!Me.IsInGroup && RotationFramework.Enemies.Count() >= 2) || BossList.isboss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Deep Freeze"), 14f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh , RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Ice Lance"), 15f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh  && Me.HaveBuff("Fingers of Frost") || t.HaveMyBuff("Frost Nova"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Fireball"), 16f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh  && !SpellManager.KnowSpell("Frostbolt"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Fire Blast"), 17f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh  && t.HealthPercent < Settings.Current.FrostFireBlast && !t.HaveBuff("Frost Nova"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Frostbolt"), 18f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh , RotationCombatUtil.BotTarget)
        };
    }
}
