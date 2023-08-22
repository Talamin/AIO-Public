using robotManager.Helpful;
using System;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Framework
{
    public class RotationStep : IComparable<RotationStep>
    {
        private readonly float _priority;
        private readonly IRotationAction _action;
        private readonly Func<IRotationAction, WoWUnit, bool> _targetPredicate;
        private readonly Func<IRotationAction, bool> _constantPredicate;
        private readonly Func<Func<WoWUnit, bool>, WoWUnit> _targetFinder;
        private readonly bool _forceCast = false;
        private readonly bool _checkRange = true;
        private readonly bool _checkLoS = false;
        private readonly string _name;
        private Timer _forcedTimer;
        private readonly int _forcedTimerMs;
        private readonly bool _preventDoubleCast;
        private readonly bool _ignoreGCD;

        public RotationStep(IRotationAction action,
            float priority,
            Func<IRotationAction, WoWUnit, bool> targetPredicate,
            Func<IRotationAction, bool> constantPredicate,
            Func<Func<WoWUnit, bool>, WoWUnit> targetFinder,
            Exclusive exclusive = null,
            bool forceCast = false,
            bool checkRange = true,
            bool checkLoS = false,
            int forcedTimerMS = 0,
            bool preventDoubleCast = false,
            bool ignoreGCD = false)
        {
            _action = action;
            _priority = priority;
            _targetPredicate = targetPredicate;
            _constantPredicate = constantPredicate;
            _targetFinder = targetFinder;
            Exclusive = exclusive;
            _forceCast = forceCast;
            _checkRange = checkRange;
            _checkLoS = checkLoS;
            _forcedTimerMs = forcedTimerMS;
            _preventDoubleCast = preventDoubleCast;

            _name = action.GetType().FullName;
            if (_action is RotationSpell spell)
            {
                _name = spell.Name;
            }
            if (_action is RotationAction rCode)
            {
                _name = rCode.Name;
            }
            _ignoreGCD = ignoreGCD;
        }

        public RotationStep(IRotationAction action,
            float priority,
            Func<IRotationAction, WoWUnit, bool> targetPredicate,
            Func<Func<WoWUnit, bool>, WoWUnit> targetFinder,
            Exclusive exclusive = null,
            bool forceCast = false,
            bool checkRange = true,
            bool checkLoS = false,
            int forcedTimerMS = 0,
            bool preventDoubleCast = false,
            bool ignoreGCD = false) :
            this(action, priority, targetPredicate, (_) => true, targetFinder, exclusive, forceCast, checkRange,
                checkLoS, forcedTimerMS, preventDoubleCast, ignoreGCD)
        { }

        // For code execution, ignores GCD by default
        public RotationStep(IRotationAction action,
            float priority,
            int forcedTimerMS = 0,
            bool ignoreGCD = true) :
            this(action, priority, (a, t) => true, (_) => true, RotationCombatUtil.FindMe, null, false, false, false, forcedTimerMS, false, ignoreGCD)
        { }

        public int CompareTo(RotationStep other) => _priority.CompareTo(other._priority);

        private Func<WoWUnit, bool> CreatePredicate(Exclusives exclusives) => (target) =>
        {
            try
            {
                /* If CheckRange is enabled, check the action range */
                if (_checkRange && target.GetDistance > _action.MaxRange)
                {
                    RotationLogger.Trace($"{_name} is out of range on {target.Name}");
                    return false;
                }

                /* If the unique token has been consumed for this target, fail the search */
                if (exclusives.Contains(target, Exclusive))
                {
                    RotationLogger.Trace($"{_name} has already had its token consumed on {target.Name}");
                    return false;
                }

                var watch = System.Diagnostics.Stopwatch.StartNew();
                bool correctTarget = _targetPredicate(_action, target);
                bool could = correctTarget && (!_checkLoS || target.IsLocalPlayer ||
                                               !TraceLine.TraceLineGo(ObjectManager.Me.PositionWithoutType,
                                                   target.PositionWithoutType, CGWorldFrameHitFlags.HitTestSpellLoS));
                watch.Stop();
                RotationLogger.Trace($"{_name} target predicated evaluated to {could} on {target.Name} in {watch.ElapsedMilliseconds} ms");

                /* If the step predicate failed, fail the search */
                if (!could)
                {
                    return false;
                }

                watch.Restart();
                (bool should, bool consume) = _action.Should(target);
                watch.Stop();
                RotationLogger.Trace($"{_name} action should predicate evaluated to ({should}, {consume}) on {target.Name} in {watch.ElapsedMilliseconds} ms");

                /* Consume the token, in order to avoid letting other actions override the effects of this action */
                if (consume)
                {
                    exclusives.Add(target, Exclusive);
                }

                /* If the spell could be casted, but the action deems it not necessary (i.e. a buff is already present),
                 * fail the search */
                return should;
            }
            catch (Exception e)
            {
                Logging.WriteError($"{e.Message}\n{e.StackTrace}", true);
                return false;
            }
        };

        public bool Execute(Exclusives exclusives)
        {
            try
            {
                // abort casting spells very early on to prevent any lagging due to poorly chosen conditions
                if (RotationCombatUtil.freeMove && _action is RotationSpell spell && spell.CastTime > 0.0 && Me.GetMove)
                {
                    return false;
                }

                //can't execute this because we can't stop the current cast to execute this
                if (!_forceCast && Me.IsCast)
                {
                    RotationLogger.Trace($"{_name} false because of IsCast.");
                    return false;
                }

                if (!_constantPredicate(_action))
                {
                    RotationLogger.Trace($"{_name} false because of constant predicate.");
                    return false;
                }

                Func<WoWUnit, bool> predicate = CreatePredicate(exclusives);

                var watch = System.Diagnostics.Stopwatch.StartNew();
                WoWUnit target = _targetFinder(predicate);
                watch.Stop();
                RotationLogger.Trace($"({_name}) targetFinder: {target?.Name} {watch.ElapsedMilliseconds} ms");

                if (target == null)
                {
                    return false;
                }

                // Check if the step has a forced timer
                if (_forcedTimer == null)
                {
                    _forcedTimer = _forcedTimerMs > 0 ? _forcedTimer = new Timer(_forcedTimerMs) : null;
                }
                else 
                {
                    if (!_forcedTimer.IsReady)
                    {
                        RotationLogger.Trace($"{_name} false because its forced timer is not ready.");
                        return false;
                    }
                    _forcedTimer.Reset();
                }

                watch.Restart();
                bool executed = _action.Execute(target, _forceCast);
                watch.Stop();
                RotationLogger.Trace($"({_name}) execute {executed}: {watch.ElapsedMilliseconds} ms");

                if (!executed)
                {
                    /* At this point, the token was consumed, but if the cast failed, allow the token to be consumed
                     * by other actions */
                    exclusives.Remove(target, Exclusive);
                }

                return executed;
            }
            catch (Exception e)
            {
                Logging.WriteError($"{e.Message}\n{e.StackTrace}", true);
                return false;
            }
        }

        public override string ToString() => $"[{_priority}] {_name}";

        public Exclusive Exclusive { get; }
        public bool PreventDoubleCast => _preventDoubleCast;
        public bool IgnoreGCD => _ignoreGCD;
    }
}