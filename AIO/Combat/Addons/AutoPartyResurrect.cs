using AIO.Combat.Common;
using AIO.Events;
using AIO.Framework;
using robotManager.FiniteStateMachine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using wManager.Events;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Addons
{
    internal class AutoPartyResurrect : ICycleable
    {
        private readonly Spell Ressurection;
        private readonly bool Combat;
        private readonly bool Enabled;

        public AutoPartyResurrect(string ressurection, bool combat = false, bool enabled = true)
        {
            Ressurection = new Spell(ressurection);
            Combat = combat;
            Enabled = enabled;
        }

        public void Dispose()
        {
            FightEvents.OnFightLoop -= OnFightLoop;
            SyntheticEvents.OnIdleStateAvailable -= OnIdleStateAvailable;
        }

        public void Initialize()
        {
            FightEvents.OnFightLoop += OnFightLoop;
            SyntheticEvents.OnIdleStateAvailable += OnIdleStateAvailable;
        }

        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable) => Run();

        private void OnIdleStateAvailable(Engine engine, State state, CancelEventArgs cancelable) => Run();

        private static Dictionary<ulong, DateTime> _blacklist = new Dictionary<ulong, DateTime>();

        private void Run()
        {
            if (Me.InCombatFlagOnly != Combat)
            {
                return;
            }
            if (Enabled == false)
            {
                return;
            }
            if (!Me.IsInGroup)
            {
                return;
            }

            if (!Ressurection.KnownSpell)
            {
                return;
            }

            _blacklist = _blacklist.Where(entry => entry.Value > DateTime.Now)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (var deadTarget in RotationFramework.PartyMembers.Where(o => !o.IsAlive && !_blacklist.ContainsKey(o.Guid)))
            {
                if (!Ressurection.IsSpellUsable)
                {
                    break;
                }

                if (TraceLine.TraceLineGo(deadTarget.Position) || deadTarget.GetDistance > Ressurection.MaxRange)
                {
                    MovementManager.MoveTo(deadTarget.Position);
                    break;
                }

                Ressurection.Launch();
                Interact.InteractGameObject(deadTarget.GetBaseAddress);
                Usefuls.WaitIsCasting();
                _blacklist.Add(deadTarget.Guid, DateTime.Now.AddMinutes(1));
            }
        }
    }
}
