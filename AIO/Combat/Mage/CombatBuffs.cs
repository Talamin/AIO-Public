using AIO.Combat.Common;
using AIO.Framework;
using static AIO.Constants;
using System.Collections.Generic;
using AIO.Settings;
using System.Linq;
using wManager.Wow.Helpers;

namespace AIO.Combat.Mage
{

    using Settings = MageLevelSettings;
    internal class CombatBuffs : BaseRotation
    {
        internal CombatBuffs() : base(runInCombat: true, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Evocation"), 0.5f, (s,t) => !Me.IsMounted && !Settings.Current.GlyphOfEvocation && Me.ManaPercentage <= 30 && RotationFramework.Enemies.Count(o => o.IsTargetingMe) == 0, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Molten Armor"), 1f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Mage Armor"), 2f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Ice Armor"), 3f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Frost Armor"), 4f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Combustion"), 7f, (s,t) => !Me.IsMounted && Fight.InFight, RotationCombatUtil.FindMe)
        };
    }
}


