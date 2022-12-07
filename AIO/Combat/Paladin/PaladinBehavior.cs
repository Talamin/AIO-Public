using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Settings;
using System.Collections.Generic;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class PaladinBehavior : BaseCombatClass
    {
        private float CombatRange;
        public override float Range => CombatRange;

        internal PaladinBehavior() : base(
            Settings.Current,
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"Holy", new Holy() },
                {"Protection", new Protection() },
                {"GroupProtectionTank", new GroupProtectionTank() },
                {"Retribution", new Retribution() },
                {"Default", new Retribution() },
            },
            new ConditionalCycleable(() => Settings.Current.Resurrect, new AutoPartyResurrect("Redemption")),
            new ConditionalCycleable(() => Settings.Current.HealOOC, new HealOOC()))
        {
            Addons.Add(new ConditionalCycleable(() => Settings.Current.Buffing, new Buffs(this)));
        }
        public override void Initialize()
        {
            base.Initialize();

            switch (Specialisation)
            {
                case "Holy":
                    CombatRange = 29.0f;
                    break;
                default:
                    CombatRange = 5.0f;
                    break;
            }
        }
    }
}
