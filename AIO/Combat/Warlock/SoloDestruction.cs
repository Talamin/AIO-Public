using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    using Settings = WarlockLevelSettings;
    internal class SoloDestruction : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Life Tap"), 3.5f, (s,t) => Me.ManaPercentage < 15 && Me.HealthPercent > 25, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Life Tap"), 3.6f, (s,t) => Settings.Current.GlyphLifeTap && !Me.HaveBuff("Life Tap") && Me.HealthPercent > 25, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Drain Soul"), 2.5f, (s,t) => t.HealthPercent <= 25 && ItemsHelper.GetItemCount("Soul Shard") <= 3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of the Elements"), 3f, (s,t) => !t.HaveBuff("Curse of the Elements"), RotationCombatUtil.BotTarget),            
            new RotationStep(new RotationSpell("Drain Life"), 3.5f, (s,t) => Me.HealthPercent < 75, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Immolate"), 4f, (s,t) => !t.HaveMyBuff("Immolate"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Corruption"), 5f, (s,t) => !t.HaveMyBuff("Corruption"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Chaos Bolt"), 6f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Incinerate"), 7f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Conflagrate"), 8f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 9f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget)
        };
    }
}
