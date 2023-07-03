using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class Protection : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Lay on Hands"), 1.1f, (s,t) => t.HealthPercent <= Settings.Current.ProtectionLoH && !Me.HaveBuff("Forbearance"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Sacred Shield"), 1.5f, (s,t) => !Me.HaveBuff("Sacred Shield"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Consecration"), 2f, (s,t) => t.HealthPercent > 25 && RotationFramework.Enemies.Count(o => o.GetDistance <=15) >= Settings.Current.GroupProtConsecration, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Divine Plea"), 2.5f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaIC, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Righteous Defense"), 3f, (s,t) => t.Name != Me.Name && RotationFramework.Enemies.Count(o => o.IsAttackable && !o.IsTargetingMe && o.IsTargetingPartyMember) >=2,RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Hand of Reckoning"), 4f, (s,t) => t.GetDistance <= 25 && !t.IsTargetingMe && !Me.IsInGroup && Settings.Current.RetributionHOR, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Hand of Reckoning"), 4.5f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsAttackable && !o.IsTargetingMe && o.IsTargetingPartyMember) >= 1,RotationCombatUtil.FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell("Cleanse"), 4.6f, (s,t) => Settings.Current.ProtectionCleanse == "Group" && t.HasDebuffType("Poison","Disease","Magic"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Cleanse"), 4.7f, (s,t) => Settings.Current.ProtectionCleanse == "Me" && t.HasDebuffType("Poison","Disease","Magic"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Divine Plea"), 5f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hand of Freedom"), 5.5f, (s, t) => Me.Rooted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Holy Light"), 6f, (s,t) => !Me.IsInGroup && Me.HealthPercent <= 50 && Settings.Current.ProtectionHolyLight, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Divine Protection"), 7f, (s,t) => Settings.Current.DivineProtection && (RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.GetDistance <=15) >= 3 || BossList.isboss), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hammer of Justice"), 8f, (s,t) => t.HealthPercent >= 75 && RotationCombatUtil.EnemyAttackingCountCluster(20) >= 2 && Settings.Current.ProtectionHammerofJustice, RotationCombatUtil.FindEnemy),
            new RotationStep(new RotationSpell("Avenger's Shield"), 9f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            //new RotationStep(new RotationSpell("Hand of Salvation"), 7f, (s,t) =>  RotationFramework.AllUnits.Count(o => o.IsAttackable && !o.IsTargetingMe && o.IsTargetingPartyMember && !TraceLine.TraceLineGo(Me.Position, o.Position)) >=2, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Hand of Salvation"), 9.1f, (s,t) => t.InCombatFlagOnly && t.HealthPercent < 99, RotationCombatUtil.FindHeal),
            new RotationStep(new RotationSpell("Hand of Protection"), 9.2f, (s,t) => Settings.Current.ProtectionHoP && t.HealthPercent < 75 && (t.WowClass == WoWClass.Mage || t.WowClass == WoWClass.Warlock || t.WowClass == WoWClass.Priest || t.WowClass == WoWClass.Druid), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Avenger's Shield"), 10f, (s,t) => t.GetDistance <= 25 && Me.ManaPercentage > 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Avenging Wrath"), 11f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=20) >=3 && Settings.Current.AvengingWrathProtection,RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Judgement of Light"), 12f, (s,t) => !SpellManager.KnowSpell("Judgement of Wisdom"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Judgement of Wisdom"), 13f,(s,t) => !t.HaveBuff("Judgement of Wisdom"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Judgement of Light"), 13.1f,(s,t) => t.HaveBuff("Judgement of Wisdom"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Hammer of Wrath"), 14f, (s,t) => t.HealthPercent < 20 && Me.ManaPercentage > 50 , RotationCombatUtil.FindEnemy),
            new RotationStep(new RotationSpell("Hammer of the Righteous"), 16f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shield of Righteousness"), 17f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Holy Shield"), 18f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
        };
    }
}
