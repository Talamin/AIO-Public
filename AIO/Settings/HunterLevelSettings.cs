using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class HunterLevelSettings : BasePersistentSettings<HunterLevelSettings>
    {
        [DefaultValue(29)]
        [Category("General")]
        [DisplayName("Range")]
        [Description("Set your Range for your FC")]
        public int RangeSet { get; set; }

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

        [Setting]
        [DefaultValue(false)]
        [Category("Aspec")]
        [DisplayName("Use Aspec of Pack")]
        [Description("Set this if you want to use Speedboost")]
        public bool UseAspecofthePack { get; set; }

        [DefaultValue(20)]
        [Category("Aspec")]
        [DisplayName("Aspect of the Viper")]
        [Description("Set the your  Mana  Treshold when to use AotV")]
        [Percentage(true)]
        public int AspecofViper { get; set; }

        [DefaultValue(60)]
        [Category("Aspec")]
        [DisplayName("Aspect of the Hawk/DragonHawk")]
        [Description("Set the your  Mana  Treshold when to use Hawk/Dragonhawk")]
        [Percentage(true)]
        public int AspecofHawks { get; set; }

        //RotationSpecific BeastMastery

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloBeastMastery")]
        [DisplayName("Feign Death")]
        [Description("Use Feign Death when targeted and pet is alive?")]
        public bool SoloBeastMasteryFD { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloBeastMastery")]
        [DisplayName("Disengage")]
        [Description("Use  Disengage?")]
        public bool SoloBeastMasteryDis { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloBeastMastery")]
        [DisplayName("Multishot")]
        [Description("Use  Multishot?")]
        public bool SoloBeastMasteryMultiS { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloBeastMastery")]
        [DisplayName("Multishot")]
        [Description("Use  Multishot?")]
        public int SoloBeastMasteryMultiSCount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloBeastMastery")]
        [DisplayName("Misdirection")]
        [Description("Use Misdirection Solo on Pet/Group on Tank?")]
        public bool SoloBeastMasteryMisdirection { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloBeastMastery")]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE")]
        public bool SoloBeastMasteryUseAOE { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloBeastMastery")]
        [DisplayName("AOE Enemy Count")]
        [Description("Number of Targets to use AOE")]
        public int SoloBeastMasteryAOECount { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloBeastMastery")]
        [DisplayName("Use Aspec of Pack")]
        [Description("Set this if you want to use in Instance")]
        public bool SoloBeastMasteryUseAspecofthePack { get; set; }

        //Rotationspecific Marksmanship

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloMarksmanship")]
        [DisplayName("Feign Death")]
        [Description("Use Feign Death when targeted and pet is alive?")]
        public bool SoloMarksmanshipFD { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloMarksmanship")]
        [DisplayName("Disengage")]
        [Description("Use  Disengage?")]
        public bool SoloMarksmanshipDis { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloMarksmanship")]
        [DisplayName("Multishot")]
        [Description("Use  Multishot?")]
        public bool SoloMarksmanshipMultiS { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloMarksmanship")]
        [DisplayName("Multishot")]
        [Description("Use  Multishot?")]
        public int SoloMarksmanshipMultiSCount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloMarksmanship")]
        [DisplayName("Misdirection")]
        [Description("Use Misdirection Solo on Pet/Group on Tank?")]
        public bool SoloMarksmanshipMisdirection { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloMarksmanship")]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE")]
        public bool SoloMarksmanshipUseAOE { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloMarksmanship")]
        [DisplayName("AOE Enemy Count")]
        [Description("Number of Targets to use AOE")]
        public int SoloMarksmanshipAOECount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloMarksmanship")]
        [DisplayName("Aimed Shot")]
        [Description("Use Aimed Shot in Rota?")]
        public bool SoloMarksmanshipAimedShot { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloMarksmanship")]
        [DisplayName("Arcane Shot")]
        [Description("Use Arcane Shot in Rota?")]
        public bool SoloMarksmanshipArcaneShot { get; set; }

        //Rotationspecific Survival

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloSurvival")]
        [DisplayName("Feign Death")]
        [Description("Use Feign Death when targeted and pet is alive?")]
        public bool SoloSurvivalFD { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloSurvival")]
        [DisplayName("Disengage")]
        [Description("Use  Disengage?")]
        public bool SoloSurvivalDis { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloSurvival")]
        [DisplayName("Multishot")]
        [Description("Use  Multishot?")]
        public bool SoloSurvivalMultiS { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloSurvival")]
        [DisplayName("Multishot")]
        [Description("Use  Multishot?")]
        public int SoloSurvivalMultiSCount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloSurvival")]
        [DisplayName("Misdirection")]
        [Description("Use Misdirection Solo on Pet/Group on Tank?")]
        public bool SoloSurvivalMisdirection { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloSurvival")]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE")]
        public bool SoloSurvivalUseAOE { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("HunterTriggerDropdown", "SoloSurvival")]
        [DisplayName("AOE Enemy Count")]
        [Description("Number of Targets to use AOE")]
        public int SoloSurvivalAOECount { get; set; }


        //Lists

        [DropdownList(new string[] { "HunterBeastMastery", "HunterSurvival", "HunterMarksmanship" })]
        public override string ChooseTalent { get; set; }

        [TriggerDropdown("HunterTriggerDropdown",new string[] { "Auto", "SoloBeastMastery", "SoloSurvival", "SoloMarksmanship" })]
        public override string ChooseRotation { get; set; }

        public HunterLevelSettings()
        {
            //General Settings
            ChooseTalent = "HunterBeastMastery";
            RangeSet = 29;
            UseMacro = false;
            PetHealth = 80;
            Backpaddle = true;
            BackpaddleRange = 5;
            Checkpet = true;
            Petfeed = true;

            UseAspecofthePack = false;
            AspecofViper = 20;
            AspecofHawks = 60;

            SoloBeastMasteryMultiS = false;
            SoloBeastMasteryMultiSCount = 3;
            SoloBeastMasteryMisdirection = true;
            SoloBeastMasteryDis = false;
            SoloBeastMasteryFD = true;
            SoloBeastMasteryUseAOE = true;
            SoloBeastMasteryAOECount = 3;

            SoloMarksmanshipMultiS = false;
            SoloMarksmanshipMultiSCount = 3;
            SoloMarksmanshipMisdirection = true;
            SoloMarksmanshipDis = false;
            SoloMarksmanshipFD = true;
            SoloMarksmanshipUseAOE = true;
            SoloMarksmanshipAOECount = 3;
            SoloMarksmanshipAimedShot = true;
            SoloMarksmanshipArcaneShot = true;

            SoloSurvivalMultiS = false;
            SoloSurvivalMultiSCount = 3;
            SoloSurvivalMisdirection = true;
            SoloSurvivalDis = false;
            SoloSurvivalFD = true;
            SoloSurvivalUseAOE = true;
            SoloSurvivalAOECount = 3;


        }
    }
}