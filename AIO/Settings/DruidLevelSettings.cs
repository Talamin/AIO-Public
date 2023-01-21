using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class DruidLevelSettings : BasePersistentSettings<DruidLevelSettings>
    {
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
        #endregion

        #region Feral Settings
        [Setting]
        [DefaultValue(2)]
        [Category("Feral")]
        [DisplayName("Bear")]
        [Description("Set the Amount of Enemies in Close Range to switch to Bear?")]
        public int FeralBearCount { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Feral Charge")]
        [Description("Use Feral Charge?")]
        public bool FeralCharge { get; set; }       

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Prowl")]
        [Description("Use Prowl?")]
        public bool FeralProwl { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Feral")]
        [DisplayName("Force Faerie")]
        [Description("Use Faerie for pull?")]
        public bool FeralForceFaerie { get; set; }


        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Tigers Fury")]
        [Description("Use Tigers Fury on Cooldown?")]
        public bool FeralTigersFury { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Feral")]
        [DisplayName("Rip Health")]
        [Description("Set the health threshold to stop using Rip")]
        public int FeralRipHealth { get; set; }

        [Setting]
        [DefaultValue(5)]
        [Category("Feral")]
        [DisplayName("Finisher Combo Points")]
        [Description("Minimum Combo Points to use FB/Rip?")]
        public int FeralFinisherComboPoints { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Use Faerie Fire")]
        [Description("Use FF in the Rotation?")]
        public bool FeralFaerieFire { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Feral")]
        [DisplayName("OOC Regrowth")]
        [Description("Set the health threshold for OOC Regrowth Healing")]
        public int FeralRegrowth { get; set; }

        [Setting]
        [DefaultValue(85)]
        [Category("Feral")]
        [DisplayName("OOC Rejuvenation")]
        [Description("Set the health threshold for OOC Rejuvenation Healing")]
        public int FeralRejuvenation { get; set; }

        [Setting]
        [DefaultValue(35)]
        [Category("Feral")]
        [DisplayName("IC heal %")]
        [Description("Set the health threshold for in combat healing")]
        public int FeralICHealThreshold { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("In combat Regrowth")]
        [Description("Shapeshift and use Regrowth in combat?")]
        public bool FeralRegrowthIC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("In combat Rejuvenation")]
        [Description("Shapeshift and use Rejuvenation in combat?")]
        public bool FeralRejuvenationIC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("In combat Healing Touch")]
        [Description("Shapeshift and use Healing Touch in combat?")]
        public bool FeralHealingTouchIC { get; set; }        

        [Setting]
        [DefaultValue(false)]
        [Category("Feral")]
        [DisplayName("Decurse")]
        [Description("Decurse Important Spells as Feral in Combat?")]
        public bool FeralDecurse { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Dash")]
        [Description("Use Dash while stealthed?")]
        public bool FeralDash { get; set; }
        #endregion

        #region SoloBalance Settings
        [Setting]
        [DefaultValue(true)]
        [Category("Balance")]
        [DisplayName("Use AOE in Instance")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool SoloBalanceUseAOE { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Balance")]
        [DisplayName("Use Starfall in Instance")]
        [Description("Set this if you want to use Starfall in Instance")]
        public bool SoloBalanceUseStarfall { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Balance")]
        [DisplayName("AOE in Instance")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int SoloBalanceAOETargets { get; set; }

        [Setting]
        [DefaultValue(10)]
        [Category("Balance")]
        [DisplayName("Healing Touch threshold")]
        [Description("Set the Healing threshold for Healing Touch")]
        [Percentage(false)]
        public int SoloBalanceHealingTouch { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Balance")]
        [DisplayName("Rejuvenation threshold")]
        [Description("Set the Healing threshold for Rejuvenation")]
        [Percentage(false)]
        public int SoloBalanceRejuvenation { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Balance")]
        [DisplayName("Regrowth threshold")]
        [Description("Set the Healing threshold for Regrowth")]
        [Percentage(false)]
        public int SoloBalanceRegrowth { get; set; }
        #endregion

        #region SoloRestoration Settings
        [Setting]
        [DefaultValue(80)]
        [Category("Restoration")]
        [DisplayName("Regrowth")]
        [Description("threshold for Regrowth")]
        [Percentage(true)]
        public int SoloRestorationRegrowth { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Restoration")]
        [DisplayName("Rejuvenation")]
        [Description("threshold for Rejuvenation")]
        [Percentage(true)]
        public int SoloRestorationRejuvenation { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Restoration")]
        [DisplayName("Wild Growth")]
        [Description("threshold for Wild Growth")]
        [Percentage(true)]
        public int SoloRestorationWildGrowth { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Restoration")]
        [DisplayName("Wild Growth")]
        [Description("threshold for Wild Growth Player Count")]
        [Percentage(false)]
        public int SoloRestorationWildGrowthCount { get; set; }

        [Setting]
        [DefaultValue(45)]
        [Category("Restoration")]
        [DisplayName("Healing Touch")]
        [Description("threshold for Healing Touch")]
        [Percentage(true)]
        public int SoloRestorationHealingTouch { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Restoration")]
        [DisplayName("Lifebloom")]
        [Description("threshold for Lifebloom")]
        [Percentage(true)]
        public int SoloRestorationLifebloom { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Restoration")]
        [DisplayName("Nourish")]
        [Description("threshold for Nourish use")]
        [Percentage(true)]
        public int SoloRestorationNourish { get; set; }


        [Setting]
        [DefaultValue(true)]
        [Category("Restoration")]
        [DisplayName("Curse")]
        [Description("Remove Curse?")]
        public bool SoloRestorationRemoveCurse { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Restoration")]
        [DisplayName("Poison")]
        [Description("Remove Poison?")]
        public bool SoloRestorationRemovePoison { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Restoration")]
        [DisplayName("Custom Tank")]
        [Description("If you want to override the tank. Leave empty if you don't know")]
        public string SoloRestoCustomTank { get; set; }
        #endregion

        #region GroupRestoration Settings
        [Setting]
        [DefaultValue(80)]
        [Category("Restoration")]
        [DisplayName("Regrowth")]
        [Description("Threshold for Regrowth")]
        [Percentage(true)]
        public int GroupRestorationRegrowth { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Restoration")]
        [DisplayName("Rejuvenation")]
        [Description("Threshold for Rejuvenation")]
        [Percentage(true)]
        public int GroupRestorationRejuvenation { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Restoration")]
        [DisplayName("Swiftmend")]
        [Description("Threshold for Swiftmend")]
        [Percentage(true)]
        public int GroupRestorationSwiftmend { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Restoration")]
        [DisplayName("Wild Growth")]
        [Description("Threshold for Wild Growth")]
        [Percentage(true)]
        public int GroupRestorationWildGrowth { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Restoration")]
        [DisplayName("Wild Growth")]
        [Description("Wild Growth minimum player count")]
        [Percentage(false)]
        public int GroupRestorationWildGrowthCount { get; set; }

        [Setting]
        [DefaultValue(45)]
        [Category("Restoration")]
        [DisplayName("Healing Touch")]
        [Description("Threshold for Healing Touch")]
        [Percentage(true)]
        public int GroupRestorationHealingTouch { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Restoration")]
        [DisplayName("Lifebloom")]
        [Description("Threshold for Lifebloom")]
        [Percentage(true)]
        public int GroupRestorationLifebloom { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Restoration")]
        [DisplayName("Nourish")]
        [Description("Threshold for Nourish use")]
        [Percentage(true)]
        public int GroupRestorationNourish { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Restoration")]
        [DisplayName("Curse")]
        [Description("Remove Curse?")]
        public bool GroupRestorationRemoveCurse { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Restoration")]
        [DisplayName("Poison")]
        [Description("Remove Poison?")]
        public bool GroupRestorationRemovePoison { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Restoration")]
        [DisplayName("Custom Tank")]
        [Description("If you want to override the tank. Leave empty if you don't know")]
        public string GroupRestoCustomTank { get; set; }
        #endregion

        #region Selectors
        [DropdownList(new string[] { "DruidFeral", "DruidBalance", "DruidRestoration" })]
        public override string ChooseTalent { get; set; }

        [DropdownList(new string[] { "Auto", "FeralCombat", "Balance", "Restoration", "GroupFeralTank", "GroupRestorationHeal" })]
        public override string ChooseRotation { get; set; }
        #endregion

        public DruidLevelSettings()
        {
            ChooseTalent = "DruidFeral";
            BuffIC = true;
            HealOOC = true;
            FeralProwl = true;
            FeralForceFaerie = false;
            FeralTigersFury = true;
            FeralFinisherComboPoints = 5;
            FeralFaerieFire = true;
            FeralDash = true;
            FeralCharge = true;           
            FeralBearCount = 2;
            Innervate = 25;
            FeralRipHealth = 30;
            FeralRegrowthIC = true;
            FeralRegrowth = 60;
            FeralRejuvenationIC = true;
            FeralHealingTouchIC = true;
            FeralICHealThreshold = 35;
            FeralRejuvenation = 30;
            FeralDecurse = false;
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
