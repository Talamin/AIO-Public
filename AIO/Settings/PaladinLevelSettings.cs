using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class PaladinLevelSettings : BasePersistentSettings<PaladinLevelSettings>
    {
        [Setting]
        [DefaultValue(50)]
        [Category("General")]
        [DisplayName("Divine Plea")]
        [Description("Set when to use Divine Plea ")]
        public int GeneralDivinePlea { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("General")]
        [DisplayName("Consecration")]
        [Description("Set the Enemycount >= for Consecration on all Specs ")]
        public int GeneralConsecration { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Crusader")]
        [Description("switch Crusader Aura")]
        public bool Crusader { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Auto Buffing")]
        [Description("use Autobuffing?")]
        public bool Buffing { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Heal OOC")]
        [Description("Use Healspells Out of Combat?")]
        public bool HealOOC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Divine Plea OOC")]
        [Description("Use DP out of Combat?")]
        public bool DivinePleaOOC { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Divine Plea IC")]
        [Description("Use DP in Combat?")]
        public bool DivinePleaIC { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Retribution")]
        [DisplayName("In Combat Heal")]
        [Description("Activate this to let Retribution Paladin Heal himself in Combat")]
        public bool RetributionHealInCombat { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Retribution")]
        [DisplayName("Hammer of Justice")]
        [Description("Hammer of Justice when more then 1 Target")]
        public bool RetributionHammerofJustice { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Retribution")]
        [DisplayName("Hand of Reckoning")]
        [Description("Use Hand of Reckoning in Rotation?")]
        public bool RetributionHOR { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Retribution")]
        [DisplayName("Holy Light")]
        [Description("Set your Treshhold when to use Holy Light")]
        [Percentage(true)]
        public int RetributionHL { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Retribution")]
        [DisplayName("Flash of Light")]
        [Description("Set your Treshhold when to use Flash of Light")]
        [Percentage(true)]
        public int RetributionFL { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Retribution")]
        [DisplayName("Group Heal")]
        [Description("Use Hand Heals on Groupmembers too?")]
        public bool RetributionHealGroup { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Retribution")]
        [DisplayName("Purify")]
        [Description("Allow Purify on yourself")]
        public bool RetributionPurify { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Retribution")]
        [DisplayName("Sacred Shield")]
        [Description("Allow the Use of Sacredshield")]
        public bool RetributionSShield { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Retribution")]
        [DisplayName("Lay on Hands")]
        [Description("Allow the Use of Lay on Hands")]
        public bool RetributionLayOnHands { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Divine Protection")]
        [Description("Allow the Use of Divine Protection")]
        public bool RetributionDivProtection { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Retribution")]
        [DisplayName("Judgement Spam")]
        [Description("Use Judgement just when the debuff runs out? (false will spam Judgement)")]
        public bool RetributionJudgementofWisdomSpam { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Retribution")]
        [DisplayName("Avenging Wrath")]
        [Description("Use Avenging Wrath?")]
        public bool AvengingWrathRetribution { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Aura")]
        [DisplayName("Combat Aura")]
        [Description("Set Combat Aura")]
        [DropdownList(new string[] { "Devotion Aura", "Retribution Aura", "Concentration Aura" })]
        public string Aura { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Retribution")]
        [DisplayName("Seal of Command or other")]
        [Description("Set the Seal you want to used by the FC")]
        [DropdownList(new string[] { "Seal of Command", "Seal of Righteousness", "Seal of Justice", "Seal of Vengeance" })]
        public string Sealret { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Protection")]
        [DisplayName("Seal of Command or other")]
        [Description("Set the Seal you want to used by the FC")]
        [DropdownList(new string[] { "Seal of Command", "Seal of Righteousness", "Seal of Justice", "Seal of Light", "Seal of Wisdom", "Seal of Vengeance" })]
        public string Sealprot { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Protection")]
        [DisplayName("Seal of ... other then Wisdom")]
        [Description("Set your Treshhold when to use the Mainseal...")]
        [Percentage(true)]
        public int ProtectionSoL { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Protection")]
        [DisplayName("Avenging Wrath")]
        [Description("Use Avenging Wrath?")]
        public bool AvengingWrathProtection { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Protection")]
        [DisplayName("Seal of Wisdom")]
        [Description("Set your Treshhold when to use Seal of Wisdom")]
        [Percentage(true)]
        public int ProtectionSoW { get; set; }

        [Setting]
        [DefaultValue(5)]
        [Category("Protection")]
        [DisplayName("Lay on Hands")]
        [Description("Set your Treshhold for LoH on Paladin")]
        [Percentage(true)]
        public int ProtectionLoH { get; set; }


        [Setting]
        [DefaultValue(true)]
        [Category("Protection")]
        [DisplayName("Hand of Reckoning")]
        [Description("Use HoR in Dungeons? Autotaunt.")]
        public bool ProtectionHoR { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Protection")]
        [DisplayName("Righteous Defense")]
        [Description("Use Righteous Defense in Dungeons on mobs? Autotaunt, Server dependent.")]
        public bool RightDefense { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Protection")]
        [DisplayName("Hand of Protection")]
        [Description("Use HoP in Dungeons? ")]
        public bool ProtectionHoP { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Protection")]
        [DisplayName("Cleanse")]
        [Description("Use Cleanse on which  Targets? ")]
        [DropdownList(new string[] { "Group", "Me", "None" })]
        public string ProtectionCleanse { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Holy")]
        [DisplayName("Holy Shock")]
        [Description("Set your Treshhold when to use Holy Shock")]
        [Percentage(true)]
        public int HolyHS { get; set; }

        [Setting]
        [DefaultValue(75)]
        [Category("Holy")]
        [DisplayName("Holy Light")]
        [Description("Set your Treshhold when to use Holy Light")]
        [Percentage(true)]
        public int HolyHL { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Holy")]
        [DisplayName("Flash of Light")]
        [Description("Set your Treshhold when to use Flash of Light")]
        [Percentage(true)]
        public int HolyFL { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Holy")]
        [DisplayName("Custom Tank")]
        [Description("If you want to override the tank. Leave empty if you don't know")]
        public string HolyCustomTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Holy")]
        [DisplayName("Purify")]
        [Description("Allow Purify on yourself")]
        public bool HolyPurify { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Holy")]
        [DisplayName("LoH")]
        [Description("Allow Lay on Hands on Tank with hp < 15%")]
        public bool HolyLoH { get; set; }

        [Setting]
        [DefaultValue(15)]
        [Category("Holy")]
        [DisplayName("LoH Value")]
        [Description("Set your Treshhold when to LoH on Tank")]
        [Percentage(true)]
        public int HolyLoHTresh { get; set; }

        [DropdownList(new string[] { "PaladinRetribution", "PaladinHoly", "PaladinProtection" })]
        public override string ChooseTalent { get; set; }

        [DropdownList(new string[] { "Auto", "Retribution", "Holy", "Protection" })]
        public override string ChooseRotation { get; set; }

        public PaladinLevelSettings()
        {
            ChooseTalent = "PaladinRetribution";
            Crusader = false;
            HealOOC = true;
            DivinePleaOOC = true;
            DivinePleaIC = true;
            GeneralDivinePlea = 50;
            GeneralConsecration = 2;
            Buffing = true;
            HolyHS = 60;
            HolyHL = 75;
            HolyFL = 95;
            HolyLoH = false;
            HolyLoHTresh = 15;
            HolyCustomTank = "";
            HolyPurify = true;
            RetributionHammerofJustice = true;
            RetributionHOR = true;
            RetributionPurify = true;
            RetributionSShield = true;
            RetributionLayOnHands = true;
            RetributionDivProtection = true;
            RetributionJudgementofWisdomSpam = false;
            RetributionHealInCombat = false;
            RetributionHealGroup = false;
            RightDefense = true;
            ProtectionHoP = true;
            RetributionHL = 50;
            RetributionFL = 30;
            ProtectionSoL = 95;
            ProtectionSoW = 40;
            ProtectionHoR = true;
            ProtectionLoH = 5;
            ProtectionCleanse = "None";
            AvengingWrathProtection = true;
            AvengingWrathRetribution = true;
            Aura = "Devotion Aura";
            Sealret = "Seal of Righteousness";
            Sealprot = "Seal of Light";
        }
    }
}