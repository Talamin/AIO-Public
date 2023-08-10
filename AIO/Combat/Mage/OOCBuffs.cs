using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Helpers.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    internal class OOCBuffs : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        private readonly List<WoWClass> _classesForFocusMagic = new List<WoWClass>()
        {
            WoWClass.Mage,
            WoWClass.Warlock,
            WoWClass.Paladin,
            WoWClass.Shaman,
            WoWClass.Priest,
            WoWClass.Druid
        };

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Arcane Intellect"), 5f, (s,t) => !Me.IsMounted && !t.HaveBuff("Fel Intelligence") && !t.HaveBuff("Arcane Brilliance"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Arcane Intellect"), 6f, (s,t) => !Me.IsMounted && !t.HaveBuff("Fel Intelligence") && !t.HaveBuff("Arcane Brilliance"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Focus Magic"), 7f, (s,t) => !Me.IsMounted, FindPartyMemberForFocusMagic),
        };

        private WoWPlayer FindPartyMemberForFocusMagic(Func<WoWUnit, bool> predicate)
        {
            if (RotationFramework.PartyMembers.Any(m => m.CHaveMyBuff("Focus Magic")))
                return null;

            foreach (WoWClass cls in _classesForFocusMagic)
            {
                List<WoWPlayer> potentialMembers = RotationFramework.PartyMembers
                    .Where(m => m.WowClass == cls)
                    .ToList();
                if (potentialMembers.Count > 0)
                    return potentialMembers.FirstOrDefault(m => m.GetDistance < 30 && m.IsAlive);
            }

            return null;
        }

        public void Initialize() { }
        public void Dispose() { }
    }
}


