﻿using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;
using wManager;
using wManager.Events;
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
        private readonly Spell _ghostWolfSpell = new Spell("Ghost Wolf");
        private readonly Spell _totemRecallSpell = new Spell("Totemic Recall");
        private void SetDefaultRange() => CombatRange = 5.0f;
        private void SetRange(float range) => CombatRange = range;

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
            })
        {
            Addons.Add(new Racials());
            if (Settings.Current.HealOOC)
                Addons.Add(new HealOOC());
            Totems totemsAddon = new Totems(this);
            Addons.Add(new AutoPartyResurrect("Ancestral Spirit"));
            Addons.Add(totemsAddon);
            Addons.Add(new CombatBuffs(this, totemsAddon));
            if (Specialisation == Spec.Shaman_SoloEnhancement)
                Addons.Add(new RangedPull(SetDefaultRange, SetRange, RangedPull.PullCondition.ENEMIES_AROUND));
        }

        public override void Initialize()
        {
            switch (Specialisation)
            {
                case Spec.Shaman_SoloEnhancement:
                case Spec.Shaman_GroupEnhancement:
                    CombatRange = 5.0f;
                    break;
                case Spec.Shaman_SoloElemental:
                    CombatRange = 27.0f;
                    break;
                case Spec.LowLevel:
                    CombatRange = 25.0f;
                    break;
                default:
                    CombatRange = 29.0f;
                    break;
            }

            MovementEvents.OnMoveToPulse += OnMoveToPulse;
            MovementEvents.OnMovementPulse += OnMovementPulse;
            base.Initialize();
        }

        public override void Dispose()
        {
            MovementEvents.OnMoveToPulse -= OnMoveToPulse;
            MovementEvents.OnMovementPulse -= OnMovementPulse;
            base.Dispose();
        }

        private void OnMoveToPulse(Vector3 point, CancelEventArgs cancelable)
        {
            UseTotemicRecall(point);
            UseGhostWolf(point);
        }

        private void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
        {
            var last = points.LastOrDefault();
            if (last == null)
            {
                return;
            }
            UseTotemicRecall(last);
            UseGhostWolf(last);
        }

        private void UseTotemicRecall(Vector3 point)
        {
            if (point.DistanceTo(Me.Position) < wManagerSetting.CurrentSetting.MountDistance)
            {
                return;
            }
            if (!_totemRecallSpell.KnownSpell || !Me.IsAlive)
            {
                return;
            }
            if (!new Regeneration().NeedToRun &&
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
                !Me.InCombatFlagOnly)
            {
                _totemRecallSpell.Launch();
                Usefuls.WaitIsCasting();
            }
        }

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
                _ghostWolfSpell.KnownSpell &&
                !Me.InCombatFlagOnly)
            {
                _ghostWolfSpell.Launch();
                Usefuls.WaitIsCasting();
            }
        }
    }
}