using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class MageLevelSettings : BasePersistentSettings<MageLevelSettings>
    {
        //Lists


        [DropdownList(new string[] { "MageFrost", "MageFire", "MageArcane" })]
        public override string ChooseTalent { get; set; }

        [TriggerDropdown("MageTriggerDropdown", new string[] { "Auto", "SoloFrost", "GroupFrost", "SoloFire", "GroupFire", "SoloArcane" })]
        public override string ChooseRotation { get; set; }

        //General
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Backpaddle")]
        [Description("Auto Backpaddle?")]
        public bool Backpaddle { get; set; }

        [DefaultValue(20)]
        [Category("General")]
        [DisplayName("BackpaddleRange")]
        [Description("Set your Range for your FC Backpaddle")]
        public int BackpaddleRange { get; set; }

        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Use Blink?")]
        [Description("Use Blink while Backpaddle?")]
        public bool Blink { get; set; }

        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Use Wand?")]
        [Description("Use Wand in General?")]
        public bool UseWand { get; set; }


        [DefaultValue(20)]
        [Category("General")]
        [DisplayName("Use Wand Treshold?")]
        [Description("Enemy Life Treshold for Wandusage?")]
        [Percentage(true)]
        public int UseWandTresh { get; set; }

        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Glyph of Evocation?")]
        [Description("Using Glyph of Evocation? Turns Evocation into a health % cooldown.")]
        public bool GlyphOfEvocation { get; set; }

        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Glyph of Eternal Water?")]
        [Description("Will use Water Elemental as a regular pet, if glyphed.")]
        public bool GlyphOfEternalWater { get; set; }

        //Rotation SoloFrost


        [DefaultValue(10)][Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFrost")]
        [DisplayName("Manastone")][Description("Treshhold for Manastone")]
        public int SoloFrostManastone { get; set; }

        [DefaultValue(10)][Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFrost")]
        [DisplayName("Fire Blast")][Description("Treshhold for Enemy Health <= to use Fire Blast")]
        [Percentage(true)]
        public int SoloFrostFrostFireBlast { get; set; }

        [DefaultValue(false)][Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFrost")]
        [DisplayName("Sheep")][Description("Uses Sheep if 2 Targets attacking")]
        public bool SoloFrostSheep { get; set; }

        [DefaultValue(false)][Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFrost")]
        [DisplayName("Use AOE in Instance")][Description("Set this if you want to use AOE in Instance")]
        public bool SoloFrostUseAOE { get; set; }

        [DefaultValue(4)][Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFrost")]
        [DisplayName("AOE in Instance")][Description("Number of Targets around the Tank to use AOE in Instance")][Percentage(false)]
        public int SoloFrostAOEInstance { get; set; }

        //Rotation GroupFrost


        [DefaultValue(10)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFrost")]
        [DisplayName("Fire Blast")]
        [Description("Treshhold for Enemy Health <= to use Fire Blast")]
        [Percentage(true)]
        public int GroupFrostFrostFireBlast { get; set; }

        [DefaultValue(10)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFrost")]
        [DisplayName("Manastone")]
        [Description("Treshhold for Manastone")]
        public int GroupFrostManastone { get; set; }

        //[DefaultValue(false)]
        //[Category("Rotation")]
        //[VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFrost")]
        //[DisplayName("Sheep")]
        //[Description("Uses Sheep if 2 Targets attacking")]
        //public bool GroupFrostSheep { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFrost")]
        [DisplayName("Use AOE in Instance")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool GroupFrostUseAOE { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFrost")]
        [DisplayName("AOE in Instance")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int GroupFrostAOEInstance { get; set; }

        //Rotation SoloFire

        [DefaultValue(10)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFire")]
        [DisplayName("Manastone")]
        [Description("Treshhold for Manastone")]
        public int SoloFireManastone { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFire")]
        [DisplayName("Sheep")]
        [Description("Uses Sheep if 2 Targets attacking")]
        public bool SoloFireSheep { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFire")]
        [DisplayName("Use AOE in Instance")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool SoloFireUseAOE { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFire")]
        [DisplayName("AOE in Instance")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int SoloFireAOEInstance { get; set; }


        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFire")]
        [DisplayName("Use Flamestrike?")]
        [Description("Use Flamestrike without Firestarter Buff?")]
        public bool SoloFireFlamestrikeWithoutFire { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloFire")]
        [DisplayName("Flamestrike EnemyCount")]
        [Description("Number of Targets around the Tank to use FS in Instance")]
        [Percentage(false)]
        public int SoloFireFlamestrikeWithoutCountFire { get; set; }

        //Rotation GroupFire

        [DefaultValue(10)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFire")]
        [DisplayName("Mana gem")]
        [Description("Mana threshold when mage should use mana gem")]
        public int GroupFireManastone { get; set; }

        //[DefaultValue(false)]
        //[Category("Rotation")]
        //[VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFire")]
        //[DisplayName("Sheep")]
        //[Description("Uses Sheep if 2 Targets attacking")]
        //public bool GroupFireSheep { get; set; }

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFire")]
        [DisplayName("Use AOE in Instance")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool GroupFireUseAOE { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFire")]
        [DisplayName("AOE in Instance")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int GroupFireAOEInstance { get; set; }


        [DefaultValue(true)][Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFire")]
        [DisplayName("Use Flamestrike?")][Description("Use Flamestrike without Firestarter Buff?")]
        public bool GroupFireFlamestrikeWithoutFire { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFire")]
        [DisplayName("Use Fire Blast?")]
        [Description("Check this if you want to finish the mobs using Fire Blast")]
        public bool GroupFireUseFireBlast { get; set; }

        [DefaultValue(3)][Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "GroupFire")]
        [DisplayName("Flamestrike EnemyCount")][Description("Number of Targets around the Tank to use FS in Instance")]
        [Percentage(false)]
        public int GroupFireFlamestrikeWithoutCountFire { get; set; }

        //Rotation SoloArcane

        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("MageTriggerDropdown", "SoloArcane")]
        [DisplayName("Sheep")]
        [Description("Uses Sheep if 2 Targets attacking")]
        public bool SoloArcaneSheep { get; set; }



        public MageLevelSettings()
        {
            ChooseTalent = "MageFrost";
            Backpaddle = true;
            BackpaddleRange = 20;
            Blink = true;
            UseWand = true;
            UseWandTresh = 20;
            GlyphOfEvocation = false;
            GlyphOfEternalWater = false;


            SoloFrostFrostFireBlast = 10;
            SoloFrostSheep = false;
            SoloFrostManastone = 10;
            SoloFrostUseAOE = false;
            SoloFrostAOEInstance = 4;

            //GroupFrostSheep = false;
            GroupFrostFrostFireBlast = 10;
            GroupFrostManastone = 10;
            GroupFrostUseAOE = false;
            GroupFrostAOEInstance = 4;

            SoloFireSheep = false;
            SoloFireManastone = 10;
            SoloFireUseAOE = false;
            SoloFireAOEInstance = 4;
            SoloFireFlamestrikeWithoutFire = true;
            SoloFireFlamestrikeWithoutCountFire = 3;

            //GroupFireSheep = false;
            GroupFireManastone = 10;
            GroupFireUseAOE = false;
            GroupFireAOEInstance = 4;
            GroupFireFlamestrikeWithoutFire = true;
            GroupFireFlamestrikeWithoutCountFire = 3;
            GroupFireUseFireBlast = true;

            SoloArcaneSheep = false;

        }
    }
}