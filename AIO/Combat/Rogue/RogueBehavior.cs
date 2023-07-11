using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Lists;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;

namespace AIO.Combat.Rogue
{
    using Settings = RogueLevelSettings;
    internal class RogueBehavior : BaseCombatClass
    {
        private float CombatRange;
        public override float Range => CombatRange;

        private void SetDefaultRange() => CombatRange = 5.0f;
        private void SetRange(float range) => CombatRange = range;

        internal RogueBehavior() : base(
            Settings.Current,
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.LowLevel, new LowLevel() },
                { Spec.Rogue_SoloCombat, new SoloCombat() },
                { Spec.Rogue_GroupCombat, new GroupCombat() },
                //{"Assassination", new Combat() },
                { Spec.Default, new SoloCombat() },
            })
        {
            SetDefaultRange();
            Addons.Add(new ConditionalCycleable(() => Settings.Current.PullRanged, new RangedPull(new List<string> { "Throw", "Shoot" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ENEMIES_AROUND)));
        }

        protected override void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable) => PoisonHelper.CheckPoison();
    }
}

