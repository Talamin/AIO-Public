using AIO.Events;
using AIO.Framework;
using robotManager.FiniteStateMachine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using wManager.Events;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using Timer = robotManager.Helpful.Timer;

namespace AIO.Combat.Common
{
    internal abstract class BaseRotation : ICycleable
    {
        private List<RotationStep> _rotation;
        protected abstract List<RotationStep> Rotation { get; }

        protected readonly bool RunInCombat = true;
        protected readonly bool RunOutsideCombat = false;
        protected readonly bool UseCombatSynthetics = false;
        protected readonly bool CompletelySynthetic = false;

        private readonly Timer IdleTimer = new Timer(new TimeSpan(0, 0, 0, 500));

        protected BaseRotation(bool runInCombat = true, bool runOutsideCombat = false, bool useCombatSynthetics = false, bool completelySynthetic = false)
        {
            RunInCombat = runInCombat;
            RunOutsideCombat = runOutsideCombat;
            UseCombatSynthetics = useCombatSynthetics;
            CompletelySynthetic = completelySynthetic;
        }

        public virtual void Initialize()
        {
            _rotation = Rotation;
            _rotation.Sort();

            if (CompletelySynthetic)
            {
                RotationFramework.CacheDirectTransmission = true;
                RotationFramework.OnCacheUpdated += TickRotation;
                return;
            }

            if (RunInCombat)
            {
                if (UseCombatSynthetics)
                {
                    SyntheticEvents.OnCombatStateRun += OnCombatStateRun;
                }
                else
                {
                    FightEvents.OnFightLoop += OnFightLoop;
                }
            }
            if (RunOutsideCombat)
            {
                SyntheticEvents.OnIdleStateAvailable += OnIdleStateAvailable;
            }
        }

        public virtual void Dispose()
        {
            if (CompletelySynthetic)
            {
                RotationFramework.OnCacheUpdated -= TickRotation;
                RotationFramework.CacheDirectTransmission = false;
                return;
            }

            if (RunInCombat)
            {
                if (UseCombatSynthetics)
                {
                    SyntheticEvents.OnCombatStateRun -= OnCombatStateRun;
                }
                else
                {
                    FightEvents.OnFightLoop -= OnFightLoop;
                }
            }
            if (RunOutsideCombat)
            {
                SyntheticEvents.OnIdleStateAvailable -= OnIdleStateAvailable;
            }
        }

        private string RotationName => GetType().FullName;

        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable) => RotationFramework.RunRotation(RotationName, _rotation);
        private void OnCombatStateRun(Engine engine, State state, CancelEventArgs cancelable) => RotationFramework.RunRotation(RotationName, _rotation);
        private void OnIdleStateAvailable(Engine engine, State state, CancelEventArgs cancelable)
        {
            if (Me.InCombatFlagOnly || !IdleTimer.IsReady)
            {
                return;
            }

            try
            {
                RotationFramework.RunRotation(RotationName, _rotation);
            }
            finally
            {
                IdleTimer.Reset();
            }
        }

        private void TickRotation(object sender, EventArgs e)
        {
            if (Conditions.ProductIsStartedNotInPause)
                RotationFramework.RunRotation(RotationName, _rotation, true);
        }
    }
}
