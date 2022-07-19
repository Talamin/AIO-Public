using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Healing Touch"), 2f, (s, t) => Me.HealthPercent <= 30, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Starfire"), 3f, (s, t) => t.HealthPercent == 100 && !t.IsTargetingMe, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Moonfire"), 4f, (s, t) => !t.HaveMyBuff("Moonfire"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Wrath"), 5f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
        };
    }
}
