using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Helpers.Caching;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Threading;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Common
{
    internal abstract class BaseRotation : ICycleable
    {
        private List<RotationStep> _combatRotation = new List<RotationStep>();
        private List<RotationStep> _oocRotation = new List<RotationStep>();
        private bool _isRunning;

        protected abstract List<RotationStep> Rotation { get; }

        protected BaseRotation() { }

        public void Launch(List<IAddon> addons)
        {
            _isRunning = true;

            foreach (IAddon addon in addons)
            {
                addon.Initialize();
                if (addon.RunInCombat && addon.RunOutsideCombat)
                {
                    Main.Log($"Adding mixed addon {addon}");
                    _oocRotation.AddRange(addon.Rotation);
                    _combatRotation.AddRange(addon.Rotation);
                }
                else if (addon.RunOutsideCombat)
                {
                    Main.Log($"Adding OOC addon {addon}");
                    _oocRotation.AddRange(addon.Rotation);
                }
                else if (addon.RunInCombat)
                {
                    Main.Log($"Adding combat addon {addon}");
                    _combatRotation.AddRange(addon.Rotation);
                }
            }

            _combatRotation.AddRange(Rotation);
            _combatRotation.Sort();
            _oocRotation.Sort();

            Main.Log("Rotation start");
            while (_isRunning)
            {
                try
                {
                    if (ObjectManager.Me.CIsResting()
                        || ObjectManager.Me.IsDead
                        || !Conditions.InGameAndConnectedAndProductStartedNotInPause)
                        continue;

                    if (Fight.InFight)
                    {
                        RotationFramework.RunRotationNoLock("Combat Rotation", _combatRotation);
                    }
                    else
                    {
                        RotationFramework.RunRotationNoLock("OOC Rotation", _oocRotation);
                    }

                    Thread.Sleep(50);
                }
                catch (Exception e)
                {
                    Logging.WriteError($"{e.Message} \n{e.StackTrace}", true);
                }
            }
            Main.Log("Rotation stopped");
        }

        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
            _isRunning = false;
            Main.Log("Rotation disposed");
        }
    }
}
