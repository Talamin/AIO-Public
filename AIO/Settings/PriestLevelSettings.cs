using System;
using System.ComponentModel;
using System.Configuration;
using AIO.Lists;
using MarsSettingsGUI;

namespace AIO.Settings
{
    [Serializable]
    public sealed class PriestLevelSettings : BasePersistentSettings<PriestLevelSettings>
    {
        //Lists
        [TriggerDropdown("PriestTriggerDropdown", new string[] { nameof(Spec.Priest_GroupDiscipline), nameof(Spec.Priest_GroupHoly), nameof(Spec.Priest_SoloShadow), nameof(Spec.Priest_GroupShadow) })]
        public override string ChooseRotation { get; set; }

        #region General
        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Use Wand?")]
        [Description("Use Wand in General?")]
        public bool UseWand { get; set; }

        [Setting]
        [DefaultValue(20)]
        [Category("General")]
        [DisplayName("Use Wand Treshold?")]
        [Description("Life Treshold for Wandusage?")]
        [Percentage(true)]
        public int UseWandTresh { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Use standart Buffing (Solo)?")]
        [Description("Use Standart AutoBuffing in General?")]
        public bool UseAutoBuff { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Use intelligent Buffing (Group)?")]
        [Description("Use Intelligent Buffing in General?")]
        public bool UseAutoBuffInt { get; set; }
        #endregion

        #region GroupShadow
        [Setting]
        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupShadow))]
        [DisplayName("Shadowfiend")]
        [Description("Use Shadowfiend when under X% mana")]
        [Percentage(true)]
        public int GroupShadowShadowfiend { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupShadow))]
        [DisplayName("PW: Shield")]
        [Description("Use Power Word Shield when under X% health")]
        [Percentage(true)]
        public int GroupShadowUseShieldTresh { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupShadow))]
        [DisplayName("Dispersion")]
        [Description("Use Dispersion when under X% mana")]
        [Percentage(true)]
        public int GroupShadowDispersion { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupShadow))]
        [DisplayName("Cure Disease")]
        [Description("Cure party diseases")]
        public bool GroupShadowCureDisease { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupShadow))]
        [DisplayName("Dispel Magic")]
        [Description("Dispel party magic debuffs")]
        public bool GroupShadowDispelMagic { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupShadow))]
        [DisplayName("Spread SW: Pain")]
        [Description("Cast SW: Pain on all enemies in range")]
        public bool GroupShadowSpreadSWPain { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupShadow))]
        [DisplayName("Spread Vampiric Touch")]
        [Description("Cast Vampiric Touch on all enemies in range")]
        public bool GroupShadowSpreadVT { get; set; }

        #endregion

        #region SoloShadow
        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("ShadowForm?")]
        [Description("Use ShadowForm in General?")]
        public bool SoloShadowShadowForm { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("Shadowfiend")]
        [Description("% when use Shadowfiend in General?")]
        public int SoloShadowShadowfiend { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("Use Dot on Offtargets?")]
        [Description("Use Dot on Adds?")]
        public bool SoloShadowDotOff { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("Use Devouring Plague?")]
        [Description("Use DP up to level 80?")]
        public bool SoloShadowDPUse { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("Use Shield Treshold?")]
        [Description("Own life for Shield Usage?")]
        [Percentage(true)]
        public int SoloShadowUseShieldTresh { get; set; }

        [Setting]
        [DefaultValue(90)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("Use Renew Treshold?")]
        [Description("Treshold for Renew Usage?")]
        [Percentage(true)]
        public int SoloShadowUseRenewTresh { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("Use Heal Treshold?")]
        [Description("Treshold for Heal Usage?")]
        [Percentage(true)]
        public int SoloShadowUseHealTresh { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("Use Flash Heal Treshold?")]
        [Description("Treshold for Flash Heal Usage?")]
        [Percentage(true)]
        public int SoloShadowUseFlashTresh { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("Dispersion")]
        [Description("Treshold for Dispersion Usage?")]
        [Percentage(true)]
        public int SoloShadowDispersion { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("Use Mindflay?")]
        [Description("Use Mindflay in General?")]
        public bool SoloShadowUseMindflay { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("Use Heal in Group?")]
        [Description("Use Heal in Group?")]
        public bool SoloShadowUseHeaInGrp { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_SoloShadow))]
        [DisplayName("PW Shield?")]
        [Description("Use Shield on Partymembers in Shadow Spec?")]
        public bool SoloShadowUseShieldParty { get; set; }

        #endregion

        #region GroupHoly  
        [Setting]
        [DefaultValue("")]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Custom Tank")]
        [Description("If you want to override the tank. Leave empty if you don't know")]
        public string GroupHolyCustomTank { get; set; }

        [Setting]
        [DefaultValue(65)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Slow Heal")]
        [Description("Treshhold to cast a big single target heal")]
        [Percentage(true)]
        public int GroupHolyBigSingleTargetHeal { get; set; }

        [Setting]
        [DefaultValue(85)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Binding Heal")]
        [Description("Treshhold to cast binding heal on you and a friendly target")]
        [Percentage(true)]
        public int GroupHolyBindingHealTresh { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Prayer of Mending")]
        [Description("Treshhold to cast Prayer of Mending on friendly targets")]
        [Percentage(true)]
        public int GroupHolyPrayerOfMendingTresh { get; set; }

        [Setting]
        [DefaultValue(90)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Circle of Healing")]
        [Description("Treshhold to cast Circle of Healing on friendly targets")]
        [Percentage(true)]
        public int GroupHolyCircleOfHealingTresh { get; set; }

        [Setting]
        [DefaultValue(80)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Prayer of Healing")]
        [Description("Treshhold to cast Prayer of Healing on friendly targets")]
        [Percentage(true)]
        public int GroupHolyPrayerOfHealingTresh { get; set; }

        [Setting]
        [DefaultValue(45)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Divine Hymn")]
        [Description("Treshhold to cast Divine Hymn if 3 party members fall below the mana treshhold")]
        [Percentage(true)]
        public int GroupHolyDivineHymnTresh { get; set; }

        [Setting]
        [DefaultValue(25)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Guardian Spirit")]
        [Description("Treshhold to cast Guardian Spirit on tank or me")]
        [Percentage(true)]
        public int GroupHolyGuardianSpiritTresh { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Fear Ward")]
        [Description("Cast Fear Ward if an enemy is casting an fear inducing spell on us")]
        public bool GroupHolyProtectAgainstFear { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Mind Soothe")]
        [Description("Uses Mind Soothe if you are too close to an enemy")]
        public bool GroupHolyUseMindSoothe { get; set; }

        [Setting]
        [DefaultValue(6)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Mind Soothe dist.")]
        [Description("Distance to cast Mind Soothe before an enemy will attack you")]
        public int GroupHolyMindSootheDistance { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Shackle Undead")]
        [Description("Shackles Undead if they are out of range and targeting you")]
        public bool GroupHolyShackleUndead { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Cast Off Tank")]
        [Description("Minimum mana percentage to do off tank casts")]
        [Percentage(true)]
        public int GroupHolyOffTankCastingMana { get; set; }
        
        [Setting]
        [DefaultValue(70)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("Preventive Healing")]
        [Description("Minimum mana percentage to do preventive healing (100 will disable it)")]
        [Percentage(true)]
        public int GroupHolyPreventiveHealMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("OOC Refresh")]
        [Description("Will refresh your group OOC? Will burn up your mana. Expected to use drinks afterwards")]
        public bool GroupHolyIgnoreManaManagementOOC { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PriestTriggerDropdown", nameof(Spec.Priest_GroupHoly))]
        [DisplayName("De-DeBuff")]
        [Description("Will remove harmful magic and diseases")]
        public bool GroupHolyDeDeBuff { get; set; }
        #endregion

        public PriestLevelSettings()
        {
            UseWand = true;
            UseWandTresh = 20;
            UseAutoBuffInt = true;
            UseAutoBuff = false;

            SoloShadowShadowForm = true;
            SoloShadowShadowfiend = 30;
            SoloShadowUseHeaInGrp = false;
            SoloShadowUseShieldTresh = 60;
            SoloShadowUseRenewTresh = 90;
            SoloShadowUseFlashTresh = 60;
            SoloShadowUseHealTresh = 40;
            SoloShadowUseMindflay = false;
            SoloShadowDPUse = true;
            SoloShadowDotOff = true;
            SoloShadowUseShieldParty = true;
            SoloShadowDispersion = 30;

            GroupShadowShadowfiend = 30;
            GroupShadowUseShieldTresh = 60;
            GroupShadowDispersion = 30;
            GroupShadowCureDisease = true;
            GroupShadowDispelMagic = true;
            GroupShadowSpreadSWPain = true;
            GroupShadowSpreadVT = true;

            GroupHolyGuardianSpiritTresh = 25;
            GroupHolyBigSingleTargetHeal = 65;
            GroupHolyBindingHealTresh = 85;
            GroupHolyPrayerOfMendingTresh = 95;
            GroupHolyPrayerOfHealingTresh = 80;
            GroupHolyCircleOfHealingTresh = 90;
            GroupHolyDivineHymnTresh = 45;
            GroupHolyProtectAgainstFear = true;
            GroupHolyUseMindSoothe = true;
            GroupHolyMindSootheDistance = 6;
            GroupHolyShackleUndead = false;
            GroupHolyOffTankCastingMana = 40;
            GroupHolyPreventiveHealMana = 70;
            GroupHolyIgnoreManaManagementOOC = true;
            GroupHolyCustomTank = "";
            GroupHolyDeDeBuff = false;
        }
    }
}