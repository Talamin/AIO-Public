using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;
namespace AIO.Combat.Priest
{
    using Settings = PriestLevelSettings;
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Power Word: Fortitude"), 2.1f, (s,t) => !Me.HaveBuff("Power Word: Fortitude") && Me.ManaPercentage > 50, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Power Word: Shield"), 3f, (s,t) => Me.HealthPercent < 99 && !Me.HaveBuff("Power Word: Shield"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Lesser Heal"), 5f, (s,t) => !Me.HaveBuff("Lesser Heal") && Me.HealthPercent < 75, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Renew"), 7f, (s,t) =>  Me.HealthPercent < 90 && !Me.HaveBuff("Renew") , RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Flash Heal"), 9f, (s,t) => Me.HealthPercent < 60, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Shadow Word: Pain"), 11f, (s,t) => (Target.HealthPercent > Settings.Current.UseWandTresh || Me.ManaPercentage < 5) && !t.HaveMyBuff("Shadow Word: Pain"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Smite"), 12f, (s,t) => Target.HealthPercent > Settings.Current.UseWandTresh || Me.ManaPercentage < 5, RotationCombatUtil.BotTarget),
        };
    }
}
