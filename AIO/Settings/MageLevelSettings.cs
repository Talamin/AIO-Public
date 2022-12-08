using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class MageLevelSettings : BasePersistentSettings<MageLevelSettings>
    {
        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Backpaddle")]
        [Description("Auto Backpaddle?")]
        public bool Backpaddle { get; set; }

        [Setting]
        [DefaultValue(20)]
        [Category("Fight")]
        [DisplayName("BackpaddleRange")]
        [Description("Set your Range for your FC Backpaddle")]
        public int BackpaddleRange { get; set; }

        [Setting]
        [DefaultValue(10)]
        [Category("Fight")]
        [DisplayName("Manastone")]
        [Description("Treshhold for Manastone")]
        public int Manastone { get; set; }

        [Setting]
        [DefaultValue(10)]
        [Category("Fight")]
        [DisplayName("Fire Blast")]
        [Description("Treshhold for Enemy Health <= to use Fire Blast")]
        [Percentage(true)]
        public int FrostFireBlast { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Fight")]
        [DisplayName("Sheep")]
        [Description("Uses Sheep if 2 Targets attacking")]
        public bool Sheep { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Fight")]
        [DisplayName("Use AOE in Instance")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool UseAOE { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Fight")]
        [DisplayName("AOE in Instance")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int AOEInstance { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Use Wand?")]
        [Description("Use Wand in General?")]
        public bool UseWand { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Use Blink?")]
        [Description("Use Blink while Backpaddle?")]
        public bool Blink { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fire")]
        [DisplayName("Use Flamestrike?")]
        [Description("Use Flamestrike without Firestarter Buff?")]
        public bool FlamestrikeWithoutFire { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Fire")]
        [DisplayName("Flamestrike EnemyCount")]
        [Description("Number of Targets around the Tank to use FS in Instance")]
        [Percentage(false)]
        public int FlamestrikeWithoutCountFire { get; set; }

        [Setting]
        [DefaultValue(20)]
        [Category("General")]
        [DisplayName("Use Wand Treshold?")]
        [Description("Enemy Life Treshold for Wandusage?")]
        [Percentage(true)]
        public int UseWandTresh { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Glyph of Evocation?")]
        [Description("Using Glyph of Evocation? Turns Evocation into a health % cooldown.")]
        public bool GlyphOfEvocation { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Glyph of Eternal Water?")]
        [Description("Will use Water Elemental as a regular pet, if glyphed.")]
        public bool GlyphOfEternalWater { get; set; }

        [DropdownList(new string[] { "MageFrost", "MageFire", "MageArcane" })]
        public override string ChooseTalent { get; set; }

        [DropdownList(new string[] { "Auto", "Frost","GroupFrost", "Fire", "Arcane" })]
        public override string ChooseRotation { get; set; }

        public MageLevelSettings()
        {
            ChooseTalent = "MageFrost";
            FrostFireBlast = 10;
            Sheep = false;
            Manastone = 10;
            UseAOE = false;
            AOEInstance = 3;
            Blink = true;
            FlamestrikeWithoutFire = true;
            FlamestrikeWithoutCountFire = 3;
            UseWand = true;
            UseWandTresh = 20;
            GlyphOfEvocation = false;
            GlyphOfEternalWater = false;
            Backpaddle = true;
            BackpaddleRange = 20;
        }
    }
}