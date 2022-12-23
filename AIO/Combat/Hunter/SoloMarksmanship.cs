using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class SoloMarksmanship : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Feign Death"), 2f, (s,t) => t.GetDistance < 5 && Me.HealthPercent < 50 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.SoloMarksmanshipFD, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Silencing Shot"), 6f, (s,t) => t.GetDistance >= 5 && t.IsCast ,RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff("Rapid Fire"), 7f, (s,t) =>(RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPet) >=3 && !Me.IsInGroup) 
                    || (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloMarksmanshipAOECount && Me.IsInGroup) 
                    || (t.IsElite && !Me.IsInGroup) 
                    || (Me.IsInGroup && t.IsBoss), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Readiness"), 8f, (s,t) => !Me.HaveBuff("Rapid Fire") && (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPet) >=3 && !Me.IsInGroup)
                    || (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloMarksmanshipAOECount && Me.IsInGroup)
                    || (t.IsElite && !Me.IsInGroup)
                    || (Me.IsInGroup && t.IsBoss), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Kill Shot"), 9f, (s,t) => t.GetDistance >= 5 && t.HealthPercent< 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Volley"), 9.1f, (s,t) => RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloMarksmanshipAOECount && Settings.Current.SoloMarksmanshipUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Hunter's Mark"), 10f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff("Hunter's Mark") && t.IsAlive && t.GetDistance >= 5 && t.HealthPercent > 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Viper Sting"), 11f, (s,t) => t.GetDistance >= 5 && t.HasMana() && Me.ManaPercentage <= 45, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Serpent Sting"), 12f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff("Serpent Sting"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Chimera Shot"), 13f, (s,t) => t.GetDistance >= 5 && t.HaveMyBuff("Serpent Sting","Viper Sting"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Arcane Shot"), 14f, (s,t) => t.GetDistance >= 5 && Settings.Current.SoloMarksmanshipArcaneShot, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Multi-Shot"), 15f, (s,t) => t.GetDistance >= 5 && Settings.Current.SoloMarksmanshipMultiS && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloMarksmanshipMultiSCount, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Aimed Shot"), 15.1f, (s,t) => t.GetDistance >= 5 && Settings.Current.SoloMarksmanshipAimedShot, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Steady Shot"), 16f, (s,t) => t.GetDistance >= 5 && t.HaveMyBuff("Serpent Sting","Viper Sting"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Raptor Strike"), 17f, (s,t) => t.GetDistance < 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Disengage"), 18f, (s,t) => t.GetDistance < 5 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.SoloMarksmanshipDis, RotationCombatUtil.BotTarget),
        };
    }
}
