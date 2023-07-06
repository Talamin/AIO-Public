﻿using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class ShamanLevelSettings : BasePersistentSettings<ShamanLevelSettings>
    {
        #region Selectors
        [DropdownList(new string[] { "ShamanEnhancement", "ShamanRestoration", "ShamanElemental" })]
        public override string ChooseTalent { get; set; }

        [TriggerDropdown("ShamanTriggerDropdown", new string[] { "Auto", "SoloEnhancement", "SoloRestoration", "SoloElemental" })]
        public override string ChooseRotation { get; set; }
        #endregion

        #region General Settings for all specs
        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Heal OOC")]
        [Description("Use Healing spells when out of combat?")]
        public bool HealOOC { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Cure Toxin")]
        [Description("Valid targets for Cure Toxin")]
        [DropdownList(new string[] { "Group", "Me", "None" })]
        public string CureToxin { get; set; }
        
        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Ghost Wolf")]
        [Description("Use Ghost Wolf form?")]
        public bool UseGhostWolf { get; set; }
        #endregion

        #region Rotation settings
        #region Elemental
        [Setting]
        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloElemental")]                
        [DisplayName("Chain Lightning Count")]
        [Description("Required number of enemies to use Chain Lightning")]
        public int SoloElementalChainlightningTresshold { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloElemental")]        
        [DisplayName("Cure Toxin")]
        [Description("Use on Groupmembers??")]
        public bool SoloElementalCureToxin { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloElemental")]
        [DisplayName("Flame Shock")]
        [Description("Use Flame Shock in Rotation??")]
        public bool SoloElementalFlameShock { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloElemental")]
        [DisplayName("Earth Shock")]
        [Description("Use Earth Shock in Rotation?")]
        public bool SoloElementalEarthShock { get; set; }
        #endregion
        #region Enhancement
        [Setting]
        [DefaultValue(5)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloEnhancement")]        
        [DisplayName("Fire  Nova")]
        [Description("Use Fire Nova?")]
        public int SoloEnhancementUseFireNova { get; set; }        

        [Setting]
        [DefaultValue(10)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloEnhancement")]
        [DisplayName("Self heal min enemy HP")]
        [Description("The min Enemy HP at which to stop healing")]
        [Percentage(true)]
        public int SoloEnhancementEnemyHPSkipHealing { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloEnhancement")]
        [DisplayName("HealthTreshhold")]
        [Description("Set the HP treshhold for self healing?")]
        [Percentage(true)]
        public int SoloEnhancementHealthForHeals { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloEnhancement")]
        [DisplayName("Reserve Healing Mana")]
        [Description("Set the Treshhold for offensive spells to save mana for heals?")]
        [Percentage(true)]
        public int SoloEnhancementManaSavedForHeals { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloEnhancement")]
        [DisplayName("Feral Spirit")]
        [Description("Use Feral Spirit on which  Targets? ")]
        [DropdownList(new string[] { "+2 and Elite", "+3 and Elite", "only Elite", "None" })]
        public string SoloEnhancementFeralSpirit { get; set; }
        #endregion
        #region Restoration
        [Setting]
        [DefaultValue(99)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloRestoration")]        
        [DisplayName("Earthshield")]
        [Description("Set the Tank Treshhold for Earthshield?")]
        [Percentage(true)]
        public int RestorationEarthshieldTank { get; set; }

        [Setting]
        [DefaultValue(75)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloRestoration")]
        [DisplayName("Riptide")]
        [Description("Set the Treshhold for Riptide usage?")]
        [Percentage(true)]
        public int RestorationRiptideGroup { get; set; }

        [Setting]
        [DefaultValue(85)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloRestoration")]
        [DisplayName("Chain Heal / Health")]
        [Description("Set the Player Treshhold for Chain Heal?")]
        [Percentage(true)]
        public int RestorationChainHealGroup { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloRestoration")]
        [DisplayName("Chain Heal / Player")]
        [Description("Set the PlayerCount Treshhold for Chain Heal (more then x  Player) ?")]
        [Percentage(false)]
        public int RestorationChainHealCountGroup { get; set; }

        [Setting]
        [DefaultValue(70)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloRestoration")]
        [DisplayName("Healing Wave")]
        [Description("Set the Player Treshhold for Healing Wave?")]
        [Percentage(true)]
        public int RestorationHealingWaveGroup { get; set; }

        [Setting]
        [DefaultValue(85)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("ShamanTriggerDropdown", "SoloRestoration")]
        [DisplayName("Lesser Healing Wave")]
        [Description("Set the Player Treshhold for Lesser Healing Wave?")]
        [Percentage(true)]
        public int RestorationLesserHealingWaveGroup { get; set; }
        #endregion
        #endregion

        #region Totem Settings for all specs

        [Setting]
        [DefaultValue(false)]
        [Category("Totem")]
        [DisplayName("Earthbind Totem")]
        [Description("Use Earthbind Totem?")]
        public bool UseEarthbindTotem { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Totem")]
        [DisplayName("Searing Totem")]
        [Description("Use Searing Totem?")]
        public bool UseSearingTotem { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Totem")]
        [DisplayName("Fire Nova")]
        [Description("Use Fire Nova?")]
        public bool UseFireNova { get; set; }

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
        [DisplayName("Air Totem")]
        [Description("Use Air Totem inside Call of the Elements?")]
        public bool UseAirTotemInCotE { get; set; }


        #endregion

        public ShamanLevelSettings()
        {
            ChooseTalent = "ShamanEnhancement";
            HealOOC = true;
            UseGhostWolf = true;
            SoloEnhancementEnemyHPSkipHealing = 10;
            SoloEnhancementUseFireNova = 5;
            CureToxin = "None";
            SoloElementalCureToxin = false;
            SoloElementalChainlightningTresshold = 3;
            SoloElementalEarthShock = true;
            SoloElementalFlameShock = true;
            SoloEnhancementManaSavedForHeals = 0;
            SoloEnhancementHealthForHeals = 50;
            SoloEnhancementFeralSpirit = "+2 and Elite";
            RestorationEarthshieldTank = 99;
            RestorationChainHealGroup = 85;
            RestorationChainHealCountGroup = 2;
            RestorationHealingWaveGroup = 70;
            RestorationLesserHealingWaveGroup = 85;
            RestorationRiptideGroup = 75;
            UseEarthbindTotem = false;
            UseSearingTotem = false;
            UseFireNova = true;
            UseCleansingTotem = true;
            UseGroundingTotem = true;
            UseCotE = true;
            UseTotemicCall = true;
            UseAirTotemInCotE = true;
        }
    }
}