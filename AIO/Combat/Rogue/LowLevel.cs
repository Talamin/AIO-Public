using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Rogue
{
    using Settings = RogueLevelSettings;
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Eviscerate"), 2f, (s, t) =>Me.ComboPoint >= 1, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Evasion"), 3f, (s, t) =>Me.HealthPercent < 30, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Sinister Strike"), 4f, (s, t) =>true, RotationCombatUtil.BotTarget),
        };
    }
}