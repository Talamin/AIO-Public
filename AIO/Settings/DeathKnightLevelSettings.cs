using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class DeathKnightLevelSettings : BasePersistentSettings<DeathKnightLevelSettings>
    {
        //Lists

        [DropdownList(new string[] { "DeathKnightBlood", "DeathKnightBloodTank", "DeathKnightFrost", "GroupDeathKnightFrost", "DeathKnightUnholy", "GroupDeathKnightUnholy" })]
        public override string ChooseTalent { get; set; }

        [TriggerDropdown("DeathKnightTriggerDropdown", new string[] { "Auto", "SoloBlood", "GroupBloodTank", "SoloFrost", "SoloUnholy", "UnholyPVP" })]
        public override string ChooseRotation { get; set; }

        //General
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Raise Dead")]
        [Description("Use Raise Dead asap?")]
        public bool RaiseDead { get; set; }

        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Glyph Raise Dead")]
        [Description("Have  Glyph and don´t need Dust?")]
        public bool GlyphRaiseDead { get; set; }

        [DefaultValue(false)]
        [Category("Fight")]
        [DisplayName("Choose Presence")]
        [Description("Set the Presence you want the FC to fight in")]
        [DropdownList(new string[] { "BloodPresence", "FrostPresence", "UnholyPresence" })]
        public string Presence { get; set; }

        //SoloBlood

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloBlood")]
        [DisplayName("Dark Command")]
        [Description("Use Dark Command in Group?")]
        public bool SoloBloodDarkCommand { get; set; }

        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloBlood")]
        [DisplayName("Deathgrip")]
        [Description("use Deathgrip for in Group?")]
        public bool SoloBloodDeathGrip { get; set; }

        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloBlood")]
        [DisplayName("Rune tap")]
        [Description("Which health % to use Rune tap?")]
        [Percentage(true)]
        public int SoloBloodRuneTap { get; set; }

        [DefaultValue(1)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloBlood")]
        [DisplayName("Bloodstrike")]
        [Description("Set Enemy Count Equal X enemy to use Bloodstrike")]
        [Percentage(false)]
        public int SoloBloodBloodStrike { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloBlood")]
        [DisplayName("Hearthstrike")]
        [Description("Set Enemy Count Equal X enemy to use Hearthstrike")]
        [Percentage(false)]
        public int SoloBloodHearthStrike { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloBlood")]
        [DisplayName("BloodBoil")]
        [Description("Set Enemy Count larger X enemy to use Bloodboil")]
        [Percentage(false)]
        public int SoloBloodBloodBoil { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloBlood")]
        [DisplayName("Death and Decay")]
        [Description("Set Enemy Count larger X enemy to use DnD")]
        [Percentage(false)]
        public int SoloBloodDnD { get; set; }

        //SoloFrost

        [DefaultValue(1)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloFrost")]
        [DisplayName("Bloodstrike")]
        [Description("Set Enemy Count Equal X enemy to use Bloodstrike")]
        [Percentage(false)]
        public int SoloFrostBloodStrike { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloFrost")]
        [DisplayName("Hearthstrike")]
        [Description("Set Enemy Count Equal X enemy to use Hearthstrike")]
        [Percentage(false)]
        public int SoloFrostHearthStrike { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloFrost")]
        [DisplayName("BloodBoil")]
        [Description("Set Enemy Count larger X enemy to use Bloodboil")]
        [Percentage(false)]
        public int SoloFrostBloodBoil { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloFrost")]
        [DisplayName("Death and Decay")]
        [Description("Set Enemy Count larger X enemy to use DnD")]
        [Percentage(false)]
        public int SoloFrostDnD { get; set; }

        //SoloUnholy

        [DefaultValue(1)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloUnholy")]
        [DisplayName("Bloodstrike")]
        [Description("Set Enemy Count Equal X enemy to use Bloodstrike")]
        [Percentage(false)]
        public int SoloUnholyBloodStrike { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloUnholy")]
        [DisplayName("Hearthstrike")]
        [Description("Set Enemy Count Equal X enemy to use Hearthstrike")]
        [Percentage(false)]
        public int SoloUnholyHearthStrike { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloUnholy")]
        [DisplayName("BloodBoil")]
        [Description("Set Enemy Count larger X enemy to use Bloodboil")]
        [Percentage(false)]
        public int SoloUnholyBloodBoil { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DeathKnightTriggerDropdown", "SoloUnholy")]
        [DisplayName("Death and Decay")]
        [Description("Set Enemy Count larger X enemy to use DnD")]
        [Percentage(false)]
        public int SoloUnholyDnD { get; set; }

        public DeathKnightLevelSettings()
        {
            ChooseTalent = "DeathKnightBlood";
            RaiseDead = true;
            GlyphRaiseDead = false;
            Presence = "BloodPresence";
            //SoloBlood
            SoloBloodDarkCommand = true;
            SoloBloodDeathGrip = true;
            SoloBloodRuneTap = 50;
            SoloBloodBloodStrike = 1;
            SoloBloodHearthStrike = 2;
            SoloBloodBloodBoil = 2;
            SoloBloodDnD = 3;
            //SoloFrost
            SoloFrostBloodStrike = 1;
            SoloFrostHearthStrike = 2;
            SoloFrostBloodBoil = 2;
            SoloFrostDnD = 3;
            //SoloUnholy
            SoloUnholyBloodStrike = 1;
            SoloUnholyHearthStrike = 2;
            SoloUnholyBloodBoil = 2;
            SoloUnholyDnD = 3;
        }
    }
}