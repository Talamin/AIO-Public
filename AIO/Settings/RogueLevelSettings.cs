using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class RogueLevelSettings : BasePersistentSettings<RogueLevelSettings>
    {
        //Lists

        [DropdownList(new string[] { "RogueCombat", "RogueAssassination", "RogueSubletly" })]
        public override string ChooseTalent { get; set; }

        [TriggerDropdown("RogueTriggerDropdown",new string[] { "Auto", "SoloCombat"})]
        public override string ChooseRotation { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Ranged Pull")]
        [Description("Should we use ranged pull when we have ranged weapon?")]
        public bool PullRanged { get; set; }

        //[Setting]
        //[DefaultValue(false)]
        //[Category("Fighting")]
        //[DisplayName("Distracting  (not functional atm")]
        //[Description("Use distracting while stealthed?")]
        //public bool Distract { get; set; }

        //SoloCombat


        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [DisplayName("Stealth")]
        [Description("Use Stealth?")]
        public bool SoloCombatStealth { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "SoloCombat")]
        [DisplayName("Evasion")]
        [Description("Enemycount for using Evasion?")]
        public int SoloCombatEvasion { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "SoloCombat")]
        [DisplayName("Blade Flurry")]
        [Description("Enemycount for using BladeFlurry?")]
        public int SoloCombatBladeFLurry { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "SoloCombat")]
        [DisplayName("Killing Spree")]
        [Description("Enemycount for using Killing Spree?")]
        public int SoloCombatKillingSpree { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "SoloCombat")]
        [DisplayName("Adrenaline Rush")]
        [Description("Enemycount for using Adrenaline Rush?")]
        public int SoloCombatAdrenalineRush { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "SoloCombat")]
        [DisplayName("Eviscarate")]
        [Description("Combopoints for using Eviscarate?")]
        public int SoloCombatEviscarate { get; set; }

        //Groupcombat

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "GroupCombat")]
        [DisplayName("Evasion")]
        [Description("Enemycount for using Evasion?")]
        public int GroupCombatEvasion { get; set; }

        [DefaultValue(80)]
        [Percentage(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "GroupCombat")]
        [DisplayName("Evasion")]
        [Description("Treshhold of own Health for using Evasion?")]
        public int GroupCombatEvasionHealth { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "GroupCombat")]
        [DisplayName("Blade Flurry")]
        [Description("Enemycount for using BladeFlurry?")]
        public int GroupCombatBladeFLurry { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "GroupCombat")]
        [DisplayName("Killing Spree")]
        [Description("Enemycount for using Killing Spree?")]
        public int GroupCombatKillingSpree { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "GroupCombat")]
        [DisplayName("Adrenaline Rush")]
        [Description("Enemycount for using Adrenaline Rush?")]
        public int GroupCombatAdrenalineRush { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", "GroupCombat")]
        [DisplayName("Eviscarate")]
        [Description("Combopoints for using Eviscarate?")]
        public int GroupCombatEviscarate { get; set; }

        public RogueLevelSettings()
        {
            
            ChooseTalent = "RogueCombat";
            PullRanged = true;

            //SoloCombat
            SoloCombatStealth = false;
            SoloCombatEvasion = 2;
            SoloCombatBladeFLurry = 2;
            SoloCombatKillingSpree = 2;
            SoloCombatAdrenalineRush = 3;
            SoloCombatEviscarate = 3;
            //Distract = false;

            //GroupCombat
            GroupCombatEvasionHealth = 80;
            GroupCombatEvasion = 2;
            GroupCombatBladeFLurry = 2;
            GroupCombatKillingSpree = 2;
            GroupCombatAdrenalineRush = 3;
            GroupCombatEviscarate = 3;
        }
    }
}

