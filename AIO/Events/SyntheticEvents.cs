using AIO.Combat.Common;
using robotManager.Events;
using robotManager.FiniteStateMachine;
using System.ComponentModel;
using AIO.Framework;
using static robotManager.Events.FiniteStateMachineEvents;
using wManager.Wow.Helpers;

namespace AIO.Events
{
    internal class SyntheticEvents : ICycleable
    {
        private const string DefaultCombatState = "InFight";
        private const string DefaultIdleState = "Idle";
        private const string DungeonCrawlerCombatState = "dCombat";
        private const string HealBotCombatState = "Healtarget";

        private static void OnBeforeCheckIfNeedToRunState(Engine engine, State state, CancelEventArgs cancelable)
        {
            if (engine?.States == null)
            {
                return;
            }
            for (var i = 0; i < engine.States.Count; i++)
            {
                var s = engine.States[i];
                if (s == null)
                {
                    continue;
                }
                switch (s.DisplayName)
                {
                    case DefaultIdleState:
                        OnIdleStateAvailable?.Invoke(engine, state, cancelable);
                        return;
                    default:
                        break;
                }
            }
        }

        private static void OnRunState(Engine engine, State state, CancelEventArgs cancelable)
        {
            switch (state?.DisplayName)
            {
                case DefaultCombatState:
                case HealBotCombatState:
                case DungeonCrawlerCombatState:
                    OnCombatStateRun?.Invoke(engine, state, cancelable);
                    break;
                default:
                    break;
            }
            if (state?.DisplayName.Contains("SmoothMove") ?? false)
                OnCombatStateRun?.Invoke(engine, state, cancelable);
        }

        public void Initialize()
        {
            FiniteStateMachineEvents.OnBeforeCheckIfNeedToRunState += OnBeforeCheckIfNeedToRunState;
            FiniteStateMachineEvents.OnRunState += OnRunState;
        }

        public void Dispose()
        {
            FiniteStateMachineEvents.OnBeforeCheckIfNeedToRunState -= OnBeforeCheckIfNeedToRunState;
            FiniteStateMachineEvents.OnRunState -= OnRunState;
        }

        public static event FSMEngineStateCancelableHandler OnCombatStateRun;

        public static event FSMEngineStateCancelableHandler OnIdleStateAvailable;
    }
}
