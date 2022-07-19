using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    internal class Buffs : BaseRotation
    {
        internal Buffs() : base(runInCombat: true, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Unending Breath"), 1f, RotationCombatUtil.Always, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Unending Breath"), 2f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Fel Armor"), 3f, RotationCombatUtil.Always, RotationCombatUtil.FindMe, Exclusive.WarlockSkin),
            new RotationStep(new RotationBuff("Demon Armor"), 4f, RotationCombatUtil.Always, RotationCombatUtil.FindMe, Exclusive.WarlockSkin),
            new RotationStep(new RotationBuff("Demon Skin"), 5f, RotationCombatUtil.Always, RotationCombatUtil.FindMe, Exclusive.WarlockSkin),
            new RotationStep(new RotationBuff("Soul Link"), 6f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Lifetap"), 7f, (s,t) => !Me.IsResting(), RotationCombatUtil.FindMe)
        };
    }
}
