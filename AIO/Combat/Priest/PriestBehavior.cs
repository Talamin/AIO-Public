using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Settings;
using System.Collections.Generic;

namespace AIO.Combat.Priest
{
    using Settings = PriestLevelSettings;
    internal class PriestBehavior : BaseCombatClass
    {
        public override float Range => 29.0f;

        internal PriestBehavior() : base(
            Settings.Current,
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"Holy", new Holy() },
                {"Shadow", new Shadow() },
                {"Default", new Shadow() },
            } , new AutoPartyResurrect("Resurrection"),
            new Buffs())
        {
            Addons.Add(new ConditionalCycleable(() => Specialisation == "Holy" || Specialisation == "Shadow", new SlowLuaCaching()));
        }
    }
}
