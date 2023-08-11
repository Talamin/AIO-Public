using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.Shaman
{
    using Settings = ShamanLevelSettings;
    internal class GroupEnhancement : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Feral Spirit"), 1.1f, (s,t) => Settings.Current.GroupEnhancementFeralSpirit =="+2 and Elite" && ((RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=20) >= 2) || t.IsElite), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Feral Spirit"), 1.2f, (s,t) => Settings.Current.GroupEnhancementFeralSpirit =="+3 and Elite" && ((RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=20) >= 3) || t.IsElite), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Feral Spirit"), 1.3f, (s,t) => Settings.Current.GroupEnhancementFeralSpirit =="only Elite" && t.IsElite, RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell("Cure Toxins"), 2f, (s,t) =>
                (RotationCombatUtil.IHaveCachedDebuff(DebuffType.Poison) || RotationCombatUtil.IHaveCachedDebuff(DebuffType.Disease))
                && (Settings.Current.CureToxin == "Group" || Settings.Current.CureToxin == "Self"),
                RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Cure Toxins"), 2.1f, (s,t) =>
                Settings.Current.CureToxin == "Group",
                p => RotationCombatUtil.GetPartyMembersWithCachedDebuff(DebuffType.Poison, true, 30).FirstOrDefault()),
            new RotationStep(new RotationSpell("Cure Toxins"), 2.1f, (s,t) =>
                Settings.Current.CureToxin == "Group",
                p => RotationCombatUtil.GetPartyMembersWithCachedDebuff(DebuffType.Disease, true, 30).FirstOrDefault()),

            new RotationStep(new RotationSpell("Wind Shear"), 3f, (s,t) => t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 20, RotationCombatUtil.FindEnemyCastingWithLoS),
            new RotationStep(new RotationSpell("Lightning Bolt"), 4f, (s,t) => Me.BuffStack(53817) >= 5 && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 10) == 1, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Chain Lightning"), 5f, (s,t) => Me.BuffStack(53817) >= 5 && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 10) >= 2, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Magma Totem"),5.1f, (s,t) => Totems.DistanceToTotem("Magma Totem") > 8 && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(Me.Position) <= 7) >= Settings.Current.GroupEnhancementRedeployMagma, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Fire Nova"), 5.1f, (s,t) => Totems.HasAny("Magma Totem", "Searing Totem", "Flametongue Totem") && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 10) >= Settings.Current.GroupEnhancementUseFireNova, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Shamanistic Rage"), 9f, (s,t) => Me.ManaPercentage <= Settings.Current.GroupEnhancementShamanisticRageMana , RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Stormstrike"), 10f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Flame Shock"), 11f, (s,t) => !t.HaveMyBuff("Flame Shock") && t.HealthPercent > 15, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Earth Shock"), 12f, (s,t) => Me.ManaPercentage >= Settings.Current.GroupEnhancementConserveMana && !t.HaveMyBuff("Earth Shock"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Fire Nova"), 13f, (s,t) => Totems.HasAny("Magma Totem", "Searing Totem", "Flametongue Totem") &&  Me.ManaPercentage >= Settings.Current.GroupEnhancementConserveMana, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Lava Lash"), 14f, (s,t) =>  Me.ManaPercentage >= Settings.Current.GroupEnhancementConserveMana, RotationCombatUtil.BotTargetFast),
        };
    }
}