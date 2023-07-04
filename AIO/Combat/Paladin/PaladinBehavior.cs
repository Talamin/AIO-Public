using AIO.Combat.Addons;
using AIO.Combat.Common;
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
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"Holy", new Holy() },
                {"GroupHolyHeal", new GroupHolyHeal() },
                {"Protection", new Protection() },
                {"GroupProtectionTank", new GroupProtectionTank() },
                {"Retribution", new Retribution() },
                {"Default", new Retribution() },
            },
            new ConditionalCycleable(() => Settings.Current.Resurrect, new AutoPartyResurrect("Redemption")),
            new ConditionalCycleable(() => Settings.Current.HealOOC, new HealOOC()))
        {
            //Addons.Add(new ConditionalCycleable(() => Settings.Current.Buffing, new Buffs(this)));
            Addons.Add(new ConditionalCycleable(() => Settings.Current.Buffing, new Blessings(this)));
            Addons.Add(new ConditionalCycleable(() => Settings.Current.Buffing, new NewBuffs(this)));
            Addons.Add(new ConditionalCycleable(() => Settings.Current.ChooseRotation == "GroupProtectionTank", new RangedPull(new List<string> { "Avenger's Shield", "Exorcism", "Hand of Reckoning" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ALWAYS)));
        }

        public override void Initialize()
        {
            base.Initialize();

            switch (Specialisation)
            {
                case "Holy":
                case "GroupHolyHeal":
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
