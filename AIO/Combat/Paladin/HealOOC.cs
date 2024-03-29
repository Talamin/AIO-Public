using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class HealOOC : IAddon
    {
        private readonly Settings _settings;

        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        internal HealOOC(Settings settings)
        {
            _settings = settings;
        }

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationAction("Cache debuffed party members", RotationCombatUtil.CacheLUADebuffedPartyMembersStep), 0f, 1000),
            new RotationStep(new RotationSpell("Holy Light"), 3f, (s,t) => t.HealthPercent < 60 && _settings.ChooseRotation == nameof(Spec.Paladin_GroupHoly), RotationCombatUtil.FindPartyMember, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Flash of Light"), 3f, (s,t) => t.HealthPercent < 85 && _settings.ChooseRotation == nameof(Spec.Paladin_GroupHoly), RotationCombatUtil.FindPartyMember, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Holy Light"), 4f, (s,t) => Me.HealthPercent < 60, RotationCombatUtil.FindMe, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Divine Plea"), 5f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaOOC, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Purify"), 6f, (s,t) => true, p => RotationCombatUtil.GetPartyMemberWithCachedDebuff(p, new List<DebuffType>() { DebuffType.Disease, DebuffType.Poison } , true, 30)),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}
