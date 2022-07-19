using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class HunterLevelSettings : BasePersistentSettings<HunterLevelSettings>
    {
        [Setting]
        [DefaultValue(29)]
        [Category("General")]
        [DisplayName("Range")]
        [Description("Set your Range for your FC")]
        public int RangeSet { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Feign Death")]
        [Description("Should use Feign Death?")]
        public bool FD { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Pet")]
        [DisplayName("Use Macro for handle Pet?")]
        [Description("This can be used when Actions are blocked by Server")]
        public bool UseMacro { get; set; }

        //[Setting]
        //[DefaultValue(true)]
        //[Category("Fight")]
        //[DisplayName("Aspect of the Pack")]
        //[Description("Use OOC Aspect of the Pack")]
        //[Percentage(true)]
        //public bool AspecofCheetah { get; set; }

        [Setting]
        [DefaultValue(20)]
        [Category("Fight")]
        [DisplayName("Aspect of the Viper")]
        [Description("Set the your  Mana  Treshold when to use AotV")]
        [Percentage(true)]
        public int AspecofViper { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Fight")]
        [DisplayName("Aspect of the Hawk/DragonHawk")]
        [Description("Set the your  Mana  Treshold when to use Hawk/Dragonhawk")]
        [Percentage(true)]
        public int AspecofHawks { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Fight")]
        [DisplayName("Disengage")]
        [Description("Use  Disengage?")]
        public bool Dis { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Fight")]
        [DisplayName("Multishot")]
        [Description("Use  Multishot?")]
        public bool MultiS { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Fight")]
        [DisplayName("Multishot")]
        [Description("Use  Multishot?")]
        public int MultiSCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Backpaddle")]
        [Description("Auto Backpaddle?")]
        public bool Backpaddle { get; set; }

        [Setting]
        [DefaultValue(5)]
        [Category("Fight")]
        [DisplayName("BackpaddleRange")]
        [Description("Set your Range for your FC Backpaddle")]
        public int BackpaddleRange { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Pet")]
        [DisplayName("Pet Feeding")]
        [Description("Want the Pet get Autofeeded?")]
        public bool Petfeed { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Pet")]
        [DisplayName("Pet Health OOC")]
        [Description("Should Check Pet Health before attack?")]
        public bool Checkpet { get; set; }

        [Setting]
        [DefaultValue(80)]
        [Category("Pet")]
        [DisplayName("Pet Health OOC")]
        [Description("Set Treshhold for Petattack?")]
        [Percentage(true)]
        public int PetHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("BeastMastery")]
        [DisplayName("Misdirection")]
        [Description("Use Misdirection Solo on Pet/Group on Tank?")]
        public bool BeastMasteryMisdirection { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Marksman")]
        [DisplayName("Aimed Shot")]
        [Description("Use Aimed Shot in Rota?")]
        public bool MarksmanAimedShot { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Marksman")]
        [DisplayName("Arcane Shot")]
        [Description("Use Arcane Shot in Rota?")]
        public bool MarksmanArcaneShot { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE")]
        public bool UseAOE { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Fight")]
        [DisplayName("Use Aspec of Pack")]
        [Description("Set this if you want to use in Instance")]
        public bool UseAspecofthePack { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Fight")]
        [DisplayName("AOE Enemy Count")]
        [Description("Number of Targets to use AOE")]
        public int AOECount { get; set; }

        [DropdownList(new string[] { "HunterBeastMastery", "HunterSurvival", "HunterMarksmanship" })]
        public override string ChooseTalent { get; set; }

        [DropdownList(new string[] { "Auto", "BeastMastery", "Survival", "Marksmanship" })]
        public override string ChooseRotation { get; set; }

        public HunterLevelSettings()
        {
            ChooseTalent = "HunterBeastMastery";
            UseAspecofthePack = false;
            RangeSet = 29;
            UseMacro = false;
            PetHealth = 80;
            AspecofViper = 20;
            AspecofHawks = 60;
            //AspecofCheetah = true;
            MultiS = false;
            MultiSCount = 3;
            Backpaddle = true;
            BackpaddleRange = 5;
            Checkpet = true;
            Petfeed = true;
            MarksmanAimedShot = true;
            MarksmanArcaneShot = true;
            BeastMasteryMisdirection = true;
            Dis = false;
            FD = true;
            UseAOE = true;
            AOECount = 3;
        }
    }
}