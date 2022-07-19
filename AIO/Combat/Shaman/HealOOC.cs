using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Shaman
{
    using Settings = ShamanLevelSettings;
    internal class HealOOC : BaseRotation
    {
        internal HealOOC() : base(runInCombat: false, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Riptide"), 1f, (s,t) => Settings.Current.HealOOC &&  t.HealthPercent < 95, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Healing Wave"), 2f, (s,t) =>Settings.Current.HealOOC &&  t.HealthPercent < 70, RotationCombatUtil.FindPartyMember),
        };
    }
}
