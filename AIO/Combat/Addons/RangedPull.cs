using AIO.Combat.Common;
using AIO.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using wManager.Events;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using Timer = robotManager.Helpful.Timer;

namespace AIO.Combat.Addons
{
    internal class RangedPull : ICycleable
    {
        private readonly List<RotationSpell> PullSpells = new List<RotationSpell>();
        private Timer Timeout = new Timer();
        private RotationSpell ChosenPullSPell;
        private Action SetDefaultRange;
        private Action<float> SetRange;
        private PullCondition _pullCondition;

        public enum PullCondition
        {
            ALWAYS,
            ENEMIES_AROUND
        }

        public RangedPull(List<string> pullSPells, Action setDefaultRange, Action<float> setRange, PullCondition pullCondition)
        {
            foreach (string pullSPell in pullSPells)
            {
                PullSpells.Add(new RotationSpell(pullSPell));
            }
            SetDefaultRange = setDefaultRange;
            SetRange = setRange;
            _pullCondition = pullCondition;
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

        private void OnFightStart(WoWUnit unit, CancelEventArgs cancelable) => FighStart();
        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable) => Run();

        private void FighStart()
        {
            Timeout = new Timer(7 * 1000);
            List<RotationSpell> availableSpells = PullSpells.FindAll(spell => spell.KnownSpell && spell.IsSpellUsable);
            if (EquippedItems.GetEquippedItems().Exists(item => item.GetItemInfo.ItemSubType == "Thrown"))
            {
                ChosenPullSPell = availableSpells.FirstOrDefault(spell => spell.Name == "Throw");
                return;
            }
            if (EquippedItems.GetEquippedItems().Exists(item => item.GetItemInfo.ItemSubType == "Bows" || item.GetItemInfo.ItemSubType == "Guns" || item.GetItemInfo.ItemSubType == "Crossbows"))
            {
                ChosenPullSPell = availableSpells.FirstOrDefault(spell => spell.Name == "Shoot");
                return;
            }
            ChosenPullSPell = availableSpells.FirstOrDefault();
        }

        private void Run()
        {
            var distanceToTarget = Me.Position.DistanceTo(Target.Position);

            // No pull spell known
            if (ChosenPullSPell == null) 
            {
                return;
            }

            if (Timeout.IsReady || Target.IsCast || Target.HasTarget && Target.Target != Me.Guid)
            {
                SetDefaultRange();
                return;
            }

            // Pull succesful, wait for the enemy to come
            if (Target.Target == Me.Guid)
            {
                return;
            }

            bool pullCOnditionMet = _pullCondition == PullCondition.ALWAYS
                || _pullCondition == PullCondition.ENEMIES_AROUND && HasNearbyEnemies(Target, 25f);
            if (!pullCOnditionMet)
            {
                SetDefaultRange();
                return;
            }

            float pullRange = ChosenPullSPell.MaxRange - 5;
            SetRange(pullRange);

            if (distanceToTarget <= pullRange + 4
                && distanceToTarget >= 10f)
            {
                MovementManager.StopMove();
                RotationCombatUtil.CastSpell(ChosenPullSPell, Target, true);
                Thread.Sleep(2000);
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
