using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class WarlockLevelSettings : BasePersistentSettings<WarlockLevelSettings>
    {

        //Lists

        [DropdownList(new string[] { "WarlockDestruction", "WarlockAffliction", "WarlockDemonology", "GroupWarlockAffliction" })]
        public override string ChooseTalent { get; set; }

        [TriggerDropdown("WarlockTriggerDropdown",new string[] { "Auto", "SoloDestruction", "SoloAffliction", "SoloDemonology", "GroupAffliction" })]
        public override string ChooseRotation { get; set; }

        //Pet
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

        //General
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
        [Category("General")]
        [DisplayName("Buffing")]
        [Description("True/False for Buffing?")]
        public bool Buffing { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Life Tap Glyph")]
        [Description("Do we have Glyph of Life Tap?")]
        public bool GlyphLifeTap { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Life Tap OOC")]
        [Description("Should Life Tap be used out of combat in dungeons?")]
        public bool LifeTapOOC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Soulshards")]
        [Description("Automanage your Soulshards?")]
        public bool Soulshards { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Healthstone")]
        [Description("Use Healthstone / Cast Healthstone?")]
        public bool Healthstone { get; set; }

        [DefaultValue(4)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "")]
        [DisplayName("AOE in Instance")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int SoloFrostAOEInstance { get; set; }

        //Rotation SoloAffliction
        [DefaultValue(20)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloAffliction")]
        [DisplayName("Lifetap")]
        [Description("Tells on which Mana % to use Lifetap")]
        [Percentage(true)]
        public int SoloAfflictionLifetap { get; set; }

        [DefaultValue(40)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloAffliction")]
        [DisplayName("Drain Life")]
        [Description("Tells on which Health % to use Drain Life")]
        [Percentage(true)]
        public int SoloAfflictionDrainlife { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloAffliction")]
        [DisplayName("Shadowbolt")]
        [Description("should Shadowbolt ignore Wand Treshhold?")]
        public bool SoloAfflictionShadowboltWand { get; set; }

        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloAffliction")]
        [DisplayName("Health Funnel Pet")]
        [Description("Tells on which PetHealth % to use Health Funnel")]
        [Percentage(true)]
        public int SoloAfflictionHealthfunnelPet { get; set; }

        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloAffliction")]
        [DisplayName("Health Funnel Player")]
        [Description("Tells until which PlayerHealth % to use Health Funnel")]
        [Percentage(true)]
        public int SoloAfflictionHealthfunnelMe { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloAffliction")]
        [DisplayName("Curse of")]
        [Description("Which Curse you want?")]
        [DropdownList(new string[] { "Agony", "Doom", "Elements", "Tongues", "Weakness", "Exhaustion" })]
        public string SoloAfflictionAfflCurse { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloAffliction")]
        [DisplayName("Use Seed of Corruption")]
        [Description("Make use of SoC while in Group?")]
        public bool SoloAfflictionUseSeedGroup { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloAffliction")]
        [DisplayName("Use Corruption on Multidot")]
        [Description("Make use of Corruption while in Group on multiple Enemies?")]
        public bool SoloAfflictionUseCorruptionGroup { get; set; }

        [DefaultValue(4)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloAffliction")]
        [DisplayName("Use AOE Count")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int SoloAfflictionAOECount { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloAffliction")]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool SoloAfflictionUseAOE { get; set; }

        //Rotation SoloDemonology

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloDemonology")]
        [DisplayName("Metamorphosis")]
        [Description("When to use  Metamorphosis?")]
        [DropdownList(new string[] { "OnCooldown", "OnBosses", "None" })]
        public string SoloDemonologyMetamorphosis { get; set; }

        [DefaultValue(20)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloDemonology")]
        [DisplayName("Lifetap")]
        [Description("Tells on which Mana % to use Lifetap")]
        [Percentage(true)]
        public int SoloDemonologyLifetap { get; set; }

        [DefaultValue(40)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloDemonology")]
        [DisplayName("Drain Life")]
        [Description("Tells on which Health % to use Drain Life")]
        [Percentage(true)]
        public int SoloDemonologyDrainlife { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloDemonology")]
        [DisplayName("Shadowbolt")]
        [Description("should Shadowbolt ignore Wand Treshhold?")]
        public bool SoloDemonologyShadowboltWand { get; set; }

        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloDemonology")]
        [DisplayName("Health Funnel Pet")]
        [Description("Tells on which PetHealth % to use Health Funnel")]
        [Percentage(true)]
        public int SoloDemonologyHealthfunnelPet { get; set; }

        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloDemonology")]
        [DisplayName("Health Funnel Player")]
        [Description("Tells until which PlayerHealth % to use Health Funnel")]
        [Percentage(true)]
        public int SoloDemonologyHealthfunnelMe { get; set; }

        [DefaultValue(4)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloDemonology")]
        [DisplayName("Use AOE Count")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int SoloDemonologyAOECount { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloDemonology")]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool SoloDemonologyUseAOE { get; set; }

        //Rotation SoloDestruction

        [DefaultValue(4)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloDestruction")]
        [DisplayName("Use AOE Count")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int SoloDestructionAOECount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "SoloDestruction")]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE in Instance")]

        public bool SoloDestructionUseAOE { get; set; }

        //Rotation GroupAffliction
        [DefaultValue(20)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Lifetap")]
        [Description("Tells on which Mana % to use Lifetap")]
        [Percentage(true)]
        public int GroupAfflictionLifetap { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Glyph Life Tap")]
        [Description("Set this if you have the life tap Glyph")]
        public bool GroupAfflictionGlyphLifetap { get; set; }

        [DefaultValue(40)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Drain Life")]
        [Description("Tells on which Health % to use Drain Life")]
        [Percentage(true)]
        public int GroupAfflictionDrainlife { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Shadowbolt")]
        [Description("should Shadowbolt ignore Wand Treshhold?")]
        public bool GroupAfflictionShadowboltWand { get; set; }

        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Health Funnel Pet")]
        [Description("Tells on which PetHealth % to use Health Funnel")]
        [Percentage(true)]
        public int GroupAfflictionHealthfunnelPet { get; set; }

        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Health Funnel Player")]
        [Description("Tells until which PlayerHealth % to use Health Funnel")]
        [Percentage(true)]
        public int GroupAfflictionHealthfunnelMe { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Curse of")]
        [Description("Which Curse you want?")]
        [DropdownList(new string[] { "Agony", "Doom", "Elements", "Tongues", "Weakness", "Exhaustion" })]
        public string GroupAfflictionAfflCurse { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Use Seed of Corruption")]
        [Description("Make use of SoC while in Group?")]
        public bool GroupAfflictionUseSeedGroup { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Use Corruption on Multidot")]
        [Description("Make use of Corruption while in Group on multiple Enemies?")]
        public bool GroupAfflictionUseCorruptionGroup { get; set; }

        [DefaultValue(4)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Use AOE Count")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int GroupAfflictionAOECount { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", "GroupAffliction")]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool GroupAfflictionUseAOE { get; set; }


        public WarlockLevelSettings()
        {
            ChooseTalent = "WarlockAffliction";
            //Pet
            Pet = "Voidwalker";
            PetInfight = true;
            //General
            Healthstone = true;
            UseWandTresh = 20;
            UseWand = true;
            Buffing = true;
            Soulshards = true;
            GlyphLifeTap = false;
            LifeTapOOC = true;

            //Rotation SoloAffliction
            SoloAfflictionUseSeedGroup = true;
            SoloAfflictionUseCorruptionGroup = true;
            SoloAfflictionLifetap = 20;
            SoloAfflictionDrainlife = 40;
            SoloAfflictionHealthfunnelPet = 30;
            SoloAfflictionHealthfunnelMe = 50;
            SoloAfflictionUseAOE = false;
            SoloAfflictionAOECount = 4;
            SoloAfflictionShadowboltWand = true;
            SoloAfflictionAfflCurse = "Agony";

            //Rotation SoloDemonology
            SoloDemonologyLifetap = 20;
            SoloDemonologyDrainlife = 40;
            SoloDemonologyHealthfunnelPet = 30;
            SoloDemonologyHealthfunnelMe = 50;
            SoloDemonologyUseAOE = false;
            SoloDemonologyAOECount = 4;
            SoloDemonologyShadowboltWand = true;
            SoloDemonologyMetamorphosis = "OnCooldown";

            //Rotation GroupAffliction
            GroupAfflictionUseSeedGroup = true;
            GroupAfflictionUseCorruptionGroup = true;
            GroupAfflictionLifetap = 20;
            GroupAfflictionGlyphLifetap = false;
            GroupAfflictionDrainlife = 40;
            GroupAfflictionHealthfunnelPet = 30;
            GroupAfflictionHealthfunnelMe = 50;
            GroupAfflictionUseAOE = false;
            GroupAfflictionAOECount = 4;
            GroupAfflictionShadowboltWand = true;
            GroupAfflictionAfflCurse = "Agony";

            //Rotation SoloDestruction
            SoloDestructionUseAOE = true;
            SoloDestructionAOECount = 4;
        }
    }
}

