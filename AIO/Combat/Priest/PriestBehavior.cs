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
                { Spec.Priest_GroupDiscipline, new GroupDiscipline() },
                { Spec.Priest_SoloShadow, new SoloShadow() },
                { Spec.Priest_GroupShadow, new GroupShadow() },
                { Spec.Fallback, new SoloShadow() },
            })
        {
            Addons.Add(new Racials());
            Addons.Add(new AutoPartyResurrect("Resurrection"));
            Addons.Add(new OOCBuffs());
            Addons.Add(new CombatBuffs());

            switch (Specialisation)
            {
                case Spec.Priest_GroupHoly:
                case Spec.Priest_SoloShadow:
                    Addons.Add(new SlowLuaCaching());
                    break;
            }
        }
    }
}
