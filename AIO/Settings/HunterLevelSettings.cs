using AIO.Lists;
using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class HunterLevelSettings : BasePersistentSettings<HunterLevelSettings>
    {

        //Lists
        #region Selectors
        [TriggerDropdown("HunterTriggerDropdown", new string[] { nameof(Spec.Hunter_SoloBeastMastery), nameof(Spec.Hunter_GroupBeastMastery), nameof(Spec.Hunter_SoloSurvival), nameof(Spec.Hunter_SoloMarksmanship) })]
        public override string ChooseRotation { get; set; }
        #endregion
        #region General Settings for all specs
        [DefaultValue(29)]
        [Category("General")]
        [DisplayName("Range")]
        [Description("Set your Range for your FC")]
        public int CombatRange { get; set; }

        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Use Macro for handle Pet?")]
        [Description("This can be used when Actions are blocked by Server")]
        public bool UseMacro { get; set; }


        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Backpaddle")]
        [Description("Auto Backpaddle?")]
        public bool Backpaddle { get; set; }

        [DefaultValue(5)]
        [Category("General")]
        [DisplayName("BackpaddleRange")]
        [Description("Set your Range for your FC Backpaddle")]
        public int BackpaddleRange { get; set; }

        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Pet Feeding")]
        [Description("Want the Pet get Autofeeded?")]
        public bool Petfeed { get; set; }

        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Pet Health OOC")]
        [Description("Should Check Pet Health before attack?")]
        public bool Checkpet { get; set; }

        [DefaultValue(80)]
        [Category("General")]
        [DisplayName("Pet Health OOC")]
        [Description("Set Treshhold for Petattack?")]
        [Percentage(true)]
        public int PetHealth { get; set; }
        #endregion
        #region Aspect settings for all specs
        [Setting]
        [DefaultValue(false)]
        [Category("Aspect")]
        [DisplayName("Use Aspect of Pack")]
        [Description("Set this if you want to use Speedboost")]
        public bool UseAspecofthePack { get; set; }

        [DefaultValue(20)]
        [Category("Aspect")]
        [DisplayName("Aspect of the Viper")]
        [Description("Set the Mana treshold when to use AotV")]
        [Percentage(true)]
        public int AspectOfTheViperTheshold { get; set; }

        [DefaultValue(60)]
        [Category("Aspect")]
        [DisplayName("Aspect of the Hawk/DragonHawk")]
        [Description("Set the your mana threshold when to use Hawk/Dragonhawk")]
        [Percentage(true)]
        public int AspectOfTheHawkThreshold { get; set; }
        #endregion

        #region Beast Mastery       
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloBeastMastery))]
        [DisplayName("Feign Death")]
        [Description("Use Feign Death when targeted and pet is alive?")]
        public bool SoloBeastMasteryFD { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloBeastMastery))]
        [DisplayName("Disengage")]
        [Description("Use Disengage?")]
        public bool SoloBeastMasteryDisengage { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloBeastMastery))]
        [DisplayName("Enable Multishot")]
        [Description("Use Multishot at all?")]
        public bool SoloBeastMasteryMultiShot { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloBeastMastery))]
        [DisplayName("Multishot Targets")]
        [Description("Multishot minimum target Count?")]
        public int SoloBeastMasteryMultiSCount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloBeastMastery))]
        [DisplayName("Misdirection")]
        [Description("Use Misdirection Solo on Pet/Group on Tank?")]
        public bool SoloBeastMasteryMisdirection { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloBeastMastery))]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE")]
        public bool SoloBeastMasteryUseAOE { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloBeastMastery))]
        [DisplayName("AOE Enemy Count")]
        [Description("Number of Targets to use AOE")]
        public int SoloBeastMasteryAOECount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_GroupBeastMastery))]
        [DisplayName("Feign Death")]
        [Description("Use Feign Death when targeted?")]
        public bool GroupBeastMasteryFD { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_GroupBeastMastery))]
        [DisplayName("Enable Deterrence")]
        [Description("Use Deterrence at all?")]
        public bool GroupBeastMasteryDeterrence { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_GroupBeastMastery))]
        [DisplayName("Enable Multishot")]
        [Description("Use Multishot at all?")]
        public bool GroupBeastMasteryMultiShot { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_GroupBeastMastery))]
        [DisplayName("Multishot Targets")]
        [Description("Multishot minimum target Count?")]
        public int GroupBeastMasteryMultiShotCount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_GroupBeastMastery))]
        [DisplayName("Misdirection")]
        [Description("Use Misdirection on Tank?")]
        public bool GroupBeastMasteryMisdirection { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_GroupBeastMastery))]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE")]
        public bool GroupBeastMasteryUseAOE { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_GroupBeastMastery))]
        [DisplayName("AOE Enemy Count")]
        [Description("Number of Targets to use AOE")]
        public int GroupBeastMasteryAOECount { get; set; }


        #endregion

        #region Marksmanship
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloMarksmanship))]
        [DisplayName("Feign Death")]
        [Description("Use Feign Death when targeted and pet is alive?")]
        public bool SoloMarksmanshipFD { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloMarksmanship))]
        [DisplayName("Disengage")]
        [Description("Use Disengage?")]
        public bool SoloMarksmanshipDisengage { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloMarksmanship))]
        [DisplayName("Multishot")]
        [Description("Use Multishot?")]
        public bool SoloMarksmanshipMultiShot { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloMarksmanship))]
        [DisplayName("Multishot")]
        [Description("Use Multishot?")]
        public int SoloMarksmanshipMultiShotCount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloMarksmanship))]
        [DisplayName("Misdirection")]
        [Description("Use Misdirection Solo on Pet/Group on Tank?")]
        public bool SoloMarksmanshipMisdirection { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloMarksmanship))]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE")]
        public bool SoloMarksmanshipUseAOE { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloMarksmanship))]
        [DisplayName("AOE Enemy Count")]
        [Description("Number of Targets to use AOE")]
        public int SoloMarksmanshipAOECount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloMarksmanship))]
        [DisplayName("Aimed Shot")]
        [Description("Use Aimed Shot in Rota?")]
        public bool SoloMarksmanshipAimedShot { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloMarksmanship))]
        [DisplayName("Arcane Shot")]
        [Description("Use Arcane Shot in Rota?")]
        public bool SoloMarksmanshipArcaneShot { get; set; }
        #endregion

        #region Survival
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloSurvival))]
        [DisplayName("Feign Death")]
        [Description("Use Feign Death when targeted and pet is alive?")]
        public bool SoloSurvivalFD { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloSurvival))]
        [DisplayName("Disengage")]
        [Description("Use Disengage?")]
        public bool SoloSurvivalDisengage { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloSurvival))]
        [DisplayName("Multishot")]
        [Description("Use Multishot?")]
        public bool SoloSurvivalUseMultiShot { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", nameof(Spec.Hunter_SoloSurvival))]
        [DisplayName("Multishot")]
        [Description("Use Multishot?")]
        public int SoloSurvivalMultiShotCount { get; set; }
        #endregion

        public HunterLevelSettings()
        {
            CombatRange = 29;
            UseMacro = false;
            PetHealth = 80;
            Backpaddle = true;
            BackpaddleRange = 5;
            Checkpet = true;
            Petfeed = true;
            UseAspecofthePack = false;
            AspectOfTheViperTheshold = 20;
            AspectOfTheHawkThreshold = 60;

            SoloBeastMasteryMultiShot = false;
            SoloBeastMasteryMultiSCount = 3;
            SoloBeastMasteryMisdirection = true;
            SoloBeastMasteryDisengage = false;
            SoloBeastMasteryFD = true;
            SoloBeastMasteryUseAOE = true;
            SoloBeastMasteryAOECount = 3;

            GroupBeastMasteryMultiShot = false;
            GroupBeastMasteryMultiShotCount = 3;
            GroupBeastMasteryMisdirection = true;
            GroupBeastMasteryFD = true;
            GroupBeastMasteryUseAOE = true;
            GroupBeastMasteryAOECount = 3;
            GroupBeastMasteryDeterrence = true;

            SoloMarksmanshipMultiShot = false;
            SoloMarksmanshipMultiShotCount = 3;
            SoloMarksmanshipMisdirection = true;
            SoloMarksmanshipDisengage = false;
            SoloMarksmanshipFD = true;
            SoloMarksmanshipUseAOE = true;
            SoloMarksmanshipAOECount = 3;
            SoloMarksmanshipAimedShot = true;
            SoloMarksmanshipArcaneShot = true;

            SoloSurvivalUseMultiShot = false;
            SoloSurvivalMultiShotCount = 3;
            SoloSurvivalDisengage = false;
            SoloSurvivalFD = true;
        }
    }
}