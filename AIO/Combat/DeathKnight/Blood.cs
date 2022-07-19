using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.DeathKnight
{
    using Settings = DeathKnightLevelSettings;
    internal class Blood : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            //Taunt Offtargets
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Dark Command"), 1.1f, (s,t) => Me.IsInGroup && Settings.Current.DarkCommand &&  RotationFramework.Enemies.Count(o => !o.IsTargetingMe && o.IsTargetingPartyMember) >=1, RotationCombatUtil.FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell("Death Grip"), 1.2f, (s,t) => Settings.Current.DeathGrip && Me.IsInGroup && RotationFramework.Enemies.Count(o => !o.IsTargetingMe && o.IsTargetingPartyMember) >=1,RotationCombatUtil.FindEnemyAttackingGroup),
           
            //Interrupt
            new RotationStep(new RotationSpell("Mind Freeze"), 2f, (s,t) => t.IsCast, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Strangulate"), 2.1f, (s,t) => t.IsCast && t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell("Death Grip"), 2.2f, (s,t) => Settings.Current.DeathGrip && t.IsCast && t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            // Defensive Shell on myself
            new RotationStep(new RotationSpell("Anti Magic Shell"), 3.1f, (s,t) => RotationFramework.Enemies.Count(o => o.IsCast && o.IsTargetingMe) >=1, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Vampiric Blood"), 3.2f, (s,t) => Me.HealthPercent <= 30, RotationCombatUtil.FindMe),
            // other useful  Spells
            new RotationStep(new RotationSpell("Empower Rune Weapon"), 3.5f, (s,t) => Me.RunesReadyCount() <= 2, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Chains of Ice"), 3.7f, (s,t) => t.Fleeing, RotationCombatUtil.BotTarget),
            //Damage Part
            new RotationStep(new RotationSpell("Death and Decay"), 4f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance < 15) >= Settings.Current.DnD, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blood Tap"), 4.1f, (s,t) => Me.RuneIsReady(1) || Me.RuneIsReady(2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Icebound Fortitude"), 5.1f, (s,t) => Me.HealthPercent < 80 && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.GetDistance <= 8) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Mark of Blood"), 6f, (s,t)  => BossList.isboss ||(t.IsElite && Me.IsInGroup), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Dancing Rune Weapon"), 7f, (s,t)  => BossList.isboss ||(t.IsElite && !Me.IsInGroup) || (RotationFramework.Enemies.Count(o => o.GetDistance <= 10) >=2 && !Me.IsInGroup), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Death Grip"), 8.1f, (s,t) => !Me.IsInGroup && t.IsAttackable && !t.IsTargetingMe && t.IsMyTarget && !TraceLine.TraceLineGo(Me.Position, t.Position) && t.GetDistance >= 7 && Settings.Current.DeathGrip, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Icy Touch"), 10f, (s,t) => !t.HaveMyBuff("Frost Fever"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Plague Strike"), 11f, (s,t) => !t.HaveMyBuff("Blood Plague"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Pestilence"), 12f, (s,t) => t.HaveMyBuff("Blood Plague", "Frost Fever") && RotationFramework.Enemies.Count(o => o.GetDistance < 15 && !o.HaveMyBuff("Blood Plague", "Frost Fever")) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blood Strike"), 13f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) == Settings.Current.BloodStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Heart Strike"), 14f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) >= Settings.Current.HearthStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blood Boil"), 15f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) > Settings.Current.BloodBoil, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Death Strike"), 16f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Death Coil"), 17f, (s,t) => Me.RunicPower >= 40, RotationCombatUtil.BotTarget)
        };
    }
}
