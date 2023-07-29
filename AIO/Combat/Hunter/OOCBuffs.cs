using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class OOCBuffs : BaseRotation
    {
        internal OOCBuffs() : base(runInCombat: false, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Aspect of the Pack"), 2f, (s, t) =>
                !Me.IsMounted
                && Settings.Current.UseAspecofthePack
                && !Me.InCombat
                && Me.IsInGroup
                && RotationFramework.Enemies.Count(u => u.IsTargetingMeOrMyPetOrPartyMember) <= 0
                && t.ManaPercentage < Settings.Current.AspectOfTheHawkThreshold
                && t.ManaPercentage > Settings.Current.AspectOfTheViperTheshold , RotationCombatUtil.FindMe, Exclusive.HunterAspect),
        };
    }
}
