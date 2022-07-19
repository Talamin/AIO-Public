using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class Survival : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Hunter's Mark"), 6f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff("Hunter's Mark") && t.IsAlive && t.GetDistance >= 5 && t.HealthPercent > 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Kill Command"), 6.1f, (s,t) => !Me.HaveBuff("Kill Command"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Serpent Sting"), 7f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff("Serpent Sting") , RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Black Arrow"), 8f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Explosive Shot"), 9f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Aimed Shot"), 10f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Steady Shot"), 11f, (s,t) => t.GetDistance >= 5 , RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Raptor Strike"), 12f, (s,t) => t.GetDistance < 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Disengage"), 13f, (s,t) => t.GetDistance < 5 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.Dis, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Feign Death"), 14f, (s,t) => t.GetDistance < 5 && Me.HealthPercent < 50 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.FD, RotationCombatUtil.BotTarget),
        };
    }
}
