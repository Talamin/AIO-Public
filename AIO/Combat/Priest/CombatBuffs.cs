using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Helpers.Caching;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Priest
{
    internal class CombatBuffs : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Inner Fire", minimumStacks: 2), 1f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Shadowform"), 2f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Vampiric Embrace"), 7f, (s, t) => !Me.IsMounted && !Me.HaveBuff("Vampiric Embrace"), RotationCombatUtil.FindMe),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}