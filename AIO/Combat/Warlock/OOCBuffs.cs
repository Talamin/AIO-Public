using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    using Settings = WarlockLevelSettings;
    internal class OOCBuffs : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        public List<RotationStep> Rotation => new List<RotationStep> 
        {
            new RotationStep(new RotationBuff("Unending Breath"), 1f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Unending Breath"), 2f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Fel Armor"), 3f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.WarlockSkin),
            new RotationStep(new RotationBuff("Demon Armor"), 4f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.WarlockSkin),
            new RotationStep(new RotationBuff("Demon Skin"), 5f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.WarlockSkin),
            new RotationStep(new RotationBuff("Soul Link"), 6f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Life Tap"), 7f, (s, t) => !Me.IsMounted && Me.ManaPercentage < Me.HealthPercent && Settings.Current.LifeTapOOC, RotationCombatUtil.FindMe),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}
