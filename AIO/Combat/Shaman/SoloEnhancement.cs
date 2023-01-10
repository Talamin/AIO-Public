using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.Shaman
{
    using Settings = ShamanLevelSettings;
    internal class SoloEnhancement : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Feral Spirit"), 1.1f, (s,t) => Settings.Current.SoloEnhancementFeralSpirit =="+2 and Elite" && ((RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=20) >= 2) || t.IsElite), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Feral Spirit"), 1.2f, (s,t) => Settings.Current.SoloEnhancementFeralSpirit =="+3 and Elite" && ((RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=20) >= 3) || t.IsElite), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Feral Spirit"), 1.3f, (s,t) => Settings.Current.SoloEnhancementFeralSpirit =="only Elite" && t.IsElite, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Healing Wave"), 1.5f, (s,t) => !Me.IsInGroup && Me.HealthPercent < Settings.Current.SoloEnhancementHealthForHeals && t.HealthPercent > Settings.Current.SoloEnhancementEnemyHPSkipHealing, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Cure Toxins"), 2f, (s,t) => (Me.HasDebuffType("Disease") || Me.HasDebuffType("Poison")) && (Settings.Current.CureToxin == "Group" || Settings.Current.CureToxin == "Self"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Cure Toxins"), 3f, (s,t) => (t.HasDebuffType("Disease") || t.HasDebuffType("Poison")) && Settings.Current.CureToxin == "Group", RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Lightning Bolt"), 4f, (s,t) => Me.ManaPercentage >= Settings.Current.SoloEnhancementManaSavedForHeals && Me.BuffStack(53817) >=5 && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 10) == 1, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Chain Lightning"), 5f, (s,t) => Me.ManaPercentage >= Settings.Current.SoloEnhancementManaSavedForHeals && Me.BuffStack(53817) >=5 && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 10) >= 2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Fire Nova"), 5.1f, (s,t) => Totems.HasAny("Magma Totem", "Searing Totem", "Flametongue Totem") && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 10) >= Settings.Current.SoloEnhancementUseFireNova, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Shamanistic Rage"), 9f, (s,t) => Me.ManaPercentage <= Settings.Current.SoloEnhancementManaSavedForHeals , RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Wind Shear"), 10f, (s,t) => Me.ManaPercentage >= Settings.Current.SoloEnhancementManaSavedForHeals && Me.ManaPercentage>= 40 && t.IsCasting() && t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell("Lightning Bolt"), 22f, (s,t) =>  Me.ManaPercentage >= Settings.Current.SoloEnhancementManaSavedForHeals && !Me.InCombatFlagOnly && t.HealthPercent == 100, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Stormstrike"), 23f, (s,t) =>  Me.ManaPercentage >= Settings.Current.SoloEnhancementManaSavedForHeals, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Flame Shock"), 24f, (s,t) => Me.ManaPercentage >= Settings.Current.SoloEnhancementManaSavedForHeals && !t.HaveMyBuff("Flame Shock") && (t.HealthPercent > 15), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Earth Shock"), 25f, (s,t) => Me.ManaPercentage >= Settings.Current.SoloEnhancementManaSavedForHeals && !t.HaveMyBuff("Earth Shock"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Lava Lash"), 26f, (s,t) =>  Me.ManaPercentage >= Settings.Current.SoloEnhancementManaSavedForHeals, RotationCombatUtil.BotTarget),
        };
    }
}