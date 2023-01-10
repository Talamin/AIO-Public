using AIO.Settings;
using robotManager.Helpful;
using robotManager.Products;
using System.Collections.Generic;
using System.ComponentModel;
using wManager.Events;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Common
{
    internal abstract class BaseCombatClass : ICycleable
    {
        public abstract float Range { get; }

        private readonly BaseSettings Settings;
        private readonly Dictionary<string, BaseRotation> Specialisations;

        private BaseRotation FightRotation { get; set; }
        protected List<ICycleable> Addons { get; set; }

        public string Specialisation { get; private set; }

        internal BaseCombatClass(BaseSettings settings, Dictionary<string, BaseRotation> specialisations, params ICycleable[] addons)
        {
            Settings = settings;
            Specialisations = specialisations;
            Addons = new List<ICycleable>(addons);
        }

        public virtual void Initialize()
        {
            Specialisation = Me.Level < 10 ? "LowLevel" : Settings.ChooseRotation;
            FightRotation = Specialisations.TryGetValue(Specialisation, out BaseRotation spec) ? spec : null;

            if (FightRotation == null)
            {
                Logging.WriteError($"Fallback to Default Specialisation");
                FightRotation = Specialisations["Default"];
                //Products.ProductStop();                
            }
            else
            {
                Logging.Write($"Running {Specialisation} specialisation");
            }
            foreach (var addon in Addons)
            {
                addon.Initialize();
            }
            FightRotation.Initialize();
            FightEvents.OnFightStart += OnFightStart;
            FightEvents.OnFightLoop += OnFightLoop;
            FightEvents.OnFightEnd += OnFightEnd;
            MovementEvents.OnMovementPulse += OnMovementPulse;
            MovementEvents.OnMoveToPulse += OnMoveToPulse;
            OthersEvents.OnPathFinderFindPath += OnMovementCalculation;
            ObjectManagerEvents.OnObjectManagerPulsed += OnObjectManagerPulse;
        }

        public virtual void Dispose()
        {
            foreach (var addon in Addons)
            {
                addon.Dispose();
            }
            FightRotation?.Dispose();
            FightEvents.OnFightStart -= OnFightStart;
            FightEvents.OnFightLoop -= OnFightLoop;
            FightEvents.OnFightEnd -= OnFightEnd;
            MovementEvents.OnMovementPulse -= OnMovementPulse;
            MovementEvents.OnMoveToPulse -= OnMoveToPulse;
            OthersEvents.OnPathFinderFindPath -= OnMovementCalculation;
            ObjectManagerEvents.OnObjectManagerPulsed -= OnObjectManagerPulse;
        }

        protected virtual void OnFightStart(WoWUnit unit, CancelEventArgs cancelable) { }
        protected virtual void OnFightEnd(ulong guid) { }
        protected virtual void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable) { }
        protected virtual void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable) { }
        protected virtual void OnMoveToPulse(Vector3 point, CancelEventArgs cancelable) { }

        protected virtual void OnMovementCalculation(Vector3 @from, Vector3 to, string continentnamempq, CancelEventArgs cancelable) { }

        protected virtual void OnObjectManagerPulse() { }
    }
}
