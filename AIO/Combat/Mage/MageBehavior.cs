using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class MageBehavior : BaseCombatClass
    {
        public override float Range => 29.0f;

        internal MageBehavior() : base(
            Settings.Current,
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"Frost", new Frost() },
                {"Arcane", new Arcane() },
                {"Fire", new Fire() },
                {"Default", new Frost() },
            },
            new Buffs(),
            new ConditionalCycleable(() => Settings.Current.Backpaddle,
                new AutoBackpedal(
                    () => Target.GetDistance <= Settings.Current.BackpaddleRange && Target.HaveBuff("Frost Nova"),
                    Settings.Current.BackpaddleRange)))
        { }

        protected override void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
        {
            MageFoodManager.CheckIfEnoughFoodAndDrinks();
        }
        //protected override void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
        //{
        //    if(Target.HaveBuff("Polymorph") && RotationFramework.AllUnits.Count(o => o.IsAlive && o.IsTargetingMe) >= 1)
        //    {
        //        WoWUnit NewTarget = RotationFramework.AllUnits.Where(o => o.IsAlive && o.IsTargetingMe && !o.HaveBuff("Polymorph")).OrderBy(o => o.Position.DistanceTo(Me.Position) < 40).FirstOrDefault();
        //        Me.Target = NewTarget.Guid;
        //        if(Target.IsAlive)
        //        {
        //            Fight.StartFight(Target.Guid);
        //        }
        //    }
        //}
    }
}