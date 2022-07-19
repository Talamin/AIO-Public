using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using wManager;
using wManager.Wow.Bot.States;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Shaman
{
    using Settings = ShamanLevelSettings;
    internal class ShamanBehavior : BaseCombatClass
    {
        private float CombatRange;
        public override float Range => CombatRange;

        internal ShamanBehavior() : base(
            Settings.Current,
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"Elemental", new Elemental() },
                {"Restoration", new Restoration() },
                {"Enhancement", new Enhancement() },
                {"Default", new Enhancement() },
            }, new AutoPartyResurrect("Ancestral Spirit"),
            new ConditionalCycleable(() => Settings.Current.HealOOC, new HealOOC()))
        {
            var totems = new Totems(this);
            Addons.Add(totems);
            Addons.Add(new Buffs(this, totems));
            Addons.Add(new WeaponHelper(this));
        }

        public override void Initialize()
        {
            base.Initialize();

            switch (Specialisation)
            {
                case "Enhancement":
                    CombatRange = 5.0f;
                    break;
                case "LowLevel":
                    CombatRange = 25.0f;
                    break;
                default:
                    CombatRange = 29.0f;
                    break;
            }
        }

        protected override void OnMoveToPulse(Vector3 point, CancelEventArgs cancelable)
        {
            UseTotemicRecall(point);
            UseGhostWolf(point); 
        }
        
        protected override void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
        {
            var last = points.LastOrDefault();
            if (last == null)
            {
                return;
            }
            UseTotemicRecall(last);
            UseGhostWolf(last);
        }

        private readonly Spell TotemRecall = new Spell("Totemic Recall");

        private void UseTotemicRecall(Vector3 point)
        {
            if (point.DistanceTo(Me.Position) < wManagerSetting.CurrentSetting.MountDistance)
            {
                return;
            }
            if(!TotemRecall.KnownSpell || !Me.IsAlive)
            {
                return;
            }
            if(!new Regeneration().NeedToRun &&
                Totems.HasAny("Stoneskin Totem",
                "Strength of Earth Totem", 
                "Magma Totem",
                "Searing Totem", 
                "Flametongue Totem", 
                "Totem of Wrath", 
                "Wrath of Air Totem",
                "Windfury Totem",
                "Mana Spring Totem",
                "Healing Stream Totem") &&
                !Me.InCombat)
            {
                TotemRecall.Launch();
                Usefuls.WaitIsCasting();
            }
        }

        private readonly Spell GhostWolf = new Spell("Ghost Wolf");
        private void UseGhostWolf(Vector3 point)
        {
            if (point.DistanceTo(Me.Position) < wManagerSetting.CurrentSetting.MountDistance)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(wManagerSetting.CurrentSetting.GroundMountName) &&
                !new Regeneration().NeedToRun &&
                !Me.HaveMyBuff("Ghost Wolf") &&
                Settings.Current.Ghostwolf &&
                Me.IsAlive &&
                GhostWolf.KnownSpell &&
                !Me.InCombat)
            {
                GhostWolf.Launch();
                Usefuls.WaitIsCasting();
            }
        }
    }
}