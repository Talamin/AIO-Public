using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
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
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.LowLevel, new LowLevel() },
                { Spec.Shaman_SoloElemental, new SoloElemental() },
                { Spec.Shaman_GroupRestoration, new GroupRestoration() },
                { Spec.Shaman_SoloEnhancement, new SoloEnhancement() },
                { Spec.Shaman_GroupEnhancement, new GroupEnhancement() },
                { Spec.Fallback, new SoloEnhancement() },
            }, new AutoPartyResurrect("Ancestral Spirit"),
            new ConditionalCycleable(() => Settings.Current.HealOOC, new HealOOC()))
        {
            var totems = new Totems(this);
            Addons.Add(totems);
            Addons.Add(new CombatBuffs(this, totems));
            Addons.Add(new WeaponHelper(this));
        }

        public override void Initialize()
        {
            base.Initialize();

            switch (Specialisation)
            {
                case Spec.Shaman_SoloEnhancement:
                case Spec.Shaman_GroupEnhancement:                    
                    CombatRange = 5.0f;
                    break;
                case Spec.Shaman_SoloElemental:
                    CombatRange = 24.0f;
                    break;
                case Spec.LowLevel:
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
                Settings.Current.UseGhostWolf &&
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