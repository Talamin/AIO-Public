using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class HealOOC : BaseRotation
    {
        private readonly Settings _settings;
        internal HealOOC(Settings settings) : base(runInCombat: false, runOutsideCombat: true) => _settings = settings;

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Holy Light"), 3f, (s,t) => t.HealthPercent < 60 && _settings.ChooseRotation == nameof(Spec.Paladin_GroupHoly), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Holy Light"), 4f, (s,t) => Me.HealthPercent < 60, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Divine Plea"), 5f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaOOC, RotationCombatUtil.FindMe),
        };
    }
}
