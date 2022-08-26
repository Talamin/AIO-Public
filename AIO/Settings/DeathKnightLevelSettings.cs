using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class DeathKnightLevelSettings : BasePersistentSettings<DeathKnightLevelSettings>
    {

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Raise Dead")]
        [Description("Use Raise Dead asap?")]
        public bool RaiseDead { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Fight")]
        [DisplayName("Glyph Raise Dead")]
        [Description("Have  Glyph and don´t need Dust?")]
        public bool GlyphRaiseDead { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Dark Command")]
        [Description("Use Dark Command in Group?")]
        public bool DarkCommand { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Deathgrip")]
        [Description("use Deathgrip for in Group?")]
        public bool DeathGrip { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Fight")]
        [DisplayName("Rune tap")]
        [Description("Which health % to use Rune tap?")]
        [Percentage(true)]
        public int RuneTap { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Fight")]
        [DisplayName("Choose Presence")]
        [Description("Set the Presence you want the FC to fight in")]
        [DropdownList(new string[] { "BloodPresence", "FrostPresence", "UnholyPresence" })]
        public string Presence { get; set; }

        [Setting]
        [DefaultValue(1)]
        [Category("Fight")]
        [DisplayName("Bloodstrike")]
        [Description("Set Enemy Count Equal X enemy to use Bloodstrike")]
        [Percentage(false)]
        public int BloodStrike { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Fight")]
        [DisplayName("Hearthstrike")]
        [Description("Set Enemy Count Equal X enemy to use Hearthstrike")]
        [Percentage(false)]
        public int HearthStrike { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Fight")]
        [DisplayName("BloodBoil")]
        [Description("Set Enemy Count larger X enemy to use Bloodboil")]
        [Percentage(false)]
        public int BloodBoil { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Fight")]
        [DisplayName("Death and Decay")]
        [Description("Set Enemy Count larger X enemy to use DnD")]
        [Percentage(false)]
        public int DnD { get; set; }

        [DropdownList(new string[] { "DeathKnightBlood", "DeathKnightFrost", "DeathKnightUnholy" })]
        public override string ChooseTalent { get; set; }

        [DropdownList(new string[] { "Auto", "Blood", "Frost", "Unholy", "UnholyPVP" })]
        public override string ChooseRotation { get; set; }

        public DeathKnightLevelSettings()
        {
            RaiseDead = true;
            GlyphRaiseDead = false;
            ChooseTalent = "DeathKnightBlood";
            DarkCommand = true;
            DeathGrip = true;
            RuneTap = 50;
            Presence = "BloodPresence";
            BloodStrike = 1;
            HearthStrike = 2;
            BloodBoil = 2;
            DnD = 3;
        }
    }
}