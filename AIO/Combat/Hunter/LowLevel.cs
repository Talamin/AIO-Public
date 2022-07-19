using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Attack"), 1f, (s,t) => Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Hunter's Mark"), 2f, (s,t) => !t.HaveMyBuff("Hunter's Mark") && t.IsAlive && t.GetDistance >= 5 && t.HealthPercent > 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Serpent Sting"), 3f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff("Serpent Sting"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Arcane Shot"), 4f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Raptor Strike"), 5f, (s,t) => t.GetDistance < 5, RotationCombatUtil.BotTarget),
        };
    }
}
