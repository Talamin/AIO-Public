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
    internal class RangedPull : IAddon
    {
        private readonly List<RotationSpell> _pullSpells = new List<RotationSpell>();
        private Timer _timeout = new Timer();
        private RotationSpell _chosenPullSPell;
        private PullCondition _pullCondition;
        private Action SetDefaultRange;
        private Action<float> SetRange;

        public bool RunOutsideCombat => false;
        public bool RunInCombat => true;
        public List<RotationStep> Rotation => new List<RotationStep>();

        public enum PullCondition
        {
            ALWAYS,
            ENEMIES_AROUND
        }

        public RangedPull(List<string> pullSPells, Action setDefaultRange, Action<float> setRange, PullCondition pullCondition)
        {
            foreach (string pullSPell in pullSPells)
            {
                _pullSpells.Add(new RotationSpell(pullSPell));
            }
            SetDefaultRange = setDefaultRange;
            SetRange = setRange;
            _pullCondition = pullCondition;
        }

        public void Initialize()
        {
            FightEvents.OnFightStart += OnFightStart;
            FightEvents.OnFightLoop += OnFightLoop;
        }

        public void Dispose()
        {
            FightEvents.OnFightStart -= OnFightStart;
            FightEvents.OnFightLoop -= OnFightLoop;
        }

        private void OnFightStart(WoWUnit unit, CancelEventArgs cancelable) => FighStart();
        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable) => Run();

        private void FighStart()
        {
            _timeout = new Timer(7 * 1000);
            List<RotationSpell> availableSpells = _pullSpells.FindAll(spell => spell.KnownSpell);
            if (EquippedItems.GetEquippedItems().Exists(item => item.GetItemInfo.ItemSubType == "Thrown"))
            {
                _chosenPullSPell = availableSpells.FirstOrDefault(spell => spell.Name == "Throw");
                return;
            }
            if (EquippedItems.GetEquippedItems().Exists(item => item.GetItemInfo.ItemSubType == "Bows" || item.GetItemInfo.ItemSubType == "Guns" || item.GetItemInfo.ItemSubType == "Crossbows"))
            {
                _chosenPullSPell = availableSpells.FirstOrDefault(spell => spell.Name == "Shoot");
                return;
            }
            _chosenPullSPell = availableSpells.FirstOrDefault();
        }

        private void Run()
        {
            var distanceToTarget = Me.Position.DistanceTo(Target.Position);

            // No pull spell known or usable, try fallback
            if (!Me.InCombatFlagOnly && (_chosenPullSPell == null || !_chosenPullSPell.IsSpellUsable))
            {
                SetDefaultRange();
                return;
            }

            if (_timeout.IsReady || Target.IsCast || Target.HasTarget && Target.Target != Me.Guid)
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

            float pullRange = _chosenPullSPell.MaxRange - 5;
            SetRange(pullRange);

            if (distanceToTarget <= pullRange + 4
                && distanceToTarget >= 10f)
            {
                MovementManager.StopMove();
                RotationCombatUtil.CastSpell(_chosenPullSPell, Target, true);
                Timer timer = new Timer(2000);
                while (!Me.InCombatFlagOnly
                    && !timer.IsReady
                    && Conditions.InGameAndConnectedAndAlive)
                {
                    Thread.Sleep(100);
                }
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
