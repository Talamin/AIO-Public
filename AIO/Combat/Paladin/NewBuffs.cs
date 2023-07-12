using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class NewBuffs : BaseRotation
    {
        private readonly BaseCombatClass CombatClass;
        private Spec Spec => CombatClass.Specialisation;

        internal NewBuffs(BaseCombatClass combatClass) : base(runInCombat: true, runOutsideCombat: true) => CombatClass = combatClass;

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Seal of Command"), 1f, (s, t) => Spec == Spec.Paladin_SoloRetribution && Settings.Current.SoloSealret == "Seal of Command", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Vengeance"), 1.1f, (s, t) => Spec == Spec.Paladin_SoloRetribution && Settings.Current.SoloSealret == "Seal of Vengeance", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Righteousness"), 1.2f, (s, t) =>Spec == Spec.Paladin_SoloRetribution && Settings.Current.SoloSealret == "Seal of Righteousness", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Command"), 1.3f, (s, t) => Spec == Spec.Paladin_GroupRetribution && Settings.Current.GroupSealret == "Seal of Command", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Vengeance"), 1.4f, (s, t) => Spec == Spec.Paladin_GroupRetribution && Settings.Current.GroupSealret == "Seal of Vengeance", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Righteousness"), 1.5f, (s, t) => Spec == Spec.Paladin_GroupRetribution && Settings.Current.GroupSealret == "Seal of Righteousness", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Righteousness"), 2.1f, (s, t) => Spec == Spec.Paladin_GroupHoly && Me.Level < 38, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Justice"), 3f, (s, t) => Spec == Spec.Paladin_SoloRetribution && Settings.Current.SoloSealret == "Seal of Justice", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Justice"), 3.1f, (s, t) => Spec == Spec.Paladin_GroupRetribution && Settings.Current.GroupSealret == "Seal of Justice", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Vengeance"), 4.1f, (s, t) => ProtSpecs() && Settings.Current.Sealprot == "Seal of Vengeance" && Me.ManaPercentage >= Settings.Current.ProtectionSoL, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Command"), 4.2f, (s, t) => ProtSpecs() && Settings.Current.Sealprot == "Seal of Command" && Me.ManaPercentage >= Settings.Current.ProtectionSoL, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Wisdom"), 5f, (s, t) => ProtSpecs() && Me.ManaPercentage < Settings.Current.ProtectionSoW , RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Wisdom"), 5.1f, (s, t) => Spec == Spec.Paladin_GroupHoly && Me.Level >= 38, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Light"), 6f, (s, t) => ProtSpecs() && Me.ManaPercentage >= Settings.Current.ProtectionSoL && Settings.Current.Sealprot == "Seal of Light", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Righteousness"), 7f, (s, t) => ProtSpecs() && !SpellManager.KnowSpell("Seal of Light"), RotationCombatUtil.FindMe),

            new RotationStep(new RotationBuff("Righteous Fury"), 4f, (s, t) => ProtSpecs(), RotationCombatUtil.FindMe),

            new RotationStep(new RotationBuff("Crusader Aura"), 7.3f, (s,t) => Me.IsMounted && Settings.Current.Crusader, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Retribution Aura"), 8f, (s,t) =>!Me.IsOnTaxi && !Me.IsMounted && Settings.Current.Aura =="Retribution Aura", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Devotion Aura"), 9f, (s,t) =>!Me.IsOnTaxi && !Me.IsMounted && Settings.Current.Aura =="Devotion Aura", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Concentration Aura"), 10f, (s,t) =>!Me.IsOnTaxi && !Me.IsMounted && Settings.Current.Aura =="Concentration Aura", RotationCombatUtil.FindMe),
        };

        private bool ProtSpecs()
        {
            if (Spec == Spec.Paladin_SoloProtection) return true;
            if (Spec == Spec.Paladin_GroupProtection) return true;
            return false;
        }
    }
}