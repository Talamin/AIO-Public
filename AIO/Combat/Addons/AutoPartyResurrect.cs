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
    internal class AutoPartyResurrect : IAddon
    {
        private readonly Spell _resurectionSpell;
        private bool _isCastingRes = false;
        private Dictionary<ulong, DateTime> _blacklist = new Dictionary<ulong, DateTime>();
        private bool _runInCombat;
        private bool _runOutsideCombat;

        public bool RunOutsideCombat => _runOutsideCombat;
        public bool RunInCombat => _runInCombat;

        public List<RotationStep> Rotation => new List<RotationStep>();

        public AutoPartyResurrect(string ressurection, bool runInCombat = false, bool runOutsideCombat = true)
        {
            _resurectionSpell = new Spell(ressurection);
            _runInCombat = runInCombat;
            _runOutsideCombat = runOutsideCombat;
        }

        public void Dispose()
        {
            //SyntheticEvents.OnIdleStateAvailable -= OnIdleStateAvailable;
            MovementEvents.OnMovementPulse += OnMovementPulse;
            MovementEvents.OnMoveToPulse += OnMoveToPulse;
        }

        public void Initialize()
        {
            //SyntheticEvents.OnIdleStateAvailable += OnIdleStateAvailable;
        }

        private void Run()
        {
            if (!Me.IsInGroup
                || !_resurectionSpell.KnownSpell
                || RotationFramework.PartyMembers.Count(player => player.IsDead) <= 0)
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

            if (playerToResurrect != null && playerToResurrect.GetDistance < _resurectionSpell.MaxRange)
            {
                if (!_resurectionSpell.IsSpellUsable)
                {
                    return;
                }

                _isCastingRes = true;

                Logging.Write($"Resurrecting {playerToResurrect.Name}");
                Interact.InteractGameObject(playerToResurrect.GetBaseAddress);
                SpellManager.CastSpellByNameLUA(_resurectionSpell.Name);
                Thread.Sleep(500);
                while (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause
                    && ObjectManager.Me.CastingTimeLeft > 0)
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
            cancelable.Cancel = _isCastingRes;
        }
        private void OnMoveToPulse(Vector3 point, CancelEventArgs cancelable)
        {
            cancelable.Cancel = _isCastingRes;
        }
        private void OnIdleStateAvailable(Engine engine, State state, CancelEventArgs cancelable) => Run();
    }
}
