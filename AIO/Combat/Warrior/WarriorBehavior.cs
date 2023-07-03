using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Settings;
using System.Collections.Generic;

namespace AIO.Combat.Warrior
{
    using Settings = WarriorLevelSettings;
    internal class WarriorBehavior : BaseCombatClass
    {
        private float CombatRange;
        public override float Range => CombatRange;
        private void SetDefaultRange() => CombatRange = 5.0f;
        private void SetRange(float range) => CombatRange = range;

        internal WarriorBehavior() : base(
            Settings.Current,
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"Arms", new Arms() },
                {"Protection", new Protection() },
                {"Fury", new Fury() },
                {"GroupFury", new Fury() },
                {"Default", new Protection() },
            })
        {
            SetDefaultRange();
            Addons.Add(new Buffs(this));
            Addons.Add(new ConditionalCycleable(() => Settings.Current.ChooseRotation != "Protection" && Settings.Current.PullRanged, new RangedPull(new List<string> { "Throw", "Shoot" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ENEMIES_AROUND)));
            Addons.Add(new ConditionalCycleable(() => Settings.Current.ChooseRotation == "Protection", new RangedPull(new List<string> { "Throw", "Shoot" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ALWAYS)));
        }
    }
}

