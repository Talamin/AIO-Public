using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class ShamanLevelSettings : BasePersistentSettings<ShamanLevelSettings>
    {
        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Heal OOC")]
        [Description("Use Healspells Out of Combat?")]
        public bool HealOOC { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Cure Toxin")]
        [Description("Use Cure Toxin on which  Targets? ")]
        [DropdownList(new string[] { "Group", "Me", "None" })]
        public string CureToxin { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Elemental")]
        [DisplayName("Chainlightning Count")]
        [Description("Enemy Count to use  Treshhold?")]
        public int ElementalChainlightningTresshold { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Elemental")]
        [DisplayName("Cure Toxin")]
        [Description("Use on Groupmembers??")]
        public bool ElementalCureToxin { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Elemental")]
        [DisplayName("Flame Shock")]
        [Description("Use Flame Shock in Rotation??")]
        public bool ElementalFlameShock { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Elemental")]
        [DisplayName("Earth Shock")]
        [Description("Use Earth Shock in Rotation?")]
        public bool ElementalEarthShock { get; set; }

        [Setting]
        [DefaultValue(5)]
        [Category("Enhancement")]
        [DisplayName("Fire  Nova")]
        [Description("Use Fire Nova?")]
        public int EnhFireNova { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Enhancement")]
        [DisplayName("Ghostwolf")]
        [Description("Use Ghostwolfform?")]
        public bool Ghostwolf { get; set; }

        [Setting]
        [DefaultValue(10)]
        [Category("Enhancement")]
        [DisplayName("Selfheal")]
        [Description("Set the Enemytreshold in % when to heal?")]
        [Percentage(true)]
        public int EnhancementEnemylife { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Enhancement")]
        [DisplayName("HealthTreshhold")]
        [Description("Set the Treshhold of your health for selfheal?")]
        [Percentage(true)]
        public int EnhancementHealthForHeals { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Enhancement")]
        [DisplayName("ManaTreshhold")]
        [Description("Setthe Treshhold for offensive spells to save mana for heals?")]
        [Percentage(true)]
        public int EnhancementManaSavedForHeals { get; set; }


        [Setting]
        [DefaultValue(false)]
        [Category("Enhancement")]
        [DisplayName("Lightning Bolt")]
        [Description("Use LNB for Pull?")]
        public bool LNB { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Enhancement")]
        [DisplayName("Feral Spirit")]
        [Description("Use Feral Spirit on which  Targets? ")]
        [DropdownList(new string[] { "+2 and Elite", "+3 and Elite", "only Elite", "None" })]
        public string EnhancementFeralSpirit { get; set; }

        [Setting]
        [DefaultValue(99)]
        [Category("Restoration")]
        [DisplayName("Earthshield")]
        [Description("Set the Tank Treshhold for Earthshield?")]
        [Percentage(true)]
        public int RestorationEarthshieldTank { get; set; }

        [Setting]
        [DefaultValue(75)]
        [Category("Restoration")]
        [DisplayName("Riptide")]
        [Description("Set the Treshhold for Riptide usage?")]
        [Percentage(true)]
        public int RestorationRiptideGroup { get; set; }

        [Setting]
        [DefaultValue(85)]
        [Category("Restoration")]
        [DisplayName("Chain Heal / Health")]
        [Description("Set the Player Treshhold for Chain Heal?")]
        [Percentage(true)]
        public int RestorationChainHealGroup { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Restoration")]
        [DisplayName("Chain Heal / Player")]
        [Description("Set the PlayerCount Treshhold for Chain Heal (more then x  Player) ?")]
        [Percentage(false)]
        public int RestorationChainHealCountGroup { get; set; }

        [Setting]
        [DefaultValue(70)]
        [Category("Restoration")]
        [DisplayName("Healing Wave")]
        [Description("Set the Player Treshhold for Healing Wave?")]
        [Percentage(true)]
        public int RestorationHealingWaveGroup { get; set; }

        [Setting]
        [DefaultValue(85)]
        [Category("Restoration")]
        [DisplayName("Lesser Healing Wave")]
        [Description("Set the Player Treshhold for Lesser Healing Wave?")]
        [Percentage(true)]
        public int RestorationLesserHealingWaveGroup { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Totem")]
        [DisplayName("CotE")]
        [Description("Use CotE?")]
        public bool UseCotE { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Totem")]
        [DisplayName("Totemic Recall")]
        [Description("Use Totemic Recall?")]
        public bool UseTotemicCall { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Totem")]
        [DisplayName("Fire Nova")]
        [Description("Use Fire Nova?")]
        public bool UseFireNova { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Totem")]
        [DisplayName("Searing Totem")]
        [Description("Use Searing Totem?")]
        public bool UseSearingTotem { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Totem")]
        [DisplayName("Cleansing Totem")]
        [Description("Use Cleansing Totem?")]
        public bool UseCleansingTotem { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Totem")]
        [DisplayName("Grounding Totem")]
        [Description("Use Grounding Totem?")]
        public bool UseGroundingTotem { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Totem")]
        [DisplayName("Earthbind Totem")]
        [Description("Use Earthbind Totem?")]
        public bool UseEarthbindTotem { get; set; }



        [DropdownList(new string[] { "ShamanEnhancement", "ShamanRestoration", "ShamanElemental" })]
        public override string ChooseTalent { get; set; }

        [DropdownList(new string[] { "Auto", "Enhancement", "Restoration", "Elemental" })]
        public override string ChooseRotation { get; set; }

        public ShamanLevelSettings()
        {
            ChooseTalent = "ShamanEnhancement";
            HealOOC = true;
            Ghostwolf = true;
            EnhancementEnemylife = 10;
            LNB = false;
            EnhFireNova = 5;
            CureToxin = "None";
            ElementalCureToxin = false;
            ElementalChainlightningTresshold = 3;
            ElementalEarthShock = true;
            ElementalFlameShock = true;
            EnhancementManaSavedForHeals = 0;
            EnhancementHealthForHeals = 50;
            EnhancementFeralSpirit = "+2 and Elite";
            RestorationEarthshieldTank = 99;
            RestorationChainHealGroup = 85;
            RestorationChainHealCountGroup = 2;
            RestorationHealingWaveGroup = 70;
            RestorationLesserHealingWaveGroup = 85;
            RestorationRiptideGroup = 75;
            UseTotemicCall = true;
            UseFireNova = true;
            UseSearingTotem = false;
            UseCleansingTotem = true;
            UseGroundingTotem = true;
            UseEarthbindTotem = false;

            UseCotE = true;
        }
    }
}