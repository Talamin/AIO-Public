using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using System.ComponentModel;
using wManager.Events;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class DruidBehavior : BaseCombatClass
    {
        public static int HealingTouchValue = 0;
        public static int RegrowthValue = 0;
        public static int RejuvenationValue = 0;
        public static int TransformValue = 0;
        private float _combatRange;
        private float _defaultRange;
        public override float Range => _combatRange;
        private void SetDefaultRange() => _combatRange = _defaultRange;
        private void SetRange(float range) => _combatRange = range;

        internal DruidBehavior() : base(
            Settings.Current,
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.LowLevel, new LowLevel() },
                { Spec.Druid_SoloFeral, new SoloFeral() },
                { Spec.Druid_SoloBalance, new SoloBalance() },
                { Spec.Druid_GroupFeralTank, new GroupFeralTank()},
                { Spec.Druid_GroupRestoration, new GroupRestoration() },
                { Spec.Fallback, new SoloFeral() },
            })
        {
            Addons.Add(new Racials());
            Addons.Add(new OOCBuffs());
            Addons.Add(new CombatBuffs());
            if (Settings.Current.ReviveAuto)
                Addons.Add(new AutoPartyResurrect("Revive"));
            if (Settings.Current.RebirthAuto)
                Addons.Add(new AutoPartyResurrect("Rebirth", true, false));
            if (Settings.Current.HealOOC) 
                Addons.Add(new HealOOC());
            if (Specialisation == Spec.Druid_GroupFeralTank) 
                Addons.Add(new RangedPull(new List<string> { "Faerie Fire (Feral)" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ALWAYS));
        }

        public override void Initialize()
        {
            switch (Specialisation)
            {
                case Spec.Druid_SoloFeral:
                case Spec.LowLevel:
                    _defaultRange = (SpellManager.KnowSpell("Growl") || SpellManager.KnowSpell("Cat Form")) ? 5.0f : 29.0f;
                    break;
                case Spec.Druid_GroupFeralTank:
                    _defaultRange = 5.0f;
                    break;
                default:
                    _defaultRange = 29.0f;
                    break;
            }
            SetRange(_defaultRange);
            FightEvents.OnFightStart += OnFightStart;
            base.Initialize();
        }

        public override void Dispose()
        {
            FightEvents.OnFightStart -= OnFightStart;
            base.Dispose();
        }

        private void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
        {
            HealingTouchValue = RotationSpell.GetSpellCost("Healing Touch");
            RejuvenationValue = RotationSpell.GetSpellCost("Rejuvenation");
            RegrowthValue = RotationSpell.GetSpellCost("Regrowth");
            TransformValue = RotationSpell.GetSpellCost("Bear Form");
        }
    }
}