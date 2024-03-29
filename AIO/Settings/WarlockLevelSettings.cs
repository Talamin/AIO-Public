﻿using AIO.Lists;
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
        [TriggerDropdown("WarlockTriggerDropdown",new string[] { nameof(Spec.Warlock_SoloDestruction), nameof(Spec.Warlock_SoloAffliction), nameof(Spec.Warlock_SoloDemonology), nameof(Spec.Warlock_GroupAffliction) })]
        public override string ChooseRotation { get; set; }

        //Pet
        [Setting]
        [DefaultValue(true)]
        [Category("Pet")]
        [DisplayName("Summon pet in fight")]
        [Description("Resummon your pet during fights")]
        public bool ReSummonPetInfight { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Pet")]
        [DisplayName("Pet")]
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
        /*
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
        */
        [Setting]
        [DefaultValue("On self")]
        [Category("General")]
        [DisplayName("Soulstone")]
        [Description("Choose the target for your soulstone")]
        [DropdownList(new string[] {"Tank", "Healer", "On self", "None"})]
        public string GeneralSoulstoneTarget { get; set; }

        //Rotation SoloAffliction
        [DefaultValue(20)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloAffliction))]
        [DisplayName("Lifetap")]
        [Description("Tells on which Mana % to use Lifetap")]
        [Percentage(true)]
        public int SoloAfflictionLifetap { get; set; }

        [DefaultValue(40)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloAffliction))]
        [DisplayName("Drain Life")]
        [Description("Tells on which Health % to use Drain Life")]
        [Percentage(true)]
        public int SoloAfflictionDrainlife { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloAffliction))]
        [DisplayName("Shadowbolt")]
        [Description("should Shadowbolt ignore Wand Treshhold?")]
        public bool SoloAfflictionShadowboltWand { get; set; }

        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloAffliction))]
        [DisplayName("Health Funnel Pet")]
        [Description("Tells on which PetHealth % to use Health Funnel")]
        [Percentage(true)]
        public int SoloAfflictionHealthfunnelPet { get; set; }

        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloAffliction))]
        [DisplayName("Health Funnel Player")]
        [Description("Tells until which PlayerHealth % to use Health Funnel")]
        [Percentage(true)]
        public int SoloAfflictionHealthfunnelMe { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloAffliction))]
        [DisplayName("Curse of")]
        [Description("Which Curse you want?")]
        [DropdownList(new string[] { "Agony", "Doom", "Elements", "Tongues", "Weakness", "Exhaustion" })]
        public string SoloAfflictionAfflCurse { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloAffliction))]
        [DisplayName("Use Seed of Corruption")]
        [Description("Make use of SoC while in Group?")]
        public bool SoloAfflictionUseSeedGroup { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloAffliction))]
        [DisplayName("Use Corruption on Multidot")]
        [Description("Make use of Corruption while in Group on multiple Enemies?")]
        public bool SoloAfflictionUseCorruptionGroup { get; set; }

        [DefaultValue(4)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloAffliction))]
        [DisplayName("Use AOE Count")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int SoloAfflictionAOECount { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloAffliction))]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool SoloAfflictionUseAOE { get; set; }

        //Rotation SoloDemonology

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDemonology))]
        [DisplayName("Metamorphosis")]
        [Description("When to use  Metamorphosis?")]
        [DropdownList(new string[] { "OnCooldown", "OnBosses", "None" })]
        public string SoloDemonologyMetamorphosis { get; set; }

        [DefaultValue(20)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDemonology))]
        [DisplayName("Lifetap")]
        [Description("Tells on which Mana % to use Lifetap")]
        [Percentage(true)]
        public int SoloDemonologyLifetap { get; set; }

        [DefaultValue(40)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDemonology))]
        [DisplayName("Drain Life")]
        [Description("Tells on which Health % to use Drain Life")]
        [Percentage(true)]
        public int SoloDemonologyDrainlife { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDemonology))]
        [DisplayName("Shadowbolt")]
        [Description("should Shadowbolt ignore Wand Treshhold?")]
        public bool SoloDemonologyShadowboltWand { get; set; }

        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDemonology))]
        [DisplayName("Health Funnel Pet")]
        [Description("Tells on which PetHealth % to use Health Funnel")]
        [Percentage(true)]
        public int SoloDemonologyHealthfunnelPet { get; set; }

        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDemonology))]
        [DisplayName("Health Funnel Player")]
        [Description("Tells until which PlayerHealth % to use Health Funnel")]
        [Percentage(true)]
        public int SoloDemonologyHealthfunnelMe { get; set; }

        [DefaultValue(4)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDemonology))]
        [DisplayName("Use AOE Count")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int SoloDemonologyAOECount { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDemonology))]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool SoloDemonologyUseAOE { get; set; }

        //Rotation SoloDestruction

        [DefaultValue(4)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDestruction))]
        [DisplayName("Use AOE Count")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int SoloDestructionAOECount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDestruction))]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE in Instance")]

        public bool SoloDestructionUseAOE { get; set; }

        //Rotation GroupDestruction

        [DefaultValue(4)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDestruction))]
        [DisplayName("Use AOE Count")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int GroupDestructionAOECount { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_SoloDestruction))]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE in Instance")]

        public bool GroupDestructionUseAOE { get; set; }

        //Rotation GroupAffliction
        [DefaultValue(20)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Lifetap")]
        [Description("Tells on which Mana % to use Lifetap")]
        [Percentage(true)]
        public int GroupAfflictionLifetap { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Glyph Life Tap")]
        [Description("Set this if you have the life tap Glyph")]
        public bool GroupAfflictionGlyphLifetap { get; set; }

        [DefaultValue(40)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Drain Life")]
        [Description("Tells on which Health % to use Drain Life")]
        [Percentage(true)]
        public int GroupAfflictionDrainlife { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Shadowbolt over wand")]
        [Description("Should Shadowbolt ignore Wand Treshhold")]
        public bool GroupAfflictionShadowboltOverWand { get; set; }

        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Health Funnel Pet")]
        [Description("Tells on which PetHealth % to use Health Funnel")]
        [Percentage(true)]
        public int GroupAfflictionHealthfunnelPet { get; set; }

        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Health Funnel Player")]
        [Description("Tells until which PlayerHealth % to use Health Funnel")]
        [Percentage(true)]
        public int GroupAfflictionHealthfunnelMe { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Curse of")]
        [Description("Which Curse you want?")]
        [DropdownList(new string[] { "Agony", "Doom", "Elements", "Tongues", "Weakness", "Exhaustion" })]
        public string GroupAfflictionAfflCurse { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Use Seed of Corruption")]
        [Description("Make use of SoC while in Group?")]
        public bool GroupAfflictionUseSeedGroup { get; set; }
        /*
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Use Corruption on Multidot")]
        [Description("Make use of Corruption while in Group on multiple Enemies?")]
        public bool GroupAfflictionUseCorruptionGroup { get; set; }
        */
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Spread Corruption")]
        [Description("Cast Corruption on multiple Enemies")]
        public bool GroupAfflictionSpreadCorruption { get; set; }
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Spread Unstable Affliction")]
        [Description("Cast Unstable Affliction on multiple Enemies")]
        public bool GroupAfflictionSpreadUnstableAffliction { get; set; }
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Spread Curse of Agony")]
        [Description("Cast Curse of Agony on multiple Enemies")]
        public bool GroupAfflictionSpreadCurseOfAgony { get; set; }
        [DefaultValue(4)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Use AOE Count")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int GroupAfflictionAOECount { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarlockTriggerDropdown", nameof(Spec.Warlock_GroupAffliction))]
        [DisplayName("Use AOE")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool GroupAfflictionUseAOE { get; set; }


        public WarlockLevelSettings()
        {
            //Pet
            Pet = "Voidwalker";
            ReSummonPetInfight = true;
            //General
            //Healthstone = true;
            UseWandTresh = 20;
            UseWand = true;
            Buffing = true;
            //Soulshards = true;
            GlyphLifeTap = false;
            LifeTapOOC = true;
            GeneralSoulstoneTarget = "On self";

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
            //GroupAfflictionUseCorruptionGroup = true;
            GroupAfflictionSpreadCorruption = true;
            GroupAfflictionSpreadCurseOfAgony = true;
            GroupAfflictionSpreadUnstableAffliction = true;
            GroupAfflictionLifetap = 20;
            GroupAfflictionGlyphLifetap = false;
            GroupAfflictionDrainlife = 40;
            GroupAfflictionHealthfunnelPet = 30;
            GroupAfflictionHealthfunnelMe = 50;
            GroupAfflictionUseAOE = false;
            GroupAfflictionAOECount = 4;
            GroupAfflictionShadowboltOverWand = true;
            GroupAfflictionAfflCurse = "Agony";

            //Rotation SoloDestruction
            SoloDestructionUseAOE = true;
            SoloDestructionAOECount = 4;

            //Rotation GroupDestruction
            GroupDestructionUseAOE = true;
            GroupDestructionAOECount = 4;
        }
    }
}

