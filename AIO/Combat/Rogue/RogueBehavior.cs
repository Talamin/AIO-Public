using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;

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
                { Spec.Rogue_GroupAssassination, new GroupAssassination() },
                { Spec.Fallback, new SoloCombat() },
            })
        {
            SetDefaultRange();
            Addons.Add(new Racials());
            Addons.Add(new ApplyPoison());
            if (Specialisation == Spec.Rogue_SoloCombat && Settings.Current.PullRanged)
                Addons.Add(new RangedPull(new List<string> { "Throw", "Shoot" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ENEMIES_AROUND));
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}

