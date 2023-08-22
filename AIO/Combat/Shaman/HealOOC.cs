using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;

namespace AIO.Combat.Shaman
{
    using Settings = ShamanLevelSettings;
    internal class HealOOC : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Riptide"), 1f, (s,t) => Settings.Current.HealOOC &&  t.HealthPercent < 95, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Healing Wave"), 2f, (s,t) =>Settings.Current.HealOOC &&  t.HealthPercent < 70, RotationCombatUtil.FindPartyMember, preventDoubleCast: true),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}
