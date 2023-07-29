using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    internal class CombatBuffs : BaseRotation
    {
        internal CombatBuffs() : base(runInCombat: true, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Tree of Life"), 5f,(s,t) => !Me.IsMounted, RotationCombatUtil.FindMe),
        };
    }
}
