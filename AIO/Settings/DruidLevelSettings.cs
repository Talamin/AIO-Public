using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class DruidLevelSettings : BasePersistentSettings<DruidLevelSettings>
    {

        #region Selectors
        [DropdownList(new string[] { "DruidFeral", "DruidBalance", "DruidRestoration" })]
        public override string ChooseTalent { get; set; }

        [TriggerDropdown("DruidTriggerDropdown", new string[] { "Auto", "SoloFeral", "SoloBalance", "SoloRestoration", "GroupFeralTank", "GroupRestorationHeal" })]
        public override string ChooseRotation { get; set; }
        #endregion

        #region General settings       
        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Buffing IC")]
        [Description("Should the Bot Buff while In Combat?")]
        public bool BuffIC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Heal OOC")]
        [Description("Use heals out of combat?")]
        public bool HealOOC { get; set; }

        [Setting]
        [DefaultValue(25)]
        [Category("General")]
        [DisplayName("Innervate Mana %")]
        [Description("Set the Mana threshold to use Innervate")]
        public int Innervate { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Rebirth")]
        [Description("Use Rebirth on dead targets in combat?")]
        public bool RebirthAuto { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("General")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "General")]
        [DisplayName("OOC Regrowth")]
        [Description("Set the health threshold for OOC Regrowth Healing")]
        public int OOCRegrowth { get; set; }

        [Setting]
        [DefaultValue(85)]
        [Category("General")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "General")]
        [DisplayName("OOC Rejuvenation")]
        [Description("Set the health threshold for OOC Rejuvenation Healing")]
        public int OOCRejuvenation { get; set; }
        #endregion

        #region SoloFeral Settings
        [Setting]
        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("Bear")]
        [Description("Set the Amount of Enemies in Close Range to switch to Bear?")]
        public int SoloFeralBearCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("Feral Charge")]
        [Description("Use Feral Charge?")]
        public bool SoloFeralCharge { get; set; }       

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("Prowl")]
        [Description("Use Prowl?")]
        public bool SoloFeralProwl { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("Force Faerie")]
        [Description("Use Faerie for pull?")]
        public bool SoloFeralForceFaerie { get; set; }


        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("Tigers Fury")]
        [Description("Use Tigers Fury on Cooldown?")]
        public bool SoloFeralTigersFury { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("Rip Health")]
        [Description("Set the health threshold to stop using Rip")]
        public int SoloFeralRipHealth { get; set; }

        [Setting]
        [DefaultValue(5)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("Finisher Combo Points")]
        [Description("Minimum Combo Points to use FB/Rip?")]
        public int SoloFeralFinisherComboPoints { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("Use Faerie Fire")]
        [Description("Use FF in the Rotation?")]
        public bool SoloFeralFaerieFire { get; set; }

        [Setting]
        [DefaultValue(35)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("IC heal %")]
        [Description("Set the health threshold for in combat healing")]
        public int SoloFeralICHealThreshold { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("In combat Regrowth")]
        [Description("Shapeshift and use Regrowth in combat?")]
        public bool SoloFeralRegrowthIC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("In combat Rejuvenation")]
        [Description("Shapeshift and use Rejuvenation in combat?")]
        public bool SoloFeralRejuvenationIC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("In combat Healing Touch")]
        [Description("Shapeshift and use Healing Touch in combat?")]
        public bool SoloFeralHealingTouchIC { get; set; }        

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("Decurse")]
        [Description("Decurse Important Spells as Feral in Combat?")]
        public bool SoloFeralDecurse { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloFeral")]
        [DisplayName("Dash")]
        [Description("Use Dash while stealthed?")]
        public bool SoloFeralDash { get; set; }
        #endregion

        #region GroupFeralTank Settings
        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupFeralTank")]
        [DisplayName("Feral Charge")]
        [Description("Use Feral Charge?")]
        public bool GroupFeralCharge { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupFeralTank")]
        [DisplayName("Use Faerie Fire")]
        [Description("Use FF in the Rotation?")]
        public bool GroupFeralFaerieFire { get; set; }
        #endregion

        #region SoloBalance Settings
        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloBalance")]
        [DisplayName("Use AOE in Instance")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool SoloBalanceUseAOE { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloBalance")]
        [DisplayName("Use Starfall in Instance")]
        [Description("Set this if you want to use Starfall in Instance")]
        public bool SoloBalanceUseStarfall { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloBalance")]
        [DisplayName("AOE in Instance")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int SoloBalanceAOETargets { get; set; }

        [Setting]
        [DefaultValue(10)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloBalance")]
        [DisplayName("Healing Touch threshold")]
        [Description("Set the Healing threshold for Healing Touch")]
        [Percentage(false)]
        public int SoloBalanceHealingTouch { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloBalance")]
        [DisplayName("Rejuvenation threshold")]
        [Description("Set the Healing threshold for Rejuvenation")]
        [Percentage(false)]
        public int SoloBalanceRejuvenation { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloBalance")]
        [DisplayName("Regrowth threshold")]
        [Description("Set the Healing threshold for Regrowth")]
        [Percentage(false)]
        public int SoloBalanceRegrowth { get; set; }
        #endregion

        #region SoloRestoration Settings
        [Setting]
        [DefaultValue(80)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloRestoration")]
        [DisplayName("Regrowth")]
        [Description("threshold for Regrowth")]
        [Percentage(true)]
        public int SoloRestorationRegrowth { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloRestoration")]
        [DisplayName("Rejuvenation")]
        [Description("threshold for Rejuvenation")]
        [Percentage(true)]
        public int SoloRestorationRejuvenation { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloRestoration")]
        [DisplayName("Wild Growth")]
        [Description("threshold for Wild Growth")]
        [Percentage(true)]
        public int SoloRestorationWildGrowth { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloRestoration")]
        [DisplayName("Wild Growth")]
        [Description("threshold for Wild Growth Player Count")]
        [Percentage(false)]
        public int SoloRestorationWildGrowthCount { get; set; }

        [Setting]
        [DefaultValue(45)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloRestoration")]
        [DisplayName("Healing Touch")]
        [Description("threshold for Healing Touch")]
        [Percentage(true)]
        public int SoloRestorationHealingTouch { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloRestoration")]
        [DisplayName("Lifebloom")]
        [Description("threshold for Lifebloom")]
        [Percentage(true)]
        public int SoloRestorationLifebloom { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloRestoration")]
        [DisplayName("Nourish")]
        [Description("threshold for Nourish use")]
        [Percentage(true)]
        public int SoloRestorationNourish { get; set; }


        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloRestoration")]
        [DisplayName("Curse")]
        [Description("Remove Curse?")]
        public bool SoloRestorationRemoveCurse { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloRestoration")]
        [DisplayName("Poison")]
        [Description("Remove Poison?")]
        public bool SoloRestorationRemovePoison { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "SoloRestoration")]
        [DisplayName("Custom Tank")]
        [Description("If you want to override the tank. Leave empty if you don't know")]
        public string SoloRestoCustomTank { get; set; }
        #endregion

        #region GroupRestorationHeal Settings
        [Setting]
        [DefaultValue(80)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]
        [DisplayName("Regrowth")]
        [Description("Threshold for Regrowth")]
        [Percentage(true)]
        public int GroupRestorationRegrowth { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]
        [DisplayName("Rejuvenation")]
        [Description("Threshold for Rejuvenation")]
        [Percentage(true)]
        public int GroupRestorationRejuvenation { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]
        [DisplayName("Swiftmend")]
        [Description("Threshold for Swiftmend")]
        [Percentage(true)]
        public int GroupRestorationSwiftmend { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]        
        [DisplayName("Wild Growth")]
        [Description("Threshold for Wild Growth")]
        [Percentage(true)]
        public int GroupRestorationWildGrowth { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]
        [DisplayName("Wild Growth")]
        [Description("Wild Growth minimum player count")]
        [Percentage(false)]
        public int GroupRestorationWildGrowthCount { get; set; }

        [Setting]
        [DefaultValue(45)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]
        [DisplayName("Healing Touch")]
        [Description("Threshold for Healing Touch")]
        [Percentage(true)]
        public int GroupRestorationHealingTouch { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]
        [DisplayName("Lifebloom")]
        [Description("Threshold for Lifebloom")]
        [Percentage(true)]
        public int GroupRestorationLifebloom { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]
        [DisplayName("Nourish")]
        [Description("Threshold for Nourish use")]
        [Percentage(true)]
        public int GroupRestorationNourish { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]
        [DisplayName("Curse")]
        [Description("Remove Curse?")]
        public bool GroupRestorationRemoveCurse { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]
        [DisplayName("Poison")]
        [Description("Remove Poison?")]
        public bool GroupRestorationRemovePoison { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("DruidTriggerDropdown", "GroupRestorationHeal")]
        [DisplayName("Custom Tank")]
        [Description("If you want to override the tank. Leave empty if you don't know")]
        public string GroupRestoCustomTank { get; set; }
        #endregion


        public DruidLevelSettings()
        {
            ChooseTalent = "DruidFeral";
            BuffIC = true;
            HealOOC = true;
            SoloFeralProwl = true;
            SoloFeralForceFaerie = false;
            SoloFeralTigersFury = true;
            SoloFeralFinisherComboPoints = 5;
            SoloFeralFaerieFire = true;
            SoloFeralDash = true;
            SoloFeralCharge = true;           
            SoloFeralBearCount = 2;
            Innervate = 25;
            SoloFeralRipHealth = 30;
            SoloFeralRegrowthIC = true;
            OOCRegrowth = 60;
            SoloFeralRejuvenationIC = true;
            SoloFeralHealingTouchIC = true;
            SoloFeralICHealThreshold = 35;
            OOCRejuvenation = 30;
            SoloFeralDecurse = false;
            GroupFeralCharge = true;
            GroupFeralFaerieFire = true;
            SoloBalanceUseAOE = true;
            SoloBalanceUseStarfall = true;
            SoloBalanceAOETargets = 3;
            RebirthAuto = true;
            SoloBalanceHealingTouch = 10;
            SoloBalanceRegrowth = 60;
            SoloBalanceRejuvenation = 30;
            SoloRestorationRegrowth = 65;
            SoloRestorationRejuvenation = 90;
            SoloRestorationWildGrowth = 90;
            SoloRestorationWildGrowthCount = 2;
            SoloRestorationHealingTouch = 50;
            SoloRestorationLifebloom = 95;
            SoloRestorationNourish = 50;
            SoloRestoCustomTank = "";
            SoloRestorationRemoveCurse = true;
            SoloRestorationRemovePoison = true;
            GroupRestorationSwiftmend = 60;
            GroupRestorationRegrowth = 80;
            GroupRestorationRejuvenation = 95;
            GroupRestorationWildGrowth = 95;
            GroupRestorationWildGrowthCount = 3;
            GroupRestorationHealingTouch = 45;
            GroupRestorationLifebloom = 95;
            GroupRestorationNourish = 50;
            GroupRestoCustomTank = "";
            GroupRestorationRemoveCurse = true;
            GroupRestorationRemovePoison = true;
        }
    }
}
