using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Holy Light"), 2f, (s,t) => !Me.IsInGroup && Me.HealthPercent < 50, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Holy Light"), 3f, (s,t) => t.HealthPercent <50 , RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Seal of Righteousness"), 4f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Blessing of Might"), 5f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Devotion Aura"), 6f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Judgement of Light"), 7f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
        };
    }
}