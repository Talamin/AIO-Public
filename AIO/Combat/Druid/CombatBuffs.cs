using AIO.Combat.Addons;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    internal class CombatBuffs : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Tree of Life"), 5f,(s,t) => !Me.IsMounted, RotationCombatUtil.FindMe),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}
