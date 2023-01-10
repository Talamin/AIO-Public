using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class Buffs : BaseRotation
    {
        internal Buffs() : base(runInCombat: true, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Aspect of the Viper"), 1f, (s, t) => !ObjectManager.Me.IsMounted && t.ManaPercentage < Settings.Current.AspectOfTheViperTheshold, RotationCombatUtil.FindMe, Exclusive.HunterAspect),
            new RotationStep(new RotationBuff("Aspect of the Pack"), 2f, (s, t) =>
            !ObjectManager.Me.IsMounted &&
            Settings.Current.UseAspecofthePack &&
            !ObjectManager.Me.InCombat &&
            ObjectManager.Me.IsInGroup &&
            RotationFramework.Enemies.Count(u => u.IsTargetingMeOrMyPetOrPartyMember) <=0 &&
            t.ManaPercentage < Settings.Current.AspectOfTheHawkThreshold && t.ManaPercentage > Settings.Current.AspectOfTheViperTheshold , RotationCombatUtil.FindMe, Exclusive.HunterAspect),
            new RotationStep(new RotationBuff("Aspect of the Dragonhawk"), 3f, (s, t) => !ObjectManager.Me.IsMounted && t.ManaPercentage > Settings.Current.AspectOfTheHawkThreshold, RotationCombatUtil.FindMe, Exclusive.HunterAspect),
            new RotationStep(new RotationBuff("Aspect of the Hawk"), 4f, (s, t) => !ObjectManager.Me.IsMounted && t.ManaPercentage > Settings.Current.AspectOfTheHawkThreshold, RotationCombatUtil.FindMe, Exclusive.HunterAspect),
            new RotationStep(new RotationBuff("Aspect of the Monkey"), 5f, (s, t) => !ObjectManager.Me.IsMounted && t.ManaPercentage > Settings.Current.AspectOfTheHawkThreshold, RotationCombatUtil.FindMe, Exclusive.HunterAspect),
            new RotationStep(new RotationBuff("Trueshot Aura"), 6f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Mend Pet"), 7f, (s, t) => Settings.Current.Checkpet && t.IsAlive && t.HealthPercent <= Settings.Current.PetHealth, RotationCombatUtil.FindPet),
        };
    }
}
