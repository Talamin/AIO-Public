using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Shaman
{
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Healing Wave"), 2f, (s,t) => Me.HealthPercent < 40 && t.HealthPercent > 10, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Lightning Bolt"), 3f, (s,t) => Me.ManaPercentage >= 20 && !Me.InCombatFlagOnly && t.HealthPercent == 100, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Lightning Bolt"), 4f, (s,t) => Me.ManaPercentage >= 50 && t.GetDistance > 7, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Earth Shock"), 5f, (s,t) =>  Me.ManaPercentage >= 20 && !t.HaveMyBuff("Earth Shock"), RotationCombatUtil.BotTarget),
        };
    }
}