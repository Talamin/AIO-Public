using AIO.Combat.Common;
using AIO.Framework;
using System;
using System.ComponentModel;
using System.Linq;
using wManager.Events;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using Timer = robotManager.Helpful.Timer;

namespace AIO.Combat.Addons
{
    internal class RangedPull : ICycleable
    {
        private readonly RotationSpell Pull;
        private readonly Func<float, float> RangeSwap;
        private readonly Timer Timeout = new Timer();

        private float? OldRange;

        public RangedPull(string pull, Func<float, float> rangeSwap)
        {
            Pull = new RotationSpell(pull);
            RangeSwap = rangeSwap;
        }

        public void Dispose()
        {
            FightEvents.OnFightStart -= OnFightStart;
            FightEvents.OnFightLoop -= OnFightLoop;
        }

        public void Initialize()
        {
            FightEvents.OnFightStart += OnFightStart;
            FightEvents.OnFightLoop += OnFightLoop;
        }

        private void OnFightStart(WoWUnit unit, CancelEventArgs cancelable) => Run();
        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable) => Run();

        private void Run()
        {
            var distanceToTarget = Me.Position.DistanceTo(Target.Position);

            if (OldRange != null)
            {
                if (distanceToTarget <= OldRange || Target.IsCast || Timeout.IsReady || ObjectManager.Target.HasTarget && ObjectManager.Target.Target != ObjectManager.Me.Guid)
                {
                    _ = RangeSwap((float)OldRange);
                    OldRange = null;
                }
            }
            if ((Pull.Name == "Throw" || Pull.Name == "Shoot") && !Extension.HaveRangedWeaponEquipped)
            {
                return;
            }
            if (Me.InCombatFlagOnly)
            {
                return;
            }
            if (!Pull.KnownSpell)
            {
                return;
            }

            if (distanceToTarget <= 29f &&
                distanceToTarget >= 10f &&
                HasNearbyEnemies(Target, 25f))
            {
                if (OldRange == null)
                {
                    OldRange = RangeSwap(29f);
                    Timeout.Reset(7 * 1000);
                }

                RotationCombatUtil.CastSpell(Pull, Target, true);
                MovementManager.StopMove();
                Usefuls.WaitIsCasting();
            }
        }

        private static bool HasNearbyEnemies(WoWUnit target, float distance)
        {
            var surroundingEnemies = RotationFramework.Enemies.Where(u =>
                !u.IsTapDenied &&
                !u.IsTaggedByOther &&
                !u.PlayerControlled &&
                u.IsAttackable &&
                u.Guid != target.Guid);

            WoWUnit closestUnit = null;
            float closestUnitDistance = float.PositiveInfinity;

            foreach (var unit in surroundingEnemies)
            {
                float distanceFromTarget = unit.Position.DistanceTo(target.Position);

                if (distanceFromTarget < closestUnitDistance)
                {
                    closestUnit = unit;
                    closestUnitDistance = distanceFromTarget;
                }
            }

            return closestUnit != null && closestUnitDistance < distance;
        }
    }
}
