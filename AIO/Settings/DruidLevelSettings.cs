using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class DruidLevelSettings : BasePersistentSettings<DruidLevelSettings>
    {
        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Swimming")]
        [Description("Make use if Swimming Form while swimming??")]
        public bool Swimming { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Buffing IC")]
        [Description("Should the Bot Buff while InCombat?")]
        public bool BuffIC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("HealOOC")]
        [Description("Use Heal Out of Combat?")]
        public bool HealOOC { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Feral")]
        [DisplayName("Bear")]
        [Description("Set the Amount of Enemies in Close Range to switch to Bear?")]
        public int FeralBearCount { get; set; }

        [Setting]
        [DefaultValue(25)]
        [Category("General")]
        [DisplayName("Innervate")]
        [Description("Set the Mana Treshhold, when to use Innervate?")]
        public int Innervate { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Prowl")]
        [Description("Use Prowl?")]
        public bool Prowl { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Feral")]
        [DisplayName("Force Faerie")]
        [Description("Use Faerie for pull?")]
        public bool ForceFaerie { get; set; }


        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Tigers Fury")]
        [Description("Use Tigers Fury on Cooldown?")]
        public bool TF { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Ferocious Bite")]
        [Description("Use FB?")]
        public bool FeralFB { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Rip")]
        [Description("Use Rip?")]
        public bool FeralRip { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Feral")]
        [DisplayName("Rip Health")]
        [Description("Set the Health Treshhold until when Rip is used?!")]
        public int FeralRipHealth { get; set; }

        [Setting]
        [DefaultValue(5)]
        [Category("Feral")]
        [DisplayName("Ferocious Bite/Rip")]
        [Description("Set the Combopoint, when to use FB/Rip?")]
        public int FBC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Faerie Fire Feral")]
        [Description("Use FF in the Rotation?")]
        public bool FFF { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Feral")]
        [DisplayName("OOC Regrowth")]
        [Description("Set the HealthTreshhold for OOC Regrowth Healing")]
        public int FeralRegrowth { get; set; }

        [Setting]
        [DefaultValue(85)]
        [Category("Feral")]
        [DisplayName("OOC Rejuvenation")]
        [Description("Set the HealthTreshhold for OOC Rejuvenation Healing")]
        public int FeralRejuvenation { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("IC Regrowth")]
        [Description("Set the HealthTreshhold for in Combat Regrowth Healing")]
        public bool FeralRegrowthIC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("OOC Rejuvenation")]
        [Description("Set the HealthTreshhold for in Combat Rejuvenation Healing")]
        public bool FeralRejuvenationIC { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Feral")]
        [DisplayName("Decurse")]
        [Description("Decurse Important Spells as Feral in Combat?")]
        public bool FeralDecurse { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Balance")]
        [DisplayName("Use AOE in Instance")]
        [Description("Set this if you want to use AOE in Instance")]
        public bool UseAOE { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Balance")]
        [DisplayName("Use Starfall in Instance")]
        [Description("Set this if you want to use Starfall in Instance")]
        public bool UseStarfall { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Balance")]
        [DisplayName("AOE in Instance")]
        [Description("Number of Targets around the Tank to use AOE in Instance")]
        [Percentage(false)]
        public int AOEInstance { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Feral")]
        [DisplayName("Dash")]
        [Description("Use Dash while stealthed?")]
        public bool Dash { get; set; }

        [Setting]
        [DefaultValue(10)]
        [Category("Balance")]
        [DisplayName("Healing Touch Treshhold")]
        [Description("Set the Healing Treshhold for Healing Touch")]
        [Percentage(false)]
        public int BalanceHealingTouch { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Balance")]
        [DisplayName("Rejuvenatio Treshhold")]
        [Description("Set the Healing Treshhold for Rejuvenation")]
        [Percentage(false)]
        public int BalanceRejuvenation { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Balance")]
        [DisplayName("Regrowth Treshhold")]
        [Description("Set the Healing Treshhold for Regrowth")]
        [Percentage(false)]
        public int BalanceRegrowth { get; set; }

        [Setting]
        [DefaultValue(65)]
        [Category("Restoration")]
        [DisplayName("Regrowth")]
        [Description("Treshhold for Regrowth")]
        [Percentage(false)]
        public int RestorationRegrowth { get; set; }

        [Setting]
        [DefaultValue(90)]
        [Category("Restoration")]
        [DisplayName("Rejuvenation")]
        [Description("Treshhold for Rejuvenation")]
        [Percentage(false)]
        public int RestorationRejuvenation { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Restoration")]
        [DisplayName("Swiftmend")]
        [Description("Treshhold for Swiftmend")]
        [Percentage(false)]
        public int RestorationSwiftmend { get; set; }

        [Setting]
        [DefaultValue(90)]
        [Category("Restoration")]
        [DisplayName("Wild Growth")]
        [Description("Treshhold for Wild Growth")]
        [Percentage(false)]
        public int RestorationWildGrowth { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Restoration")]
        [DisplayName("Wild Growth")]
        [Description("Treshhold for Wild Growth Player Count")]
        [Percentage(false)]
        public int RestorationWildGrowthCount { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Restoration")]
        [DisplayName("Healing Touch")]
        [Description("Treshhold for Healing Touch")]
        [Percentage(false)]
        public int RestorationHealingTouch { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Restoration")]
        [DisplayName("Lifebloom")]
        [Description("Treshhold for Lifebloom")]
        [Percentage(false)]
        public int RestorationLifebloom { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Restoration")]
        [DisplayName("Lifebloom")]
        [Description("Count for bloom Stacks on Tank")]
        [Percentage(false)]
        public int RestorationLifebloomCount { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Restoration")]
        [DisplayName("Nourish")]
        [Description("Treshhold for Nourish use")]
        [Percentage(false)]
        public int RestorationNourish { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Restoration")]
        [DisplayName("Rebirth")]
        [Description("AutoRebirth on dead Targets?")]
        public bool RestorationRebirthAuto { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Restoration")]
        [DisplayName("Curse")]
        [Description("Remove Curse?")]
        public bool RestorationRemoveCurse { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Restoration")]
        [DisplayName("Poison")]
        [Description("Remove Poison?")]
        public bool RestorationRemovePoison { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Restoration")]
        [DisplayName("Custom Tank")]
        [Description("If you want to override the tank. Leave empty if you don't know")]
        public string RestoCustomTank { get; set; }

        [DropdownList(new string[] { "DruidFeral", "DruidBalance", "DruidRestoration" })]
        public override string ChooseTalent { get; set; }

        [DropdownList(new string[] { "Auto", "FeralCombat", "Balance", "Restoration", "GroupFeralTank", "GroupRestorationHeal" })]
        public override string ChooseRotation { get; set; }

        public DruidLevelSettings()
        {
            ChooseTalent = "DruidFeral";
            Swimming = true;
            BuffIC = true;
            HealOOC = true;
            Prowl = true;
            ForceFaerie = false;
            TF = true;
            FBC = 5;
            FFF = true;
            Dash = true;
            FeralFB = true;
            FeralRip = true;
            FeralBearCount = 2;
            Innervate = 25;
            FeralRipHealth = 30;
            FeralRegrowthIC = true;
            FeralRegrowth = 60;
            FeralRejuvenationIC = true;
            FeralRejuvenation = 30;
            FeralDecurse = false;
            UseAOE = true;
            UseStarfall = true;
            AOEInstance = 3;
            RestorationRebirthAuto = true;
            BalanceHealingTouch = 10;
            BalanceRegrowth = 60;
            BalanceRejuvenation = 30;
            RestorationSwiftmend = 60;
            RestorationRegrowth = 65;
            RestorationRejuvenation = 90;
            RestorationWildGrowth = 90;
            RestorationWildGrowthCount = 2;
            RestorationHealingTouch = 50;
            RestorationLifebloom = 95;
            RestorationLifebloomCount = 3;
            RestorationNourish = 50;
            RestoCustomTank = "";
            RestorationRemoveCurse = true;
            RestorationRemovePoison = true;
        }
    }
}
