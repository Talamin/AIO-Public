using AIO.Combat.Addons;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    internal class OOCBuffs : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Mark of the Wild"), 1f, (s,t) => !Me.IsMounted && !t.HaveBuff("Gift of the Wild") && !t.HaveBuff("Stamina") && !t.HaveBuff("Armor") && !t.HaveBuff("Agility") && !t.HaveBuff("Strength") && !t.HaveBuff("Spirit"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Mark of the Wild"), 2f, (s,t) => !Me.IsMounted && !t.HaveBuff("Gift of the Wild") && !t.HaveBuff("Stamina") && !t.HaveBuff("Armor") && !t.HaveBuff("Agility") && !t.HaveBuff("Strength") && !t.HaveBuff("Spirit"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Thorns"), 3f,(s,t) => !Me.IsMounted && !t.HaveBuff("Thorns"), RotationCombatUtil.FindTank),
            new RotationStep(new RotationBuff("Thorns"), 4f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}
