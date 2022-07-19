using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Priest
{
    using Settings = PriestLevelSettings;
    internal class Buffs : BaseRotation
    {
        internal Buffs() : base(runInCombat: true, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Prayer of Fortitude"),   1f, (s,t) =>  CanPrayFort() && NeedsFort(s,t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Prayer of Spirit"),      2f, (s,t) =>  CanPraySpirit() && NeedsSpirit(s,t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Prayer of Shadow Protection"), 3f, (s,t) =>  CanPrayShadow() && NeedsShadow(s,t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Power Word: Fortitude"), 4f, (s,t) =>  CanFort() && NeedsFort(s,t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Divine Spirit"),         5f, (s,t) =>  CanSpirit() && NeedsSpirit(s,t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Shadow Protection"),     6f, (s,t) =>  CanShadow() && NeedsShadow(s,t), RotationCombatUtil.FindPartyMember),

            new RotationStep(new RotationBuff("Inner Fire", minimumStacks: 2), 7f, (s, t) => !t.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Shadow Form"), 8f, (s, t) => SpellManager.KnowSpell("Shadow Form") && !t.IsMounted, RotationCombatUtil.FindMe),
        };
        private bool HasCandle() => ItemsManager.HasItemById(17029) || ItemsManager.HasItemById(17028); // Sacred Candle and Holy Candle
        private bool CanPrayFort() => !Me.IsMounted && SpellManager.KnowSpell("Prayer of Fortitude") && HasCandle();
        private bool CanPraySpirit() => !Me.IsMounted && SpellManager.KnowSpell("Prayer of Spirit") && HasCandle();
        private bool CanPrayShadow() => !Me.IsMounted && SpellManager.KnowSpell("Prayer of Shadow Protection") && HasCandle();

        private bool CanFort() => !Me.IsMounted && SpellManager.KnowSpell("Power Word: Fortitude");
        private bool CanSpirit() => !Me.IsMounted && SpellManager.KnowSpell("Divine Spirit");
        private bool CanShadow() => !Me.IsMounted && SpellManager.KnowSpell("Shadow Protection");

        private bool NeedsFort(IRotationAction action, WoWUnit target) => !target.HaveBuff("Power Word: Fortitude") && !target.HaveBuff("Prayer of Fortitude");
        private bool NeedsSpirit(IRotationAction action, WoWUnit target) => !target.HaveBuff("Divine Spirit") && !target.HaveBuff("Prayer of Spirit");
        private bool NeedsShadow(IRotationAction action, WoWUnit target) => !target.HaveBuff("Shadow Protection") && !target.HaveBuff("Prayer of Shadow Protection");

    }
}