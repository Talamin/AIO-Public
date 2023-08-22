using AIO.Lists;
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
        [TriggerDropdown("RogueTriggerDropdown",new string[] { nameof(Spec.Rogue_SoloCombat), nameof(Spec.Rogue_GroupCombat), nameof(Spec.Rogue_GroupAssassination) })]
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
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_SoloCombat))]
        [DisplayName("Stealth")]
        [Description("Use Stealth?")]
        public bool SoloCombatStealth { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_SoloCombat))]
        [DisplayName("Evasion")]
        [Description("Enemycount for using Evasion?")]
        public int SoloCombatEvasion { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_SoloCombat))]
        [DisplayName("Blade Flurry")]
        [Description("Enemycount for using BladeFlurry?")]
        public int SoloCombatBladeFLurry { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_SoloCombat))]
        [DisplayName("Killing Spree")]
        [Description("Enemycount for using Killing Spree?")]
        public int SoloCombatKillingSpree { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_SoloCombat))]
        [DisplayName("Adrenaline Rush")]
        [Description("Enemycount for using Adrenaline Rush?")]
        public int SoloCombatAdrenalineRush { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_SoloCombat))]
        [DisplayName("Eviscarate")]
        [Description("Combopoints for using Eviscarate?")]
        public int SoloCombatEviscarate { get; set; }

        //Groupcombat

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_GroupCombat))]
        [DisplayName("Evasion")]
        [Description("Enemycount for using Evasion?")]
        public int GroupCombatEvasion { get; set; }

        [DefaultValue(80)]
        [Percentage(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_GroupCombat))]
        [DisplayName("Evasion")]
        [Description("Treshhold of own Health for using Evasion?")]
        public int GroupCombatEvasionHealth { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_GroupCombat))]
        [DisplayName("Blade Flurry")]
        [Description("Enemycount for using BladeFlurry?")]
        public int GroupCombatBladeFLurry { get; set; }

        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_GroupCombat))]
        [DisplayName("Killing Spree")]
        [Description("Enemycount for using Killing Spree?")]
        public int GroupCombatKillingSpree { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_GroupCombat))]
        [DisplayName("Adrenaline Rush")]
        [Description("Enemycount for using Adrenaline Rush?")]
        public int GroupCombatAdrenalineRush { get; set; }

        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_GroupCombat))]
        [DisplayName("Eviscarate")]
        [Description("Combopoints for using Eviscarate?")]
        public int GroupCombatEviscarate { get; set; }

        // Group Assassination
        [DefaultValue(50)]
        [Percentage(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_GroupAssassination))]
        [DisplayName("Evasion")]
        [Description("Treshhold of own Health for using Evasion")]
        public int GroupAssassEvasionHealth { get; set; }

        [DefaultValue(50)]
        [Percentage(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_GroupAssassination))]
        [DisplayName("Cloak of Shadows")]
        [Description("Treshhold of own Health for using Cloak of Shadows")]
        public int GroupAssassCoSHealth { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_GroupAssassination))]
        [DisplayName("Blind")]
        [Description("Use Blind to interrupt enemies")]
        public bool GroupAssassBlind { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("RogueTriggerDropdown", nameof(Spec.Rogue_GroupAssassination))]
        [DisplayName("Fan of Knives")]
        [Description("Use Fan of Knives when at least X enemies are around you")]
        public int GroupAssassFanOfKnives { get; set; }

        public RogueLevelSettings()
        {
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

            // Group Assassination
            GroupAssassEvasionHealth = 50;
            GroupAssassCoSHealth = 50;
            GroupAssassBlind = true;
            GroupAssassFanOfKnives = 3;
        }
    }
}

