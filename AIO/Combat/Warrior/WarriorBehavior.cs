using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Settings;
using System.Collections.Generic;

namespace AIO.Combat.Warrior
{
    using Settings = WarriorLevelSettings;
    internal class WarriorBehavior : BaseCombatClass
    {
        private float CombatRange = 5.0f;
        public override float Range => CombatRange;

        private float SwapRange(float range)
        {
            var old = CombatRange;
            CombatRange = range;
            return old;
        }

        internal WarriorBehavior() : base(
            Settings.Current,
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"Arms", new Arms() },
                {"Protection", new Protection() },
                {"Fury", new Fury() },
                {"Default", new Protection() },
            })
        {
            Addons.Add(new Buffs(this));
            Addons.Add(new ConditionalCycleable(() => Settings.Current.PullRanged, new RangedPull("Throw", SwapRange)));
        }
    }
}

