using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class HealOOC : BaseRotation
    {
        private readonly BaseCombatClass CombatClass;
        private string Spec => CombatClass.Specialisation;
        internal HealOOC(BaseCombatClass combatClass) : base(runInCombat: false, runOutsideCombat: true) => CombatClass = combatClass;

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Rejuvenation"), 1f, (s,t) => Spec =="FeralCombat" && Me.HealthPercent <= Settings.Current.FeralRejuvenation && Me.ManaPercentage > 15 && !Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Regrowth"), 2f, (s,t) => Spec =="FeralCombat" && Me.HealthPercent <= Settings.Current.FeralRegrowth && Me.ManaPercentage > 15 && !Me.HaveBuff("Regrowth") && Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Rejuvenation"), 3f, (s,t) => Spec =="Balance" && Me.HealthPercent <= Settings.Current.BalanceRejuvenation && Me.ManaPercentage > 15 && !Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Regrowth"), 4f, (s,t) => Spec =="Balance" && Me.HealthPercent <= Settings.Current.BalanceRegrowth && Me.ManaPercentage > 15 && !Me.HaveBuff("Regrowth") && Me.HaveBuff("Rejuvenation"), RotationCombatUtil.FindPartyMember),
        };
    }
}
