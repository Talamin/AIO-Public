﻿using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;

namespace AIO.Combat.Rogue
{
    using Settings = RogueLevelSettings;
    internal class RogueBehavior : BaseCombatClass
    {
        private float CombatRange = 5.0f;
        public override float Range => CombatRange;

        private float SwapRange(float range)
        {
            var old = CombatRange;
            CombatRange = range;
            return old;
        }

        internal RogueBehavior() : base(
            Settings.Current,
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"Combat", new Combat() },
                //{"Assassination", new Combat() },
                {"Default", new Combat() },
            })
        {
            Addons.Add(new ConditionalCycleable(() => Settings.Current.PullRanged, new RangedPull("Throw", SwapRange)));
        }

        protected override void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable) => PoisonHelper.CheckPoison();
    }
}

