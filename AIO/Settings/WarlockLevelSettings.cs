using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class WarlockLevelSettings : BasePersistentSettings<WarlockLevelSettings>
    {
        [Setting]
        [DefaultValue(true)]
        [Category("Pet")]
        [DisplayName("Cast Pet Infight?")]
        [Description("Checks if Pet is dead and Cast Infight?")]
        public bool PetInfight { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Pet")]
        [DisplayName("Pets for Warlock")]
        [Description("Set your Pet")]
        [DropdownList(new string[] { "Felguard", "Voidwalker", "Imp", "Felhunter" })]
        public string Pet { get; set; }


        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Buffing")]
        [Description("True/False for Buffing?")]
        public bool Buffing { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Soulshards")]
        [Description("Automanage your Soulshards?")]
        public bool Soulshards { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Healthstone")]
        [Description("Use Healthstone / Cast Healthstone?")]
        public bool Healthstone { get; set; }

        [Setting]
        [DefaultValue(20)]
        [Category("Fight")]
        [DisplayName("Lifetap")]
        [Description("Tells on which Mana % to use Lifetap")]
        [Percentage(true)]
        public int Lifetap { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Fight")]
        [DisplayName("Drain Life")]
        [Description("Tells on which Health % to use Drain Life")]
        [Percentage(true)]
        public int Drainlife { get; set; }

        [Setting]
        [DefaultValue(20)]
        [Category("Fight")]
        [DisplayName("Use Wand Treshold?")]
        [Description("Enemy Life Treshold for Wandusage?")]
        [Percentage(true)]
        public int UseWandTresh { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Use Wand")]
        [Description("Use Wand?")]
        public bool UseWand { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Shadowbolt")]
        [Description("should Shadowbolt ignore Wand Treshhold?")]
        public bool ShadowboltWand { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Fight")]
        [DisplayName("Health Funnel Pet")]
        [Description("Tells on which PetHealth % to use Health Funnel")]
        [Percentage(true)]
        public int HealthfunnelPet { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Fight")]
        [DisplayName("Health Funnel Player")]
        [Description("Tells until which PlayerHealth % to use Health Funnel")]
        [Percentage(true)]
        public int HealthfunnelMe { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Demonology")]
        [DisplayName("Metamorphosis")]
        [Description("When to use  Metamorphosis?")]
        [DropdownList(new string[] { "OnCooldown", "OnBosses", "None" })]
        public string Metamorphosis { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Affliction")]
        [DisplayName("Curse of")]
        [Description("Which Curse you want?")]
        [DropdownList(new string[] { "Agony", "Doom", "Elements", "Tongues", "Weakness", "Exhaustion" })]
        public string AfflCurse { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Fight")]
        [DisplayName("Use AOE in Instance")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool UseAOE { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Use AOE Outside Instance")]
        [Description("Set this if you want to use AOE in Outside Instance")]
        public bool UseAOEOutside { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Use Seed of Corruption")]
        [Description("Make use of SoC while in Group?")]
        public bool UseSeedGroup { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Use Corruption on Multidot")]
        [Description("Make use of Corruption while in Group on multiple Enemies?")]
        public bool UseCorruptionGroup { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Fight")]
        [DisplayName("AOE in Instance")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int AOEInstance { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Fight")]
        [DisplayName("AOE Outside Instance")]
        [Description("Number of Targets to use AOE in Outside Instance")]
        [Percentage(false)]
        public int AOEOutsideInstance { get; set; }

        [DropdownList(new string[] { "WarlockDestruction", "WarlockAffliction", "WarlockDemonology" })]
        public override string ChooseTalent { get; set; }

        [DropdownList(new string[] { "Auto", "Destruction", "Affliction", "Demonology" })]
        public override string ChooseRotation { get; set; }

        public WarlockLevelSettings()
        {
            ChooseTalent = "WarlockAffliction";
            Healthstone = true;
            Pet = "Voidwalker";
            PetInfight = true;
            UseSeedGroup = true;
            UseCorruptionGroup = true;
            Lifetap = 20;
            Drainlife = 40;
            HealthfunnelPet = 30;
            HealthfunnelMe = 50;
            UseAOE = false;
            UseAOEOutside = true;
            AOEInstance = 3;
            AOEOutsideInstance = 3;
            UseWandTresh = 20;
            UseWand = true;
            Buffing = true;
            Soulshards = true;
            ShadowboltWand = true;
            Metamorphosis = "OnCooldown";
            AfflCurse = "Agony";
        }
    }
}

