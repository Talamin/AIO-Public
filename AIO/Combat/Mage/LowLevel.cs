using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Fire Blast"), 2.1f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh , RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Fireball"), 3f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh  && !SpellManager.KnowSpell("Frostbolt"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Frostbolt"), 4f, (s,t) =>  Me.ManaPercentage > Settings.Current.UseWandTresh , RotationCombatUtil.BotTarget),
        };
    }
}
