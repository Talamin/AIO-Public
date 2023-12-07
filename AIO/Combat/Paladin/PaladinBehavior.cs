using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using wManager.Events;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;

    internal class PaladinBehavior : BaseCombatClass
    {
        private static readonly string _crusaderAuraName = "Crusader Aura";
        private float CombatRange;
        private float DefaultRange;
        public override float Range => CombatRange;
        private void SetDefaultRange() => CombatRange = DefaultRange;
        private void SetRange(float range) => CombatRange = range;

        internal PaladinBehavior() : base(
            Settings.Current,
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.LowLevel, new LowLevel() },
                { Spec.Paladin_GroupHoly, new GroupHoly() },
                { Spec.Paladin_SoloProtection, new SoloProtection() },
                { Spec.Paladin_GroupProtection, new GroupProtection() },
                { Spec.Paladin_SoloRetribution, new SoloRetribution() },
                { Spec.Paladin_GroupRetribution, new GroupRetribution() },
                { Spec.Fallback, new SoloRetribution() },
            })
        {
            Addons.Add(new Racials());
            if (Settings.Current.Resurrect)
                Addons.Add(new AutoPartyResurrect("Redemption"));
            if (Settings.Current.HealOOC)
                Addons.Add(new HealOOC(Settings.Current));
            if (Settings.Current.Buffing)
            {
                Addons.Add(new Blessings(this));
                Addons.Add(new NewBuffs(this));
            }

            switch (Specialisation)
            {
                case Spec.Paladin_GroupProtection:
                    Addons.Add(new RangedPull(SetDefaultRange, SetRange, RangedPull.PullCondition.ALWAYS));
                    break;
                case Spec.Paladin_SoloProtection:
                case Spec.Paladin_SoloRetribution:
                    Addons.Add(new RangedPull(SetDefaultRange, SetRange, RangedPull.PullCondition.ENEMIES_AROUND));
                    break;
            }
        }

        public override void Initialize()
        {
            switch (Specialisation)
            {
                case Spec.Paladin_GroupHoly:
                    DefaultRange = 29.0f;
                    break;
                default:
                    DefaultRange = 5.0f;
                    break;
            }
            SetRange(DefaultRange);
            ObjectManagerEvents.OnObjectManagerPulsed += OnObjectManagerPulse;
            base.Initialize();
        }

        public override void Dispose()
        {
            ObjectManagerEvents.OnObjectManagerPulsed -= OnObjectManagerPulse;
            base.Dispose();
        }

        private void OnObjectManagerPulse()
        {
            if (!Settings.Current.Crusader)
                return;
            if (ObjectManager.Me.IsMounted && SpellManager.KnowSpell(_crusaderAuraName) && !ObjectManager.Me.HaveBuff(_crusaderAuraName))
            {
                SpellManager.CastSpellByNameLUA(_crusaderAuraName);
            }
        }
    }
}
