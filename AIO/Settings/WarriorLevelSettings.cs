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
        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Hamstring")]
        [Description("Use Hamstring in your Rotation?")]
        public bool Hamstring { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fury")]
        [DisplayName("Intercept")]
        [Description("Should we use Intercept?")]
        public bool FuryIntercept { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Protection")]
        [DisplayName("Intercept")]
        [Description("Should we use Intercept?")]
        public bool ProtectionIntercept { get; set; }

        [Setting]
        [DefaultValue(65)]
        [Category("Protection")]
        [DisplayName("Enraged Regeneration")]
        [Description("Treshhold for Warrior HP, when to use ER")]
        public int ProtectionEnragedRegeneration { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Protection")]
        [DisplayName("Shield Block")]
        [Description("Enemycount to use Block Wall")]
        public int ProtectionShieldBlock { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Protection")]
        [DisplayName("Shield Wall")]
        [Description("Enemycount to use Shield Wall")]
        public int ProtectionShieldWall { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Protection")]
        [DisplayName("Taunt")]
        [Description("Should we use Taunt in  Group?")]
        public bool ProtectionTauntGroup { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Fight")]
        [DisplayName("Ranged Pull")]
        [Description("Should we use ranged pull when we have ranged weapon?")]
        public bool PullRanged { get; set; }

        [Setting]
        [DefaultValue(3)]
        [Category("Protection")]
        [DisplayName("Cleave Count")]
        [Description("Enemycount to use Cleave")]
        public int ProtectionCleaveCount { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Protection")]
        [DisplayName("Cleave Count")]
        [Description("Ragecount to use Cleave")]
        public int ProtectionCleaveRageCount { get; set; }

        [Setting]
        [DefaultValue(1)]
        [Category("Protection")]
        [DisplayName("Demoralizing Shout")]
        [Description("Enemycount for Shout?")]
        public int ProtectionDemoralizingCount { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Protection")]
        [DisplayName("Shockwave")]
        [Description("Enemycount for Shout?")]
        public int ProtectionShockwaveCount { get; set; }

        [DropdownList(new string[] { nameof(Spec.Warrior_GroupProtection), nameof(Spec.Warrior_SoloArms), nameof(Spec.Warrior_SoloFury), nameof(Spec.Warrior_GroupFury) })]
        public override string ChooseRotation { get; set; }

        public WarriorLevelSettings()
        {
            PullRanged = true;
            Hamstring = true;
            FuryIntercept = true;
            ProtectionIntercept = true;
            ProtectionShieldBlock = 2;
            ProtectionShieldWall = 3;
            ProtectionCleaveCount = 3;
            ProtectionCleaveRageCount = 30;
            ProtectionDemoralizingCount = 1;
            ProtectionTauntGroup = true;
            ProtectionShockwaveCount = 2;
            ProtectionEnragedRegeneration = 65;
        }
    }
}