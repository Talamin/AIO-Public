using AIO.Lists;
using MarsSettingsGUI;
using System;
using System.ComponentModel;
using System.Configuration;

namespace AIO.Settings
{
    [Serializable]
    public class WarriorLevelSettings : BasePersistentSettings<WarriorLevelSettings>
    {

        #region Lists
        [TriggerDropdown("WarriorTriggerDropdown", new string[] { nameof(Spec.Warrior_GroupFury), nameof(Spec.Warrior_GroupProtection), nameof(Spec.Warrior_GroupArms), nameof(Spec.Warrior_SoloArms), nameof(Spec.Warrior_SoloFury) })]
        public override string ChooseRotation { get; set; }
        #endregion

        #region General
        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Hamstring")]
        [Description("Use Hamstring in your Rotation?")]
        public bool Hamstring { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Ranged Pull")]
        [Description("Should we use ranged pull when we have ranged weapon?")]
        public bool PullRanged { get; set; }

        #endregion

        #region GroupProtection

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupProtection))]
        [DisplayName("Intercept")]
        [Description("Should we use Intercept?")]
        public bool GroupProtectionIntercept { get; set; }

        [Setting]
        [DefaultValue(65)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupProtection))]
        [DisplayName("Enraged Regeneration")]
        [Description("Treshhold for Warrior HP, when to use ER")]
        public int GroupProtectionEnragedRegeneration { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupProtection))]
        [DisplayName("Shield Block")]
        [Description("Enemycount to use Block Wall")]
        public int GroupProtectionShieldBlock { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupProtection))]
        [DisplayName("Shield Wall")]
        [Description("Enemycount to use Shield Wall")]
        public int GroupProtectionShieldWall { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupProtection))]
        [DisplayName("Taunt")]
        [Description("Should we use Taunt in  Group?")]
        public bool GroupProtectionTauntGroup { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupProtection))]
        [DisplayName("Cleave Count")]
        [Description("Enemycount to use Cleave")]
        public int GroupProtectionCleaveCount { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupProtection))]
        [DisplayName("Cleave Count")]
        [Description("Ragecount to use Cleave")]
        public int GroupProtectionCleaveRageCount { get; set; }

        [Setting]
        [DefaultValue(1)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupProtection))]
        [DisplayName("Demoralizing Shout")]
        [Description("Enemycount for Shout?")]
        public int GroupProtectionDemoralizingCount { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupProtection))]
        [DisplayName("Shockwave")]
        [Description("Enemycount for Shout?")]
        public int GroupProtectionShockwaveCount { get; set; }

        #endregion

        #region SoloFury
        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_SoloFury))]
        [DisplayName("Intercept")]
        [Description("Should we use Intercept?")]
        public bool SoloFuryIntercept { get; set; }
        #endregion

        #region GroupFury
        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupFury))]
        [DisplayName("Charge")]
        [Description("Should we use Charge on mobs close to party members?")]

        public bool GroupFuryCharge { get; set; }
        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupFury))]
        [DisplayName("Intercept")]
        [Description("Should we use Intercept on mobs close to party members?")]
        public bool GroupFuryIntercept { get; set; }
        #endregion

        #region GroupArms

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupArms))]
        [DisplayName("Slam")]
        [Description("Use Slam (recommended if tier bonus)")]
        public bool GroupArmsSlam { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupArms))]
        [DisplayName("AoE Rotation")]
        [Description("Use AoE Rotation when at least X numbers on enemies at close range")]
        public int GroupArmsAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupArms))]
        [DisplayName("Spread Rend")]
        [Description("Use Rend on all targets around you")]
        public bool GroupArmsSpreadRend { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupArms))]
        [DisplayName("Enraged Regeneration")]
        [Description("Use Enraged Regeneration when under X% Health")]
        public int GroupArmsEnragedRegen { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupArms))]
        [DisplayName("Demoralizing Shout")]
        [Description("Use Demoralizing Shout")]
        public bool GroupArmsDemoShout { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_GroupArms))]
        [DisplayName("Intimidating Shout")]
        [Description("Use Intimidating Shout to try and interrupt enemies around you")]
        public bool GroupArmsIntimShout { get; set; }
        #endregion

        #region SoloArms

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_SoloArms))]
        [DisplayName("Slam")]
        [Description("Use Slam (recommended if tier bonus)")]
        public bool SoloArmsSlam { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_SoloArms))]
        [DisplayName("AoE Rotation")]
        [Description("Use AoE Rotation when at least X numbers on enemies at close range")]
        public int SoloArmsAoe { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_SoloArms))]
        [DisplayName("Spread Rend")]
        [Description("Use Rend on all targets around you")]
        public bool SoloArmsSpreadRend { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_SoloArms))]
        [DisplayName("Enraged Regeneration")]
        [Description("Use Enraged Regeneration when under X% Health")]
        public int SoloArmsEnragedRegen { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_SoloArms))]
        [DisplayName("Demoralizing Shout")]
        [Description("Use Demoralizing Shout")]
        public bool SoloArmsDemoShout { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rotation")]
        [VisibleWhenDropdownValue("WarriorTriggerDropdown", nameof(Spec.Warrior_SoloArms))]
        [DisplayName("Intimidating Shout")]
        [Description("Use Intimidating Shout to try and interrupt enemies around you")]
        public bool SoloArmsIntimShout { get; set; }

        #endregion

        public WarriorLevelSettings()
        {
            PullRanged = true;
            Hamstring = true;
            //GroupFury
            GroupFuryCharge = true;
            GroupFuryIntercept = true;
            //SoloFury
            SoloFuryIntercept = true;
            //GroupProtection
            GroupProtectionIntercept = true;
            GroupProtectionShieldBlock = 2;
            GroupProtectionShieldWall = 3;
            GroupProtectionCleaveCount = 3;
            GroupProtectionCleaveRageCount = 30;
            GroupProtectionDemoralizingCount = 1;
            GroupProtectionTauntGroup = true;
            GroupProtectionShockwaveCount = 2;
            GroupProtectionEnragedRegeneration = 65;

            // Group Arms
            GroupArmsSlam = false;
            GroupArmsAoe = 3;
            GroupArmsEnragedRegen = 50;
            GroupArmsDemoShout = true;
            GroupArmsIntimShout = false;
            GroupArmsSpreadRend = true;

            // Solo Arms
            SoloArmsSlam = false;
            SoloArmsAoe = 3;
            SoloArmsEnragedRegen = 50;
            SoloArmsDemoShout = true;
            SoloArmsIntimShout = false;
            SoloArmsSpreadRend = true;
        }
    }
}