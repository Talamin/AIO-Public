using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Lists;
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
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.LowLevel, new LowLevel() },
                { Spec.Warrior_SoloArms, new SoloArms() },
                { Spec.Warrior_GroupProtection, new GroupProtection() },
                { Spec.Warrior_SoloFury, new SoloFury() },
                { Spec.Warrior_GroupFury, new GroupFury() },
                { Spec.Warrior_GroupArms, new GroupArms() },
                { Spec.Fallback, new SoloFury() },
            })
        {
            SetDefaultRange();
            Addons.Add(new Racials());
            Addons.Add(new CombatBuffs(this));
            if (Specialisation != Spec.Warrior_GroupProtection 
                && Specialisation != Spec.Warrior_GroupArms
                && Specialisation != Spec.Warrior_GroupFury
                && Settings.Current.PullRanged)
                Addons.Add(new RangedPull(new List<string> { "Throw", "Shoot" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ENEMIES_AROUND));
            if (Specialisation == Spec.Warrior_GroupProtection)
                Addons.Add(new RangedPull(new List<string> { "Throw", "Shoot" }, SetDefaultRange, SetRange, RangedPull.PullCondition.ALWAYS));
        }
    }
}

