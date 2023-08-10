using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class SoloBeastMastery : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Feign Death"), 2f, (s,t) => t.GetDistance < 5 && Me.HealthPercent < 50 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.SoloBeastMasteryFD, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Deterrence"), 2.1f, (s,t) => t.IsTargetingMe && Me.HealthPercent < 50, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Intimidation"), 3f, (s,t) => Pet.Target == Me.Target && Pet.Position.DistanceTo(t.Position) <= 6 && t.IsCasting(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Concussive Shot"), 3.1f, (s,t) => t.Fleeing && !t.HaveBuff("Concussive Shot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Misdirection"), 3.2f, (s,t) => Settings.Current.SoloBeastMasteryMisdirection && !Me.IsInGroup && !Me.HaveBuff("Misdirection") && Pet.IsAlive && t.IsMyPet && RotationFramework.Enemies.Count(u => u.IsTargetingMe) >=1 , RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell("Misdirection"), 3.3f, (s,t) => Settings.Current.SoloBeastMasteryMisdirection && Me.IsInGroup && !Me.HaveBuff("Misdirection") && t.IsAlive , RotationCombatUtil.FindTank),
            new RotationStep(new RotationSpell("Volley"), 4f, (s,t) => Settings.Current.SoloBeastMasteryUseAOE && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloBeastMasteryAOECount, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Kill Shot"), 5f, (s,t) => t.GetDistance >= 5 && t.HealthPercent< 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Hunter's Mark"), 9f, (s,t) => !t.HaveMyBuff("Hunter's Mark") && t.IsAlive && t.GetDistance >= 5 && t.HealthPercent > 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Bestial Wrath"), 10f, (s,t) => (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPet) >=2 && !Me.IsInGroup) || (t.IsElite && !Me.IsInGroup) || (Me.IsInGroup && t.IsBoss), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Rapid Fire"), 11f, (s,t) => (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPet) >= Settings.Current.SoloBeastMasteryAOECount && !Me.IsInGroup)||  (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloBeastMasteryAOECount && Me.IsInGroup)  || (t.IsElite && !Me.IsInGroup) || (Me.IsInGroup && BossList.MyTargetIsBoss), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Kill Command"), 12f, (s,t) => !Me.HaveBuff("Kill Command"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Serpent Sting"), 13f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff("Serpent Sting") && (t.HealthPercent >= 70 || (BossList.MyTargetIsBoss && t.HealthPercent >= 20)), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Arcane Shot"), 14f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Multi-Shot"), 15f, (s,t) => t.GetDistance >= 5 && Settings.Current.SoloBeastMasteryMultiShot && RotationFramework.Enemies.Count(o => o.IsAttackable && o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloBeastMasteryMultiSCount, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Steady Shot"), 15.1f, (s,t) => !Me.GetMove && t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Raptor Strike"), 16f, (s,t) => t.GetDistance < 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Disengage"), 17f, (s,t) => t.GetDistance < 5 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.SoloBeastMasteryDisengage, RotationCombatUtil.BotTarget),
        };
    }
}
