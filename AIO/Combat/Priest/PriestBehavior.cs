using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Lists;
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
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.LowLevel, new LowLevel() },
                { Spec.Priest_GroupHoly, new GroupHoly() },
                { Spec.Priest_SoloShadow, new SoloShadow() },
                { Spec.Default, new SoloShadow() },
            } , 
            new AutoPartyResurrect("Resurrection"),
            new Buffs())
        {
            Addons.Add(new ConditionalCycleable(() => Specialisation == Spec.Priest_GroupHoly || Specialisation == Spec.Priest_SoloShadow, new SlowLuaCaching()));
        }
    }
}
