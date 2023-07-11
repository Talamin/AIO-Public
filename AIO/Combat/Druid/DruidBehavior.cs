using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using System.ComponentModel;
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
        private float CombatRange;
        private float DefaultRange;
        public override float Range => CombatRange;
        private void SetDefaultRange() => CombatRange = DefaultRange;
        private void SetRange(float range) => CombatRange = range;

        internal DruidBehavior() : base(
            Settings.Current,
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.LowLevel, new LowLevel() },
                { Spec.Druid_SoloFeral, new SoloFeral() },
                { Spec.Druid_SoloBalance, new SoloBalance() },
                { Spec.Druid_SoloRestoration, new SoloRestoration() },
                { Spec.Druid_GroupFeralTank, new GroupFeralTank()},
                { Spec.Druid_GroupRestoration, new GroupRestoration() },
                { Spec.Default, new SoloFeral() },
            },
            new Buffs(),
            new AutoPartyResurrect("Revive"),
            new AutoPartyResurrect("Rebirth", true, Settings.Current.RebirthAuto))
        {
            Addons.Add(new ConditionalCycleable(() => Settings.Current.HealOOC, new HealOOC()));
            Addons.Add(new ConditionalCycleable(() => Specialisation == Spec.Druid_GroupFeralTank, new RangedPull(new List<string> { "Faerie Fire (Feral)" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ALWAYS)));
        }

        public override void Initialize()
        {
            base.Initialize();
            switch (Specialisation)
            {
                case Spec.Druid_SoloFeral:
                case Spec.LowLevel:
                    DefaultRange = (SpellManager.KnowSpell("Growl") || SpellManager.KnowSpell("Cat Form")) ? 5.0f : 29.0f;
                    break;
                case Spec.Druid_GroupFeralTank:
                    DefaultRange = 5.0f;
                    break;
                default:
                    DefaultRange = 29.0f;
                    break;
            }
            SetRange(DefaultRange);
        }

        protected override void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
        {
            HealingTouchValue = RotationSpell.GetSpellCost("Healing Touch");
            RejuvenationValue = RotationSpell.GetSpellCost("Rejuvenation");
            RegrowthValue = RotationSpell.GetSpellCost("Regrowth");
            TransformValue = RotationSpell.GetSpellCost("Bear Form");
        }
    }
}