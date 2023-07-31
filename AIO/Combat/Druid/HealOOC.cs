using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class HealOOC : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Rejuvenation"), 1f, (s,t) => Me.HealthPercent <= Settings.Current.OOCRejuvenation && Me.ManaPercentage > 15 && !Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Regrowth"), 2f, (s,t) =>  Me.HealthPercent <= Settings.Current.OOCRegrowth && Me.ManaPercentage > 15 && !Me.HaveBuff("Regrowth") && Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Rejuvenation"), 3f, (s,t) => t.HealthPercent <= Settings.Current.OOCRejuvenation && Me.ManaPercentage > 15 && !t.HaveBuff("Rejuvenation"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Regrowth"), 4f, (s,t) =>  t.HealthPercent <= Settings.Current.OOCRegrowth && Me.ManaPercentage > 15 && !t.HaveBuff("Regrowth") && t.HaveBuff("Rejuvenation"), RotationCombatUtil.FindPartyMember),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}
