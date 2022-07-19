using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    using Settings = WarlockLevelSettings;
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Immolate"), 3f, (s,t) => !t.HaveMyBuff("Immolate") && SpellManager.KnowSpell("Curse of Agony"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of Agony"), 4f, (s,t) => !t.HaveMyBuff("Curse of Agony"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Corruption"), 5f, (s,t) => !t.HaveMyBuff("Corruption"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 6f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget)
        };
    }
}