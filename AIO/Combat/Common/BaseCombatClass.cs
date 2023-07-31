using AIO.Combat.Addons;
using AIO.Lists;
using AIO.Settings;
using robotManager.Helpful;
using robotManager.Products;
using System;
using System.Collections.Generic;
using wManager.Wow.Enums;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Common
{
    internal abstract class BaseCombatClass : ICycleable
    {
        private readonly BaseSettings _settings;
        private readonly BaseRotation _baseRotation;

        public abstract float Range { get; }
        protected List<IAddon> Addons { get; set; } = new List<IAddon>();
        public Spec Specialisation { get; private set; }

        internal BaseCombatClass(BaseSettings settings, Dictionary<Spec, BaseRotation> specialisations)
        {
            _settings = settings;
            WoWClass myClass = ObjectManager.Me.WowClass;
            Spec mysSpec;

            if (Enum.TryParse(_settings.ChooseRotation, out mysSpec))
            {
                Specialisation = mysSpec;
            }
            else
            {
                Logging.WriteError($"Couldn't find rotation {_settings.ChooseRotation}, setting back to default");
                Specialisation = (Spec)Enum.Parse(typeof(Spec), Extension.DefaultRotations[myClass]);
            }

            Specialisation = Me.Level < 10 ? Spec.LowLevel : Specialisation;
            _baseRotation = specialisations.TryGetValue(Specialisation, out BaseRotation spec) ? spec : null;

            if (_baseRotation == null)
            {
                if (!specialisations.ContainsKey(Spec.Fallback))
                {
                    Logging.WriteError($"ERROR: No fallback rotation has been defined in the the class rotation dictionary for {myClass}");
                    Products.DisposeProduct();
                    return;
                }
                Logging.WriteError($"WARNING: {Specialisation} is absent from the class rotation dictionary. Using fallback ({specialisations[Spec.Fallback]}).");
                _baseRotation = specialisations[Spec.Fallback];
            }
            else
            {
                Logging.Write($"Running {Specialisation} specialisation");
            }
        }

        public virtual void Initialize()
        {
            _baseRotation.Initialize();
            _baseRotation.Launch(Addons);
        }

        public virtual void Dispose()
        {
            foreach (IAddon addon in Addons)
            {
                addon.Dispose();
            }
            _baseRotation?.Dispose();
        }
    }
}
