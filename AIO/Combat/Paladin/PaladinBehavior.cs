using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Settings;
using System.Collections.Generic;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class PaladinBehavior : BaseCombatClass
    {
        public override float Range => 5.0f;

        internal PaladinBehavior() : base(
            Settings.Current,
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"Holy", new Holy() },
                {"Protection", new Protection() },
                {"Retribution", new Retribution() },
                {"Default", new Retribution() },
            },
            new AutoPartyResurrect("Redemption"),
            new ConditionalCycleable(() => Settings.Current.HealOOC, new HealOOC()))
        {
            Addons.Add(new ConditionalCycleable(() => Settings.Current.Buffing, new Buffs(this)));
        }
    }
}
