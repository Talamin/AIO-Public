using AIO.Combat.Common;
using AIO.Framework;
using static AIO.Constants;
using System.Collections.Generic;
using AIO.Settings;
using System.Linq;

namespace AIO.Combat.Mage
{

    using Settings = MageLevelSettings;
    internal class Buffs : BaseRotation
    {
        internal Buffs() : base(runInCombat: true, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Evocation"), 0.5f, (s,t) => !Settings.Current.GlyphOfEvocation && Me.ManaPercentage <= 30 && RotationFramework.Enemies.Count(o => o.IsTargetingMe) == 0, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Molten Armor"), 1f, RotationCombatUtil.Always, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Mage Armor"), 2f, RotationCombatUtil.Always, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Ice Armor"), 3f, RotationCombatUtil.Always, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Frost Armor"), 4f, RotationCombatUtil.Always, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Arcane Intellect"), 5f, (s,t) => !t.HaveBuff("Fel Intelligence") && !t.HaveBuff("Arcane Brilliance"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Arcane Intellect"), 6f, (s,t) => !t.HaveBuff("Fel Intelligence") && !t.HaveBuff("Arcane Brilliance"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Combustion"), 7f, (s,t) => !Me.HaveMyBuff("Combustion"), RotationCombatUtil.FindMe)
        };
    }
}


