using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class Balance : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Regrowth"), 1.1f, (s,t) => !Me.HaveBuff("Regrowth") && Me.HealthPercent < Settings.Current.BalanceRegrowth, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Healing Touch"), 1.2f, (s ,t) => Me.HealthPercent < Settings.Current.BalanceHealingTouch, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Innervate"), 1.3f, (s,t) => Me.IsInGroup && t.Name == RotationFramework.HealName && t.ManaPercentage < Settings.Current.Innervate, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Innervate"), 1.4f, (s,t) => Me.ManaPercentage <= Settings.Current.Innervate, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Moonkin Form"), 2f, (s, t) => !Me.HaveBuff("Moonkin Form"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Barkskin"), 3f, (s, t) => RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <= 20) >= 2 || (!Me.IsInGroup && Me.InCombat), RotationCombatUtil.FindMe),
            //new RotationStep(new RotationSpell("Abolish Poison"), 3.1f, (s,t) =>  !t.HaveMyBuff("Abolish Poison") && (t.HaveBuff("Venom Sting") || t.HaveBuff("Leech Poison")), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Abolish Poison"), 3.2f, (s,t) =>  !t.HaveMyBuff("Abolish Poison") && t.HaveImportantPoison(), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Remove Curse"), 3.3f, (s,t) => t.HaveImportantCurse(), RotationCombatUtil.FindPartyMember),
            //new RotationStep(new RotationSpell("Remove Curse"), 3.3f, (s,t) => t.HaveBuff("Veil of Shadow") || t.HaveBuff("Curse of Tongues")|| t.HaveBuff("Curse of Tuten'kash") || t.HaveBuff("Curse of the Deadwood"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Typhoon"), 4f, (s, t) => t.GetDistance < 7 && RotationFramework.Enemies.Count(u=> u.HealthPercent >= 20 && u.IsTargetingMe) > 1, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Starfall"), 5f, RotationCombatUtil.Always, s=> BossList.isboss || (Me.IsInGroup && RotationCombatUtil.EnemyAttackingCountCluster(30) > 2 && Settings.Current.UseStarfall), RotationCombatUtil.BotTarget),
            //new RotationStep(new RotationSpell("Starfall"), 6f, (s, t) => Settings.Current.UseStarfall && BossList.isboss || (Me.IsInGroup && (RotationFramework.AllUnits.Count(o => o.IsAlive  && o.Reaction == Reaction.Hostile && o.Position.DistanceTo(t.Position) <= 30) >= 2)), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Starfall"), 6.1f, (s,t) =>!Me.IsInGroup && t.HealthPercent >= 50 && SpellManager.GetSpellCooldownTimeLeft(33831) > 1 && RotationFramework.AllUnits.Count(u=> u.IsMyPet) <= 0 && (RotationFramework.AllUnits.Count(o => o.IsAlive  && o.Reaction <= Reaction.Neutral && o.IsTargetingMeOrMyPet) >= 2) && Settings.Current.UseStarfall, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Force of Nature"), 6.5f, RotationCombatUtil.Always, s => BossList.isboss || (Me.IsInGroup && (RotationCombatUtil.EnemyAttackingCountCluster(30) >= Settings.Current.AOEInstance)), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Force of Nature"), 6.6f, (s, t) => !Me.IsInGroup && t.HealthPercent >= 50 && (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= 2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hurricane"), 7f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.AOEInstance && Settings.Current.UseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Starfire"), 8f, (s, t) => t.HealthPercent == 100 && !t.IsTargetingMe, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Insect Swarm"), 9f, (s, t) => !t.HaveMyBuff("Insect Swarm") && (t.Health > 35 || t.IsBoss) && !Me.HaveBuff("Eclipse (Lunar)") && !Me.HaveBuff("Eclipse (Solar)"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Faerie Fire"), 0f, (s, t) => !t.HaveBuff("Faerie Fire") && t.IsBoss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Moonfire"), 13f, (s, t) => !t.HaveMyBuff("Moonfire") && (t.HealthPercent >= 60 || t.IsBoss) && !Me.HaveBuff("Eclipse (Lunar)") && !Me.HaveBuff("Eclipse (Solar)"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Starfire"), 14f, (s, t) => t.HealthPercent >= 10 && !Me.HaveBuff("Eclipse (Solar)") && Me.HaveBuff("Nature's Grace") || Me.HaveBuff("Eclipse (Lunar)"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Wrath"), 15f, (s, t) => Me.HaveBuff("Eclipse (Solar)"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Wrath"), 16f, (s, t) => !Me.HaveBuff("Eclipse (Solar)") && !Me.HaveBuff("Eclipse (Lunar)"), RotationCombatUtil.BotTarget),
        };
    }
}