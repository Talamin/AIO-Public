using AIO.Combat.Common;
using AIO.Events;
using AIO.Framework;
using robotManager.FiniteStateMachine;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
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
        private bool _isCastingRes = false;
        private Dictionary<ulong, DateTime> _blacklist = new Dictionary<ulong, DateTime>();

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
            MovementEvents.OnMovementPulse += OnMovementPulse;
            MovementEvents.OnMoveToPulse += OnMoveToPulse;
        }

        public void Initialize()
        {
            FightEvents.OnFightLoop += OnFightLoop;
            SyntheticEvents.OnIdleStateAvailable += OnIdleStateAvailable;
        }

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

            // Note: RotationFramework.PartyMember only returns party members in LoS
            WoWPlayer playerToResurrect = RotationFramework.PartyMembers
                .Where(player => player.IsDead && !_blacklist.ContainsKey(player.Guid))
                .OrderBy(player => player.PositionWithoutType.DistanceTo(ObjectManager.Me.Position))
                .FirstOrDefault();

            if (playerToResurrect != null && playerToResurrect.GetDistance < Ressurection.MaxRange)
            {
                if (!Ressurection.IsSpellUsable)
                {
                    return;
                }

                _isCastingRes = true;

                Logging.Write($"Resurrecting {playerToResurrect.Name}");
                Interact.InteractGameObject(playerToResurrect.GetBaseAddress);
                SpellManager.CastSpellByNameLUA(Ressurection.Name);
                Thread.Sleep(500);
                while (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && ObjectManager.Me.IsCast)
                {
                    WoWPlayer player = ObjectManager.GetObjectWoWPlayer().FirstOrDefault(o => o.Name == playerToResurrect.Name);
                    Thread.Sleep(100);
                    if (player == null || !player.IsDead)
                    {
                        Lua.LuaDoString("SpellStopCasting();");
                    }
                }

                _isCastingRes = false;

                if (!_blacklist.ContainsKey(playerToResurrect.Guid))
                {
                    _blacklist.Add(playerToResurrect.Guid, DateTime.Now.AddMinutes(1));
                }
            }
        }

        private void OnMovementPulse(List<Vector3> path, CancelEventArgs cancelable)
        {
            if (_isCastingRes)
            {
                cancelable.Cancel = true;
            }
        }
        private void OnMoveToPulse(Vector3 point, CancelEventArgs cancelable)
        {
            if (_isCastingRes)
            {
                cancelable.Cancel = true;
            }
        }
        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable) => Run();
        private void OnIdleStateAvailable(Engine engine, State state, CancelEventArgs cancelable) => Run();
    }
}
