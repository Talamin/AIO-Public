using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using wManager.Wow.Helpers;

namespace AIO.Combat.Warrior
{
    internal class Buffs : BaseRotation
    {
        private readonly BaseCombatClass CombatClass;
        private bool KnowStance;
        private string Spec => CombatClass.Specialisation;

        internal Buffs(BaseCombatClass combatClass) : base(runInCombat: true, runOutsideCombat: true) {
            CombatClass = combatClass;
            KnowStance = SpellManager.KnowSpell("Berserker Stance");
        }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Vigilance"), 1f, RotationCombatUtil.Always, RotationCombatUtil.FindHeal),
            new RotationStep(new RotationBuff("Battle Shout"), 2f, (s,t) => !t.HaveBuff("Greater Blessing of Might"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Defensive Stance"), 3f, (s,t) => Spec == "Protection", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Battle Stance"), 4f, (s,t) => Spec == "Arms" || (Spec == "Fury" && !KnowStance), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Berserker Stance"), 4f, (s,t) => Spec == "Fury", RotationCombatUtil.FindMe),
        };
    }
}
