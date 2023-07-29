using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    internal class CombatBuffs : BaseRotation
    {
        private readonly BaseCombatClass CombatClass;
        private bool KnowBerserkerStance;
        private Spec Spec => CombatClass.Specialisation;

        internal CombatBuffs(BaseCombatClass combatClass) : base(runInCombat: true, runOutsideCombat: true) {
            CombatClass = combatClass;
            KnowBerserkerStance = SpellManager.KnowSpell("Berserker Stance");
        }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Vigilance"), 1f,(s,t) => !Me.IsMounted && !t.HaveBuff("Vigilance"), RotationCombatUtil.FindHeal),
            new RotationStep(new RotationBuff("Battle Shout"), 2f, (s,t) => !Me.IsMounted && !t.HaveBuff("Greater Blessing of Might"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Defensive Stance"), 3f, (s,t) => !Me.IsMounted && Spec == Spec.Warrior_GroupProtection, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Battle Stance"), 4f, (s,t) => !Me.IsMounted && Spec == Spec.Warrior_SoloArms, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Berserker Stance"), 4f, (s,t) => !Me.IsMounted && (Spec == Spec.Warrior_SoloFury || Spec == Spec.Warrior_GroupFury), RotationCombatUtil.FindMe),
            // Fallback for fury
            new RotationStep(new RotationBuff("Battle Stance"), 4f, (s,t) => !Me.IsMounted && (Spec == Spec.Warrior_SoloFury|| Spec == Spec.Warrior_GroupFury) && !KnowBerserkerStance, RotationCombatUtil.FindMe),
        };
    }
}
