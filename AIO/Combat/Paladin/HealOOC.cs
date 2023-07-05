using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class HealOOC : BaseRotation
    {
        internal HealOOC() : base(runInCombat: false, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Holy Light"), 3f, (s,t) => t.HealthPercent < 60, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Holy Light"), 4f, (s,t) => Me.HealthPercent < 60, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Divine Plea"), 5f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaOOC, RotationCombatUtil.FindMe),
        };
    }
}
