using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers.Caching;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class SoloBalance : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Tranquility"), 3f, UseTranquility, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Regrowth"), 4f, (s,t) => !Me.HaveBuff("Regrowth") && Me.HealthPercent < Settings.Current.SoloBalanceRegrowth, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Healing Touch"), 5f, (s ,t) => Me.HealthPercent < Settings.Current.SoloBalanceHealingTouch, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Regrowth"), 6f, (s,t) => !t.HaveBuff("Regrowth") && t.HealthPercent < Settings.Current.SoloBalanceRegrowth, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Healing Touch"), 7f, (s ,t) => t.HealthPercent < Settings.Current.SoloBalanceHealingTouch, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Innervate"), 8f, (s,t) => Me.IsInGroup && t.Name == RotationFramework.HealName && t.ManaPercentage < Settings.Current.Innervate, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Innervate"), 9f, (s,t) => Me.ManaPercentage <= Settings.Current.Innervate, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Moonkin Form"), 10f, (s, t) => !Me.HaveBuff("Moonkin Form"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Barkskin"), 11f, (s, t) => RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <= 20) >= 2 || (!Me.IsInGroup && Me.InCombat), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Abolish Poison"), 12f, (s,t) =>  !t.HaveMyBuff("Abolish Poison") && t.HaveImportantPoison(), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Remove Curse"), 13f, (s,t) => t.HaveImportantCurse(), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Starfall"), 13.5f, (s,t) => (BossList.isboss && t.HealthPercent > 25) || (RotationFramework.Enemies.Count(o => o.IsElite && o.Position.DistanceTo(t.Position) <= 33) >= 3 && Settings.Current.SoloBalanceUseStarfall), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Typhoon"), 14f, (s, t) => t.GetDistance < 30 && RotationFramework.Enemies.Count(u => u.IsTargetingMeOrMyPetOrPartyMember) >= 3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Starfall"), 14.5f, (s,t) => !Me.IsInGroup && t.HealthPercent >= 50 && (RotationFramework.AllUnits.Count(o => o.IsAlive && o.IsTargetingMeOrMyPet) >= 2) && Settings.Current.SoloBalanceUseStarfall, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Force of Nature"), 15f, (s, t) => BossList.isboss || (Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.IsElite) >= Settings.Current.SoloBalanceAOETargets), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Force of Nature"), 16f, (s, t) => !Me.IsInGroup && t.HealthPercent >= 50 && (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= 2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hurricane"), 17f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloBalanceAOETargets && Settings.Current.SoloBalanceUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Starfire"),18f, (s, t) => t.HealthPercent == 100 && !t.IsTargetingMe, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Insect Swarm"), 19f, (s, t) => !t.HaveMyBuff("Insect Swarm") && (t.Health > 45 || t.IsBoss) && !Me.HaveBuff("Eclipse (Lunar)") && !Me.HaveBuff("Eclipse (Solar)"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Faerie Fire"), 20f, (s, t) => !t.HaveBuff("Faerie Fire") && BossList.isboss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Moonfire"), 21f, (s, t) => Settings.Current.SoloBalanceUseMoonfire && t.IsBoss && !t.HaveMyBuff("Moonfire") && t.HealthPercent > 35 || !Settings.Current.SoloBalanceUseMoonfire && !t.HaveMyBuff("Moonfire") && t.HealthPercent >= 60 && !Me.HaveBuff("Eclipse (Lunar)") && !Me.HaveBuff("Eclipse (Solar)"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Starfire"), 22f, (s, t) => t.HealthPercent >= 10 && !Me.HaveBuff("Eclipse (Solar)") && Me.HaveBuff("Nature's Grace") || Me.HaveBuff("Eclipse (Lunar)"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Wrath"), 23f, (s, t) => Me.HaveBuff("Eclipse (Solar)") && Me.ManaPercentage > 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Wrath"), 24f, (s, t) => !Me.HaveBuff("Eclipse (Solar)") && !Me.HaveBuff("Eclipse (Lunar)"), RotationCombatUtil.BotTarget, checkLoS: true),
        };


        private bool UseTranquility(IRotationAction s, WoWUnit t)
        {
            var nearbyFriendlies = RotationFramework.PartyMembers.Where(o => o.IsAlive && o.GetDistance <= 40).ToList();

            var under40 = nearbyFriendlies.Count(o => o.HealthPercent <= 40);
            var under55 = nearbyFriendlies.Count(o => o.HealthPercent <= 55);
            var under65 = nearbyFriendlies.Count(o => o.HealthPercent <= 65);

            return Me.IsInGroup && RotationFramework.PartyMembers.Count(u => u.IsAlive) >= 1 && (under40 >= 2 || under55 >= 3 || under65 >= 4);
        }
    }
}