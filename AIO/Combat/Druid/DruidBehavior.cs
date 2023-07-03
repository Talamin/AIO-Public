using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
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
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"SoloFeral", new SoloFeral() },
                {"SoloBalance", new SoloBalance() },
                {"SoloRestoration", new SoloRestoration() },
                {"GroupFeralTank", new GroupFeralTank()},
                {"GroupRestorationHeal", new GroupRestorationHeal() },
                {"Default", new SoloFeral() },
            },
            new Buffs(),
            new AutoPartyResurrect("Revive"),
            new AutoPartyResurrect("Rebirth", true, Settings.Current.RebirthAuto))
        {
            SetDefaultRange();
            Addons.Add(new ConditionalCycleable(() => Settings.Current.HealOOC, new HealOOC()));
            Addons.Add(new ConditionalCycleable(() => Settings.Current.ChooseRotation == "GroupFeralTank", new RangedPull(new List<string> { "Faerie Fire (Feral)" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ALWAYS)));
        }

        public override void Initialize()
        {
            base.Initialize();
            switch (Specialisation)
            {
                case "SoloFeral":
                case "LowLevel":
                    DefaultRange = (SpellManager.KnowSpell("Growl") || SpellManager.KnowSpell("Cat Form")) ? 5.0f : 29.0f;
                    break;
                case "GroupFeralTank":
                    DefaultRange = 5.0f;
                    break;
                default:
                    DefaultRange = 29.0f;
                    break;
            }
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