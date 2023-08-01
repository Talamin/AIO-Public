using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Helpers.Caching;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Common
{
    internal abstract class BaseRotation : ICycleable
    {
        private List<RotationStep> _combatRotation = new List<RotationStep>();
        private List<RotationStep> _oocRotation = new List<RotationStep>();
        private CancellationTokenSource _rotationToken;

        protected abstract List<RotationStep> Rotation { get; }

        public void Launch(List<IAddon> addons)
        {
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

            Task.Factory.StartNew(() =>
            {
                Main.Log("Rotation started");
                while (!_rotationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (//ObjectManager.Me.CIsResting() // this doesn't work because "Food" and "Drink" have different spell Ids depending on the level of food/drink
                            ObjectManager.Me.HaveBuff("Food")
                            || ObjectManager.Me.HaveBuff("Drink")
                            || ObjectManager.Me.IsDead
                            || !Conditions.InGameAndConnectedAndProductStartedNotInPause)
                            continue;

                        if (Fight.InFight || ObjectManager.Me.InCombat)
                        {
                            RotationFramework.RunRotation("Combat Rotation", _combatRotation);
                        }
                        else
                        {
                            RotationFramework.RunRotation("OOC Rotation", _oocRotation);
                        }
                        Thread.Sleep(50);
                    }
                    catch (Exception e)
                    {
                        Logging.WriteError($"{e.Message}\n{e.StackTrace}", true);
                    }
                }
            }, _rotationToken.Token);
        }

        public virtual void Initialize()
        {
            _rotationToken = new CancellationTokenSource();
        }

        public virtual void Dispose()
        {
            _rotationToken?.Cancel();
            Main.Log("Rotation disposed");
        }
    }
}
