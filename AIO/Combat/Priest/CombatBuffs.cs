using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using wManager.Wow.Helpers;

namespace AIO.Combat.Priest
{
    internal class CombatBuffs : BaseRotation
    {
        internal CombatBuffs() : base(runInCombat: true, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Inner Fire", minimumStacks: 2), 7f, (s, t) => !t.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Shadow Form"), 8f, (s, t) => SpellManager.KnowSpell("Shadow Form") && !t.IsMounted, RotationCombatUtil.FindMe),
        };
    }
}