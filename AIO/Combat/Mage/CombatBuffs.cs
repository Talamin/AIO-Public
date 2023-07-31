using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Mage
{

    using Settings = MageLevelSettings;
    internal class CombatBuffs : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Evocation"), 0.5f, (s,t) => !Me.IsMounted && !Settings.Current.GlyphOfEvocation && Me.ManaPercentage <= 30 && RotationFramework.Enemies.Count(o => o.IsTargetingMe) == 0, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Molten Armor"), 1f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Mage Armor"), 2f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Ice Armor"), 3f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Frost Armor"), 4f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.MageArmor),
            new RotationStep(new RotationBuff("Combustion"), 7f, (s,t) => !Me.IsMounted && Fight.InFight, RotationCombatUtil.FindMe)
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}


