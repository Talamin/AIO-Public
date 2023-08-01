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
        private readonly float Priority;
        private readonly IRotationAction Action;
        private readonly Func<IRotationAction, WoWUnit, bool> TargetPredicate;
        private readonly Func<IRotationAction, bool> ConstantPredicate;
        private readonly Func<Func<WoWUnit, bool>, WoWUnit> TargetFinder;
        private readonly bool ForceCast = false;
        private readonly bool CheckRange = true;
        private readonly bool CheckLoS = false;
        private readonly string Name;
        private readonly Timer ForcedTimer;

        public RotationStep(IRotationAction action,
            float priority,
            Func<IRotationAction, WoWUnit, bool> targetPredicate,
            Func<IRotationAction, bool> constantPredicate,
            Func<Func<WoWUnit, bool>, WoWUnit> targetFinder,
            Exclusive exclusive = null,
            bool forceCast = false,
            bool checkRange = true,
            bool checkLoS = false,
            int forcedTimerMS = 0)
        {
            Action = action;
            Priority = priority;
            TargetPredicate = targetPredicate;
            ConstantPredicate = constantPredicate;
            TargetFinder = targetFinder;
            Exclusive = exclusive;
            ForceCast = forceCast;
            CheckRange = checkRange;
            CheckLoS = checkLoS;

            Name = action.GetType().FullName;
            if (Action is RotationSpell spell)
            {
                Name = spell.Name;
            }

            if (forcedTimerMS > 0)
            {
                ForcedTimer = new Timer(forcedTimerMS);
            }
        }

        public RotationStep(IRotationAction action,
            float priority,
            Func<IRotationAction, WoWUnit, bool> targetPredicate,
            Func<Func<WoWUnit, bool>, WoWUnit> targetFinder,
            Exclusive exclusive = null,
            bool forceCast = false,
            bool checkRange = true,
            bool checkLoS = false,
            int forcedTimerMS = 0) :
            this(action, priority, targetPredicate, (_) => true, targetFinder, exclusive, forceCast, checkRange,
                checkLoS, forcedTimerMS)
        { }

        public int CompareTo(RotationStep other) => Priority.CompareTo(other.Priority);

        private Func<WoWUnit, bool> CreatePredicate(Exclusives exclusives) => (target) =>
        {
            try
            {
                /* If CheckRange is enabled, check the action range */
                if (CheckRange && target.GetDistance > Action.MaxRange)
                {
                    RotationLogger.Trace($"{Name} is out of range on {target.Name}");
                    return false;
                }

                /* If the unique token has been consumed for this target, fail the search */
                if (exclusives.Contains(target, Exclusive))
                {
                    RotationLogger.Trace($"{Name} has already had its token consumed on {target.Name}");
                    return false;
                }

                var watch = System.Diagnostics.Stopwatch.StartNew();
                bool correctTarget = TargetPredicate(Action, target);
                bool could = correctTarget && (!CheckLoS || target.IsLocalPlayer ||
                                               !TraceLine.TraceLineGo(ObjectManager.Me.PositionWithoutType,
                                                   target.PositionWithoutType, CGWorldFrameHitFlags.HitTestSpellLoS));
                watch.Stop();
                RotationLogger.Trace($"{Name} target predicated evaluated to {could} on {target.Name} in {watch.ElapsedMilliseconds} ms");

                /* If the step predicate failed, fail the search */
                if (!could)
                {
                    return false;
                }

                watch.Restart();
                (bool should, bool consume) = Action.Should(target);
                watch.Stop();
                RotationLogger.Trace($"{Name} action should predicate evaluated to ({should}, {consume}) on {target.Name} in {watch.ElapsedMilliseconds} ms");

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

        public bool Execute(bool globalActive, Exclusives exclusives)
        {
            try
            {
                // abort casting spells very early on to prevent any lagging due to poorly chosen conditions
                if (RotationCombatUtil.freeMove && Action is RotationSpell spell && spell.CastTime > 0.0 && Me.GetMove)
                {
                    return false;
                }

                //can't execute this, because global is still active
                //can't execute this because we can't stop the current cast to execute this
                if ((globalActive && !Action.IgnoresGlobal) || !ForceCast && Me.IsCast)
                {
                    RotationLogger.Trace($"{Name} false because of GCD or IsCast.");
                    return false;
                }

                //if (Fight.InFight && Name == "Shield Wall") Logging.WriteError($"{Priority} ConstantPredicate");
                bool constantEval = ConstantPredicate(Action);
                //if (Fight.InFight && Name == "Shield Wall") Logging.WriteError($"{Priority} constantEval {constantEval}");
                if (!constantEval)
                {
                    RotationLogger.Trace($"{Name} false because of constant predicate.");
                    return false;
                }

                Func<WoWUnit, bool> predicate = CreatePredicate(exclusives);

                var watch = System.Diagnostics.Stopwatch.StartNew();
                WoWUnit target = TargetFinder(predicate);
                watch.Stop();
                RotationLogger.Trace($"({Name}) targetFinder: {target?.Name} {watch.ElapsedMilliseconds} ms");

                if (target == null)
                {
                    return false;
                }

                // Check if the step has a forced timer
                if (ForcedTimer != null)
                {
                    if (!ForcedTimer.IsReady)
                    {
                        RotationLogger.Trace($"{Name} false because its forced timer is not ready.");
                        return false;
                    }
                    ForcedTimer.Reset();
                }

                watch.Restart();
                bool executed = Action.Execute(target, ForceCast);
                watch.Stop();
                RotationLogger.Trace($"({Name}) execute {executed}: {watch.ElapsedMilliseconds} ms");

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

        public override string ToString() => $"[{Priority}] {Name}";

        public Exclusive Exclusive { get; }
    }
}