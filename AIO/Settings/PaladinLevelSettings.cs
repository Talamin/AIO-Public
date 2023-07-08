using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class PaladinLevelSettings : BasePersistentSettings<PaladinLevelSettings>
    {
        #region Selectors

        [DropdownList(new string[] { "PaladinRetribution", "PaladinHoly", "GroupPaladinHoly", "PaladinProtection", "GroupPaladinProtection" })]
        public override string ChooseTalent { get; set; }

        [TriggerDropdown("PaladinTriggerDropdown",new string[] { "Auto", "SoloRetribution", "Holy", "GroupHolyHeal", "Protection", "GroupProtectionTank" })]
        public override string ChooseRotation { get; set; }
        #endregion

        #region General Settings

        [Setting]
        [DefaultValue(50)]
        [Category("General")]
        [DisplayName("Divine Plea")]
        [Description("Set when to use Divine Plea ")]
        public int GeneralDivinePlea { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Divine Protection")]
        [Description("Use Divine Protection")]
        public bool DivineProtection { get; set; }

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
        [DisplayName("Auto Resurrect")]
        [Description("use Autorevive?")]
        public bool Resurrect { get; set; }

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
        #endregion

        #region SoloRetribution

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("In Combat Heal")]
        [Description("Activate this to let Retribution Paladin Heal himself in Combat")]
        public bool SoloRetributionHealInCombat { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Hammer of Justice")]
        [Description("Hammer of Justice when more then 1 Target")]
        public bool SoloRetributionHammerofJustice { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Hand of Reckoning")]
        [Description("Use Hand of Reckoning in Rotation?")]
        public bool SoloRetributionHOR { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Consecration")]
        [Description("How many nearby enemies do we need to use Concectration ")]
        public int SoloRetributionConsecration { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Holy Light")]
        [Description("Set your Treshhold when to use Holy Light")]
        [Percentage(true)]
        public int SoloRetributionHL { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Flash of Light")]
        [Description("Set your Treshhold when to use Flash of Light")]
        [Percentage(true)]
        public int SoloRetributionFL { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Group Heal")]
        [Description("Use Hand Heals on Groupmembers too?")]
        public bool SoloRetributionHealGroup { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Purify")]
        [Description("Allow Purify on yourself")]
        public bool SoloRetributionPurify { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [DisplayName("Sacred Shield")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [Description("Allow the Use of Sacredshield")]
        public bool SoloRetributionSShield { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Lay on Hands")]
        [Description("Allow the Use of Lay on Hands")]
        public bool SoloRetributionLayOnHands { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Judgement Spam")]
        [Description("Use Judgement just when the debuff runs out? (false will spam Judgement)")]
        public bool SoloRetributionJudgementofWisdomSpam { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Avenging Wrath")]
        [Description("Use Avenging Wrath?")]
        public bool SoloAvengingWrathRetribution { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "SoloRetribution")]
        [DisplayName("Seal of Command or other")]
        [Description("Set the Seal you want to used by the FC")]
        [DropdownList(new string[] { "Seal of Command", "Seal of Righteousness", "Seal of Justice", "Seal of Vengeance" })]
        public string SoloSealret { get; set; }
        #endregion

        #region GroupRetribution

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("In Combat Heal")]
        [Description("Activate this to let Retribution Paladin Heal himself in Combat")]
        public bool GroupRetributionHealInCombat { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("Hammer of Justice")]
        [Description("Hammer of Justice when more then 1 Target")]
        public bool GroupRetributionHammerofJustice { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("Hand of Reckoning")]
        [Description("Use Hand of Reckoning in Rotation?")]
        public bool GroupRetributionHOR { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("Consecration")]
        [Description("How many nearby enemies do we need to use Concectration ")]
        public int GroupRetributionConsecration { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("Holy Light")]
        [Description("Set your Treshhold when to use Holy Light")]
        [Percentage(true)]
        public int GroupRetributionHL { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("Flash of Light")]
        [Description("Set your Treshhold when to use Flash of Light")]
        [Percentage(true)]
        public int GroupRetributionFL { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("Purify")]
        [Description("Allow Purify on yourself")]
        public bool GroupRetributionPurify { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [DisplayName("Sacred Shield")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [Description("Allow the Use of Sacredshield")]
        public bool GroupRetributionSShield { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("Lay on Hands")]
        [Description("Allow the Use of Lay on Hands")]
        public bool GroupRetributionLayOnHands { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("Judgement Spam")]
        [Description("Use Judgement just when the debuff runs out? (false will spam Judgement)")]
        public bool GroupRetributionJudgementofWisdomSpam { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("Avenging Wrath")]
        [Description("Use Avenging Wrath?")]
        public bool GroupAvengingWrathRetribution { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupRetribution")]
        [DisplayName("Seal of Command or other")]
        [Description("Set the Seal you want to used by the FC")]
        [DropdownList(new string[] { "Seal of Command", "Seal of Righteousness", "Seal of Justice", "Seal of Vengeance" })]
        public string GroupSealret { get; set; }
        #endregion


        #region Aura
        [Setting]
        [DefaultValue(false)]
        [Category("Aura")]
        [DisplayName("Combat Aura")]
        [Description("Set Combat Aura")]
        [DropdownList(new string[] { "Devotion Aura", "Retribution Aura", "Concentration Aura" })]
        public string Aura { get; set; }
        #endregion

        #region SoloProtectionTank
        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Seal of Command or other")]
        [Description("Set the Seal you want to used by the FC")]
        [DropdownList(new string[] { "Seal of Command", "Seal of Righteousness", "Seal of Justice", "Seal of Light", "Seal of Wisdom", "Seal of Vengeance" })]
        public string Sealprot { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Consecration")]
        [Description("How many nearby enemies do we need to use Concectration ")]
        public int ProtConsecration { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Seal of ... other then Wisdom")]
        [Description("Set your Treshhold when to use the Mainseal...")]
        [Percentage(true)]
        public int ProtectionSoL { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Avenging Wrath")]
        [Description("Use Avenging Wrath?")]
        public bool AvengingWrathProtection { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Seal of Wisdom")]
        [Description("Set your Treshhold when to use Seal of Wisdom")]
        [Percentage(true)]
        public int ProtectionSoW { get; set; }

        [Setting]
        [DefaultValue(5)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Lay on Hands")]
        [Description("Set your Treshhold for LoH on Paladin")]
        [Percentage(true)]
        public int ProtectionLoH { get; set; }


        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Hand of Reckoning")]
        [Description("Use HoR in Dungeons? Autotaunt.")]
        public bool ProtectionHoR { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Righteous Defense")]
        [Description("Use Righteous Defense in Dungeons on mobs? Autotaunt, Server dependent.")]
        public bool RightDefense { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Hand of Protection")]
        [Description("Use HoP in Dungeons? ")]
        public bool ProtectionHoP { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Cleanse")]
        [Description("Use Cleanse on which  Targets? ")]
        [DropdownList(new string[] { "Group", "Me", "None" })]
        public string ProtectionCleanse { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Holy Light")]
        [Description("Use HL to selfheal when not in group")]
        public bool ProtectionHolyLight { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Protection")]
        [DisplayName("Hammer of Justice")]
        [Description("Hammer of Justice when more then 1 Target")]
        public bool ProtectionHammerofJustice { get; set; }
        #endregion

        #region GroupProt
        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupProtectionTank")]
        [DisplayName("Seal of Command or other")]
        [Description("Set the Seal you want to used by the FC")]
        [DropdownList(new string[] { "Seal of Command", "Seal of Righteousness", "Seal of Justice", "Seal of Light", "Seal of Wisdom", "Seal of Vengeance" })]
        public string GroupSealprot { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupProtectionTank")]
        [DisplayName("Seal of ... other then Wisdom")]
        [Description("Set your Treshhold when to use the Mainseal...")]
        [Percentage(true)]
        public int GroupProtectionSoL { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupProtectionTank")]
        [DisplayName("Consecration")]
        [Description("How many nearby enemies do we need to use Concectration ")]
        public int GroupProtConsecration { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupProtectionTank")]
        [DisplayName("Avenging Wrath")]
        [Description("Use Avenging Wrath?")]
        public bool GroupAvengingWrathProtection { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupProtectionTank")]
        [DisplayName("Seal of Wisdom")]
        [Description("Set your Treshhold when to use Seal of Wisdom")]
        [Percentage(true)]
        public int GroupProtectionSoW { get; set; }

        [Setting]
        [DefaultValue(5)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupProtectionTank")]
        [DisplayName("Lay on Hands")]
        [Description("Set your Treshhold for LoH & Divine Shield on Paladin")]
        [Percentage(true)]
        public int GroupProtectionLoH { get; set; }


        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupProtectionTank")]
        [DisplayName("Hand of Reckoning")]
        [Description("Use HoR in Dungeons? Autotaunt.")]
        public bool GroupProtectionHoR { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupProtectionTank")]
        [DisplayName("Hand of Protection")]
        [Description("Use HoP in Dungeons? ")]
        public bool GroupProtectionHoP { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupProtectionTank")]
        [DisplayName("Cleanse")]
        [Description("Use Cleanse on which  Targets? ")]
        [DropdownList(new string[] { "Group", "Me", "None" })]
        public string GroupProtectionCleanse { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupProtectionTank")]
        [DisplayName("Hammer of Justice")]
        [Description("Hammer of Justice when more then 1 Target")]
        public bool GroupProtectionHammerofJustice { get; set; }
        #endregion

        #region SoloHoly
        [Setting]
        [DefaultValue(60)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Holy")]
        [DisplayName("Holy Shock")]
        [Description("Set your Treshhold when to use Holy Shock")]
        [Percentage(true)]
        public int HolyHS { get; set; }

        [Setting]
        [DefaultValue(75)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Holy")]
        [DisplayName("Holy Light")]
        [Description("Set your Treshhold when to use Holy Light")]
        [Percentage(true)]
        public int HolyHL { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Holy")]
        [DisplayName("Flash of Light")]
        [Description("Set your Treshhold when to use Flash of Light")]
        [Percentage(true)]
        public int HolyFL { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Holy")]
        [DisplayName("Custom Tank")]
        [Description("If you want to override the tank. Leave empty if you don't know")]
        public string HolyCustomTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Holy")]
        [DisplayName("Purify")]
        [Description("Allow Purify on yourself")]
        public bool HolyPurify { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Holy")]
        [DisplayName("LoH")]
        [Description("Allow Lay on Hands on Tank with hp < 15%")]
        public bool HolyLoH { get; set; }

        [Setting]
        [DefaultValue(15)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "Holy")]
        [DisplayName("LoH Value")]
        [Description("Set your Treshhold when to LoH on Tank")]
        [Percentage(true)]
        public int HolyLoHTresh { get; set; }
        #endregion

        #region GroupHoly
        [Setting]
        [DefaultValue(60)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupHolyHeal")]
        [DisplayName("Holy Shock")]
        [Description("Set your Treshhold when to use Holy Shock")]
        [Percentage(true)]
        public int GroupHolyHS { get; set; }

        [Setting]
        [DefaultValue(75)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupHolyHeal")]
        [DisplayName("Holy Light")]
        [Description("Set your Treshhold when to use Holy Light")]
        [Percentage(true)]
        public int GroupHolyHL { get; set; }

        [Setting]
        [DefaultValue(95)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupHolyHeal")]
        [DisplayName("Flash of Light")]
        [Description("Set your Treshhold when to use Flash of Light")]
        [Percentage(true)]
        public int GroupHolyFL { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupHolyHeal")]
        [DisplayName("Custom Tank")]
        [Description("If you want to override the tank. Leave empty if you don't know")]
        public string GroupHolyCustomTank { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupHolyHeal")]
        [DisplayName("Purify")]
        [Description("Allow Purify on yourself")]
        public bool GroupHolyPurify { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupHolyHeal")]
        [DisplayName("LoH")]
        [Description("Allow Lay on Hands on Tank with hp < 15%")]
        public bool GroupHolyLoH { get; set; }

        [Setting]
        [DefaultValue(15)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("PaladinTriggerDropdown", "GroupHolyHeal")]
        [DisplayName("LoH Value")]
        [Description("Set your Treshhold when to LoH on Tank")]
        [Percentage(true)]
        public int GroupHolyLoHTresh { get; set; }
        #endregion

        public PaladinLevelSettings()
        {
            ChooseTalent = "PaladinRetribution";
            Crusader = false;
            HealOOC = true;
            DivinePleaOOC = true;
            DivinePleaIC = true;
            GeneralDivinePlea = 50;
            ProtConsecration = 2;
            GroupProtConsecration = 2;

            Resurrect = true;
            Buffing = true;
            DivineProtection = true;
            GroupHolyHS = 60;
            HolyHL = 75;
            HolyFL = 95;
            HolyLoH = false;
            HolyLoHTresh = 15;
            HolyCustomTank = "";
            HolyPurify = true;

            RightDefense = true;
            ProtectionHammerofJustice = true;
            ProtectionHoP = true;

            ProtectionHolyLight = true;
            ProtectionSoL = 95;
            ProtectionSoW = 40;
            ProtectionHoR = true;
            ProtectionLoH = 5;
            ProtectionCleanse = "None";
            AvengingWrathProtection = true;


            SoloRetributionConsecration = 2;
            SoloRetributionHammerofJustice = true;
            SoloRetributionHOR = true;
            SoloRetributionPurify = true;
            SoloRetributionSShield = true;
            SoloRetributionLayOnHands = true;
            SoloRetributionJudgementofWisdomSpam = false;
            SoloRetributionHealInCombat = false;
            SoloRetributionHealGroup = false;
            SoloRetributionHL = 50;
            SoloRetributionFL = 30;
            SoloAvengingWrathRetribution = true;

            GroupRetributionConsecration = 2;
            GroupRetributionHammerofJustice = true;
            GroupRetributionHOR = true;
            GroupRetributionPurify = true;
            GroupRetributionSShield = true;
            GroupRetributionLayOnHands = true;
            GroupRetributionJudgementofWisdomSpam = false;
            GroupRetributionHealInCombat = false;
            GroupRetributionHL = 50;
            GroupRetributionFL = 30;
            GroupAvengingWrathRetribution = true;


            Aura = "Devotion Aura";
            SoloSealret = "Seal of Righteousness";
            GroupSealret= "Seal of Righteousness";
            Sealprot = "Seal of Light";
            GroupProtectionSoL = 95;
            GroupSealprot = "Seal of Wisdom";
            GroupProtectionSoW = 40;
            GroupProtectionLoH = 10;
            GroupProtectionHoR = true;
            GroupProtectionHoP = true;
            GroupProtectionCleanse = "Group";
            GroupProtectionHammerofJustice = true;
            GroupAvengingWrathProtection = true;
            GroupProtectionHammerofJustice = true;
            GroupHolyLoH = true;
            GroupHolyLoHTresh = 15;
            GroupHolyHS = 60;
            GroupHolyHL = 75;
            GroupHolyFL = 95;
            GroupHolyLoH = false;
            GroupHolyLoHTresh = 15;
            GroupHolyPurify = true;
            GroupHolyCustomTank = "";

        }
    }
}