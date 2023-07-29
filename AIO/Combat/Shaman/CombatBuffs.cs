using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Shaman
{
    using Settings = ShamanLevelSettings;

    internal class CombatBuffs : BaseRotation
    {
        private readonly BaseCombatClass CombatClass;
        private readonly Totems Totems;
        private Spec Spec => CombatClass.Specialisation;
        internal CombatBuffs(BaseCombatClass combatClass, Totems totems) 
            : base(useCombatSynthetics: Settings.Current.UseSyntheticCombatEvents, runInCombat: true, runOutsideCombat: true)
        {
            CombatClass = combatClass;
            Totems = totems;
        }

        protected override List<RotationStep> Rotation => new List<RotationStep> {

            new RotationStep(new RotationBuff("Water Shield"), 2f, (s,t) => !Me.IsMounted && (Spec == Spec.Shaman_GroupRestoration || Spec == Spec.Shaman_SoloElemental || (Spec == Spec.Shaman_SoloEnhancement && Me.ManaPercentage <= 50)), RotationCombatUtil.FindMe, Exclusive.ShamanShield),
            new RotationStep(new RotationBuff("Lightning Shield"), 3f, (s,t) => !Me.IsMounted && (Me.ManaPercentage > 50 || !SpellManager.KnowSpell("Water Shield")) && !Me.HaveBuff("Water Shield"), RotationCombatUtil.FindMe, Exclusive.ShamanShield),

            new RotationStep(new RotationSpell("Totemic Recall"), 10f, (s,t) => !Me.IsMounted && Totems.ShouldRecall() && !Totems.HasAny("Earth Elemental Totem", "Mana Tide Totem","Stoneclaw Totem"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Call of the Elements"), 11f, (s,t) => !Me.IsMounted && Settings.Current.UseCotE && !MovementManager.InMovement && Totems.MissingDefaults() && !Totems.HasTemporary(), RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Mana Tide Totem"), 20f, (s,t) => !Me.IsMounted && Me.ManaPercentage <= 30, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Earth Elemental Totem"), 30f, (s,t) =>
                !Me.IsInGroup &&
                !Totems.HasAny("Stoneclaw Totem") &&
                RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 20) >= 3, RotationCombatUtil.FindMe),
            
            new RotationStep(new RotationSpell("Stoneclaw Totem"), 31f, (s,t) =>
                !Me.IsInGroup &&
                !Totems.HasAny("Earth Elemental Totem") &&
                RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 20) >= 2, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Magma Totem"), 40f, (s,t) =>
                Spec == Spec.Shaman_SoloEnhancement &&
                Target.GetDistance <= 15 &&
                !Totems.HasAny("Magma Totem") &&
                Me.ManaPercentage > 40, RotationCombatUtil.FindMe),

             new RotationStep(new RotationSpell("Searing Totem"), 45f, (s,t) =>
                Target.GetDistance <= 15 &&
                !Totems.HasAny("Searing Totem") &&
                !Totems.HasAny("Magma Totem") &&
                Settings.Current.RedeploySearingTotem &&
                Me.ManaPercentage > 30, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Cleansing Totem"), 50f, (s,t) =>
                Settings.Current.UseCleansingTotem &&
                RotationFramework.PartyMembers.Count(o =>
                o.HasDebuffType("Poison","Disease")) > 0 &&
                !Totems.HasAny("Cleansing Totem"), RotationCombatUtil.FindMe),
            
            new RotationStep(new RotationSpell("Tremor Totem"), 51f, (s,t) =>
                RotationFramework.PartyMembers.Count(o =>
                o.HasDebuffType("Fear","Charm","Sleep")) > 0, RotationCombatUtil.FindMe),
            
            new RotationStep(new RotationSpell("Grounding Totem"), 52f, (s,t) =>
                Settings.Current.UseGroundingTotem &&
                RotationFramework.Enemies.Count(o => o.GetDistance < 30 && o.IsCast) > 0, RotationCombatUtil.FindMe),
            
            new RotationStep(new RotationSpell("Earthbind Totem"), 53f, (s,t) => 
                Settings.Current.UseEarthbindTotem &&
                RotationFramework.Enemies.Count(o => o.GetDistance < 10 && o.CreatureTypeTarget=="Humanoid") > 0, RotationCombatUtil.FindMe)
        };
    }
}