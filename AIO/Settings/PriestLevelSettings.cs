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
        public PriestLevelSettings()
        {
            UseWand = true;
            UseWandTresh = 20;
            UseAutoBuffInt = true;
            UseAutoBuff = false;
            ShadowForm = true;
            ShadowShadowfiend = 30;
            ShadowUseHeaInGrp = false;
            ShadowUseShieldTresh = 60;
            ShadowUseRenewTresh = 90;
            ShadowUseFlashTresh = 60;
            ShadowUseHealTresh = 40;
            ShadowUseMindflay = false;
            ShadowDPUse = true;
            ShadowDotOff = true;
            ShadowUseShieldParty = true;
            ShadowDispersion = 30;
            HolyGuardianSpiritTresh = 25;
            HolyBigSingleTargetHeal = 65;
            HolyBindingHealTresh = 85;
            HolyPrayerOfMendingTresh = 95;
            HolyPrayerOfHealingTresh = 80;
            HolyCircleOfHealingTresh = 90;
            HolyDivineHymnTresh = 45;
            HolyProtectAgainstFear = true;
            HolyUseMindSoothe = true;
            HolyMindSootheDistance = 6;
            HolyShackleUndead = false;
            HolyOffTankCastingMana = 40;
            HolyPreventiveHealMana = 70;
            HolyIgnoreManaManagementOOC = true;
            HolyCustomTank = "";
            HolyDeDeBuff = false;
        }

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

        [Setting]
        [DefaultValue(true)]
        [Category("Shadow")]
        [DisplayName("ShadowForm?")]
        [Description("Use ShadowForm in General?")]
        public bool ShadowForm { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Shadow")]
        [DisplayName("Shadowfiend")]
        [Description("% when use Shadowfiend in General?")]
        public int ShadowShadowfiend { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Shadow")]
        [DisplayName("Use Dot on Offtargets?")]
        [Description("Use Dot on Adds?")]
        public bool ShadowDotOff { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Shadow")]
        [DisplayName("Use Devouring Plague?")]
        [Description("Use DP up to level 80?")]
        public bool ShadowDPUse { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Shadow")]
        [DisplayName("Use Shield Treshold?")]
        [Description("Own life for Shield Usage?")]
        [Percentage(true)]
        public int ShadowUseShieldTresh { get; set; }

        [Setting]
        [DefaultValue(90)]
        [Category("Shadow")]
        [DisplayName("Use Renew Treshold?")]
        [Description("Treshold for Renew Usage?")]
        [Percentage(true)]
        public int ShadowUseRenewTresh { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Shadow")]
        [DisplayName("Use Heal Treshold?")]
        [Description("Treshold for Heal Usage?")]
        [Percentage(true)]
        public int ShadowUseHealTresh { get; set; }
        
        [Setting]
        [DefaultValue(60)]
        [Category("Shadow")]
        [DisplayName("Use Flash Heal Treshold?")]
        [Description("Treshold for Flash Heal Usage?")]
        [Percentage(true)]
        public int ShadowUseFlashTresh { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Shadow")]
        [DisplayName("Dispersion")]
        [Description("Treshold for Dispersion Usage?")]
        [Percentage(true)]
        public int ShadowDispersion { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Shadow")]
        [DisplayName("Use Mindflay?")]
        [Description("Use Mindflay in General?")]
        public bool ShadowUseMindflay { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Shadow")]
        [DisplayName("Use Heal in Group?")]
        [Description("Use Heal in Group?")]
        public bool ShadowUseHeaInGrp { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Shadow")]
        [DisplayName("PW Shield?")]
        [Description("Use Shield on Partymembers in Shadow Spec?")]
        public bool ShadowUseShieldParty { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Holy")]
        [DisplayName("Custom Tank")]
        [Description("If you want to override the tank. Leave empty if you don't know")]
        public string HolyCustomTank { get; set; }

        [Setting]
        [DefaultValue(65)]
        [Category("Holy")]
        [DisplayName("Slow Heal")]
        [Description("Treshhold to cast a big single target heal")]
        [Percentage(true)]
        public int HolyBigSingleTargetHeal { get; set; }

        [Setting]
        [DefaultValue(85)]
        [Category("Holy")]
        [DisplayName("Binding Heal")]
        [Description("Treshhold to cast binding heal on you and a friendly target")]
        [Percentage(true)]
        public int HolyBindingHealTresh { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Holy")]
        [DisplayName("Prayer of Mending")]
        [Description("Treshhold to cast Prayer of Mending on friendly targets")]
        [Percentage(true)]
        public int HolyPrayerOfMendingTresh { get; set; }

        [Setting]
        [DefaultValue(90)]
        [Category("Holy")]
        [DisplayName("Circle of Healing")]
        [Description("Treshhold to cast Circle of Healing on friendly targets")]
        [Percentage(true)]
        public int HolyCircleOfHealingTresh { get; set; }

        [Setting]
        [DefaultValue(80)]
        [Category("Holy")]
        [DisplayName("Prayer of Healing")]
        [Description("Treshhold to cast Prayer of Healing on friendly targets")]
        [Percentage(true)]
        public int HolyPrayerOfHealingTresh { get; set; }

        [Setting]
        [DefaultValue(45)]
        [Category("Holy")]
        [DisplayName("Divine Hymn")]
        [Description("Treshhold to cast Divine Hymn if 3 party members fall below the mana treshhold")]
        [Percentage(true)]
        public int HolyDivineHymnTresh { get; set; }

        [Setting]
        [DefaultValue(25)]
        [Category("Holy")]
        [DisplayName("Guardian Spirit")]
        [Description("Treshhold to cast Guardian Spirit on tank or me")]
        [Percentage(true)]
        public int HolyGuardianSpiritTresh { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Holy")]
        [DisplayName("Fear Ward")]
        [Description("Cast Fear Ward if an enemy is casting an fear inducing spell on us")]
        public bool HolyProtectAgainstFear { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Holy")]
        [DisplayName("Mind Soothe")]
        [Description("Uses Mind Soothe if you are too close to an enemy")]
        public bool HolyUseMindSoothe { get; set; }

        [Setting]
        [DefaultValue(6)]
        [Category("Holy")]
        [DisplayName("Mind Soothe dist.")]
        [Description("Distance to cast Mind Soothe before an enemy will attack you")]
        public int HolyMindSootheDistance { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Holy")]
        [DisplayName("Shackle Undead")]
        [Description("Shackles Undead if they are out of range and targeting you")]
        public bool HolyShackleUndead { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Holy")]
        [DisplayName("Cast Off Tank")]
        [Description("Minimum mana percentage to do off tank casts")]
        [Percentage(true)]
        public int HolyOffTankCastingMana { get; set; }
        
        [Setting]
        [DefaultValue(70)]
        [Category("Holy")]
        [DisplayName("Preventive Healing")]
        [Description("Minimum mana percentage to do preventive healing (100 will disable it)")]
        [Percentage(true)]
        public int HolyPreventiveHealMana { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Holy")]
        [DisplayName("OOC Refresh")]
        [Description("Will refresh your group OOC? Will burn up your mana. Expected to use drinks afterwards")]
        public bool HolyIgnoreManaManagementOOC { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Holy")]
        [DisplayName("De-DeBuff")]
        [Description("Will remove harmful magic and diseases")]
        public bool HolyDeDeBuff { get; set; }

        [DropdownList(new[] { nameof(Spec.Priest_SoloShadow), nameof(Spec.Priest_GroupHoly) })]
        public override string ChooseRotation { get; set; }
    }
}