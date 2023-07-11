using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;

    internal class PaladinBehavior : BaseCombatClass
    {
        private static readonly string crusader = "Crusader Aura";
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
                { Spec.Default, new SoloRetribution() },
            },
            new ConditionalCycleable(() => Settings.Current.Resurrect, new AutoPartyResurrect("Redemption")),
            new ConditionalCycleable(() => Settings.Current.HealOOC, new HealOOC(Settings.Current)))
        {
            //Addons.Add(new ConditionalCycleable(() => Settings.Current.Buffing, new Buffs(this)));
            Addons.Add(new ConditionalCycleable(() => Settings.Current.Buffing, new Blessings(this)));
            Addons.Add(new ConditionalCycleable(() => Settings.Current.Buffing, new NewBuffs(this)));
            Addons.Add(new ConditionalCycleable(() => Specialisation == Spec.Paladin_GroupProtection, new RangedPull(new List<string> { "Avenger's Shield", "Exorcism", "Hand of Reckoning" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ALWAYS)));
        }

        public override void Initialize()
        {
            base.Initialize();

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
        }

        protected override void OnObjectManagerPulse()
        {
            if (!Settings.Current.Crusader)
                return;
            if (SpellManager.KnowSpell(crusader) && ObjectManager.Me.IsMounted && !ObjectManager.Me.HaveBuff(crusader))
            {
                SpellManager.CastSpellByNameLUA(crusader);
            }
        }
    }
}
