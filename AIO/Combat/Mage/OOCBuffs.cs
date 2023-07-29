using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    internal class OOCBuffs : BaseRotation
    {
        internal OOCBuffs() : base(runInCombat: false, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Arcane Intellect"), 5f, (s,t) => !Me.IsMounted && !t.HaveBuff("Fel Intelligence") && !t.HaveBuff("Arcane Brilliance"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Arcane Intellect"), 6f, (s,t) => !Me.IsMounted && !t.HaveBuff("Fel Intelligence") && !t.HaveBuff("Arcane Brilliance"), RotationCombatUtil.FindMe),
        };
    }
}


