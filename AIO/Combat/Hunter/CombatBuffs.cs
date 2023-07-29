using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class CombatBuffs : BaseRotation
    {
        internal CombatBuffs() : base(runInCombat: true, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Aspect of the Viper"), 1f, (s, t) => !Me.IsMounted && t.ManaPercentage < Settings.Current.AspectOfTheViperTheshold, RotationCombatUtil.FindMe, Exclusive.HunterAspect),
            new RotationStep(new RotationBuff("Aspect of the Dragonhawk"), 3f, (s, t) => !Me.IsMounted && t.ManaPercentage > Settings.Current.AspectOfTheHawkThreshold, RotationCombatUtil.FindMe, Exclusive.HunterAspect),
            new RotationStep(new RotationBuff("Aspect of the Hawk"), 4f, (s, t) => !Me.IsMounted && t.ManaPercentage > Settings.Current.AspectOfTheHawkThreshold, RotationCombatUtil.FindMe, Exclusive.HunterAspect),
            new RotationStep(new RotationBuff("Aspect of the Monkey"), 5f, (s, t) => !Me.IsMounted && t.ManaPercentage > Settings.Current.AspectOfTheHawkThreshold, RotationCombatUtil.FindMe, Exclusive.HunterAspect),
            new RotationStep(new RotationBuff("Trueshot Aura"), 6f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Mend Pet"), 7f, (s, t) => !Me.IsMounted && Settings.Current.Checkpet && t.IsAlive && t.HealthPercent <= Settings.Current.PetHealth, RotationCombatUtil.FindPet),
        };
    }
}
