using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    internal class SoloArms : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Execute"), 2f, (s, t) => Me.HaveBuff("Sudden Death"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Rend"), 3f, (s, t) => !t.HaveMyBuff("Rend") && !t.IsCreatureType("Elemental"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Overpower"), 4f, (s, t) => Me.HaveBuff("Taste for Blood"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Mortal Strike"), 5f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Heroic Strike"), 6f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget)
        };
    }
}
