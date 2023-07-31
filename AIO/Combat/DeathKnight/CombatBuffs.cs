using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.DeathKnight
{
    using Settings = DeathKnightLevelSettings;
    internal class CombatBuffs : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Blood Presence"), 1f, (s, t) => Settings.Current.Presence == "BloodPresence", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Frost Presence"), 2f, (s, t) => Settings.Current.Presence == "FrostPresence", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Unholy Presence"), 3f, (s, t) => Settings.Current.Presence == "UnholyPresence", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Horn of Winter"), 4f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Bone Shield"), 5f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe)
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}
