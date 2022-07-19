using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using static AIO.Constants;

namespace AIO.Combat.Rogue
{
    using Settings = RogueLevelSettings;
    internal class Combat : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Sprint"), 2f, (s,t) => t.GetDistance >= 15 && !Settings.Current.PullRanged, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Kick"), 3f, (s,t) => t.IsCasting() && t.GetDistance < 7, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell("Evasion"), 3.1f, (s, t) => RotationFramework.Enemies.Count(o => o.GetDistance <=10 && o.IsTargetingMe) >=Settings.Current.Evasion || (Me.HealthPercent <= 30 && t.HealthPercent >70), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Evasion"), 3.2f, (s, t) => !Me.IsInGroup && Target.IsElite, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Riposte"), 4f, (s, t) => !Me.HaveBuff("Stealth"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blade Flurry"), 2f, (s,t) =>t.HealthPercent> 70 && !Me.HaveBuff("Stealth") && (RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=Settings.Current.BladeFLurry), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Adrenaline Rush"), 6f, (s,t) =>!Me.HaveBuff("Stealth") && RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=Settings.Current.AdrenalineRush, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Adrenaline Rush"), 6.1f, (s,t) =>!Me.HaveBuff("Stealth") && Target.IsElite && !Me.IsInGroup, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Slice and Dice"), 7f, (s, t) => !Me.HaveBuff("Slice and Dice") && Me.ComboPoint >= 1 && t.HealthPercent > 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Eviscerate"), 8f, (s, t) =>!Me.HaveBuff("Stealth") && Me.ComboPoint >= Settings.Current.Eviscarate, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Killing Spree"), 9f, (s, t) =>!Me.HaveBuff("Adrenaline Rush") && !Me.HaveBuff("Blade Flurry") && !Me.HaveBuff("Stealth") && RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=Settings.Current.KillingSpree, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Sinister Strike"), 10f, (s, t) =>!Me.HaveBuff("Stealth"), RotationCombatUtil.BotTarget),
        };
    }
}
