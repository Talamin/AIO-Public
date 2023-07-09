using AIO.Framework;
using MarsSettingsGUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using static AIO.Constants;

namespace AIO.Settings
{
    [Serializable]
    public abstract class BaseSettings : robotManager.Helpful.Settings
    {
        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Frame Lock")]
        [Description("Use framelock for better logic performance, but lower FPS")]
        public bool FrameLock { get; set; }

        //[Setting]
        //[DefaultValue(50)]
        //[Category("General")]
        //[DisplayName("Scan Range")]
        //[Description("Unit scan range for caching")]
        public int ScanRange { get; set; }

        //[Setting]
        //[DefaultValue(5)]
        //[Category("General")]
        //[DisplayName("Players LoS Credits")]
        //[Description("Maximum number of players to compute LoS for, as part of the caching")]
        public int LoSCreditsPlayers { get; set; }

        //[Setting]
        //[DefaultValue(10)]
        //[Category("General")]
        //[DisplayName("NPCs LoS Credits")]
        //[Description("Maximum number of NPCs to compute LoS for, as part of the caching")]
        public int LoSCreditsNPCs { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Healbot?")]
        [Description("Make AIO Compatible with Healbot")]
        public bool UseSyntheticCombatEvents { get; set; }

        //[Setting]
        //[DefaultValue(false)]
        //[Category("General")]
        //[DisplayName("Straight Pipe")]
        //[Description("You will most likely not want to enable this. Only makes sense when playing manually.")]
        public bool CompletelySynthetic { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Free Move")]
        [Description("Prevent casting during movement")]
        public bool FreeMove { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Backwards")]
        [Description("Adjust Position for Automovement (Warriors,DK)")]
        public bool Backwards { get; set; }

        [Setting]
        [Category("Talents")]
        [DisplayName("Talents Codes")]
        [Description("Use a talent calculator to generate your own codes: https://talentcalculator.org/wotlk/. Do not modify if you are not sure.")]
        public List<string> TalentCodes { get; set; }

        [Setting]
        [Category("Talents")]
        [DefaultValue(true)]
        [DisplayName("Use default talents")]
        [Description("If True, Make sure your talents match the default talents, or reset your talents.")]
        public bool UseDefaultTalents { get; set; }

        [Setting]
        [Category("Talents")]
        [DefaultValue(true)]
        [DisplayName("Auto assign talents")]
        [Description("Will automatically assign your talent points.")]
        public bool AssignTalents { get; set; }

        [Setting]
        [DefaultValue("invalid")]
        [Category("Talents")]
        [DisplayName("Talent Tree")]
        [Description("Choose which Talent Tree you want to learn")]
        public abstract string ChooseTalent { get; set; }

        [Setting]
        [DefaultValue("Auto")]
        [Category("Rotation")]
        [DisplayName("Rotation")]
        [Description("Choose which spell rotation you want to execute")]
        public abstract string ChooseRotation { get; set; }

        protected BaseSettings()
        {
            FrameLock = false;
            ScanRange = 50;
            LoSCreditsPlayers = 5;
            LoSCreditsNPCs = 10;
            UseSyntheticCombatEvents = false;
            CompletelySynthetic = false;
            Backwards = true;
            FreeMove = false;
            AssignTalents = true;
            TalentCodes = new List<string> { };
            UseDefaultTalents = true;
            ChooseTalent = "invalid";
            ChooseRotation = "Auto";
        }

        protected virtual void OnUpdate()
        {
            TalentsManager.Set(AssignTalents, UseDefaultTalents, TalentCodes.ToArray(), ChooseTalent);
            RotationFramework.Setup(framelock: FrameLock, losCreditsPlayers: LoSCreditsPlayers, losCreditsNPCs: LoSCreditsNPCs, scanRange: ScanRange);
            RotationCombatUtil.freeMove = FreeMove;
            RotationFramework.UseSynthetic = UseSyntheticCombatEvents;
        }

        public void ShowConfiguration()
        {
            var settingWindow = new SettingsWindow(this, Me.WowClass.ToString());
            settingWindow.ShowDialog();
            OnUpdate();
        }
    }
}