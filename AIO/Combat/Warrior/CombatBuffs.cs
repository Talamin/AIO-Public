using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    internal class CombatBuffs : IAddon
    {
        private readonly BaseCombatClass CombatClass;
        private bool KnowBerserkerStance;
        private Spec Spec => CombatClass.Specialisation;

        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;

        internal CombatBuffs(BaseCombatClass combatClass)
        {
            CombatClass = combatClass;
            KnowBerserkerStance = SpellManager.KnowSpell("Berserker Stance");
        }

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Vigilance"), 1f,(s,t) => !Me.IsMounted && !t.HaveBuff("Vigilance"), RotationCombatUtil.FindHeal),
            new RotationStep(new RotationBuff("Battle Shout"), 2f, (s,t) => !Me.IsMounted && !t.HaveBuff("Greater Blessing of Might"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Defensive Stance"), 3f, (s,t) => !Me.IsMounted && Spec == Spec.Warrior_GroupProtection, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Battle Stance"), 4f, (s,t) => !Me.IsMounted && Spec == Spec.Warrior_SoloArms, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Berserker Stance"), 4f, (s,t) => !Me.IsMounted && (Spec == Spec.Warrior_SoloFury || Spec == Spec.Warrior_GroupFury), RotationCombatUtil.FindMe),
            // Fallback for fury
            new RotationStep(new RotationBuff("Battle Stance"), 4f, (s,t) => !Me.IsMounted && (Spec == Spec.Warrior_SoloFury|| Spec == Spec.Warrior_GroupFury) && !KnowBerserkerStance, RotationCombatUtil.FindMe),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}
