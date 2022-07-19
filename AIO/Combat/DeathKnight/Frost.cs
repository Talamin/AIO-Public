using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.DeathKnight
{
    using Settings = DeathKnightLevelSettings;
    internal class Frost : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Mind Freeze"), 2f, (s,t) => t.IsCast, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Strangulate"), 3f, (s,t) => t.IsCast && t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell("Howling Blast"), 4f, (s,t) => Me.HaveBuff("Killing Machine") || Me.HaveBuff("Freezing Fog"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Death and Decay"), 5f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance < 15) >= Settings.Current.DnD, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Icebound Fortitude"), 6f, (s,t) => Me.HealthPercent < 80 && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.GetDistance <= 8) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Frost Strike"), 7f, (s,t) => Me.RunicPower > 40, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Icy Touch"), 8f, (s,t) => !t.HaveMyBuff("Frost Fever"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Plague Strike"), 9f, (s,t) => !t.HaveMyBuff("Blood Plague"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Pestilence"), 10f, (s,t) => t.HaveMyBuff("Blood Plague", "Frost Fever") && RotationFramework.Enemies.Count(o => o.GetDistance < 15 && !o.HaveMyBuff("Blood Plague", "Frost Fever")) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Obliterate"), 11f, (s,t) => t.HaveMyBuff("Blood Plague", "Frost Fever"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blood Strike"), 12f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) == Settings.Current.BloodStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Heart Strike"), 13f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) >= Settings.Current.HearthStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blood Boil"), 14f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) > Settings.Current.BloodBoil, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Death Strike"), 15f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget)
        };
    }
}