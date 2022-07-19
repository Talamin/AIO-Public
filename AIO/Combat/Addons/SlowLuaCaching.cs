using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers.Caching;
using robotManager.Helpful;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Addons {
    public class SlowLuaCaching : ICycleable {
        private readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        private static void Update() {
            var players = new List<WoWPlayer> {ObjectManager.Me};
            players.AddRange(Party.GetParty());
            players.AddRange(Party.GetRaidMembers());
            players = players.Where(player => player.Name.Length > 0).ToList();

            if (players.Count <= 0 || !Conditions.InGameAndConnected) return;

            string playerStringArray = "{" + string.Join(",", players.Select(player => $"'{player.Name}'")) + "}";

            var threats = Lua.LuaDoString<List<int>>($@"
            players = {playerStringArray}
            threats = {{}}
            for i = 1, {players.Count} do
                threat = UnitThreatSituation(players[i])
                if threat == nil then threat = 0 end
                table.insert(threats, threat)
            end
            return unpack(threats)");

            if (threats.Count != players.Count) {
                Logging.WriteError("Mismatch in SlowLua threat function. " +
                                   "If this is not being spammed, it can safely be ignored.");
                return;
            }

            lock (LuaCache.LockThreat) {
                LuaCache.UnitThreatSituations.Clear();
                for (var i = 0; i < players.Count; i++) {
                    try {
                        LuaCache.UnitThreatSituations.Add(players[i].GetBaseAddress, threats[i]);
                    } catch (ArgumentException) {
                        // Duplicate entry. This can happen if we are in a raid group. Our player may pop up twice.
                    }
                }
            }
        }

        public static void UpdateThreatSituations() {
            List<WoWPlayer> players = RotationFramework.PartyMembers.Where(player => player.Name.Length > 0).ToList();
            string playerStringArray = "{" + string.Join(",", players.Select(player => $"'{player.Name}'")) + "}";

            var threats = Lua.LuaDoString<List<int>>($@"
            players = {playerStringArray}
            threats = {{}}
            for i = 1, {players.Count} do
                threat = UnitThreatSituation(players[i])
                if threat == nil then threat = 0 end
                table.insert(threats, threat)
            end
            return unpack(threats)");

            if (threats.Count != players.Count) {
                Logging.WriteError("Mismatch in SlowLua threat function. " +
                                   "If this is not being spammed, it can safely be ignored.");
                return;
            }

            lock (LuaCache.LockThreat) {
                LuaCache.UnitThreatSituations.Clear();
                for (var i = 0; i < players.Count; i++) {
                    Logging.Write($"Adding {players[i].Name}");
                    LuaCache.UnitThreatSituations.Add(players[i].GetBaseAddress, threats[i]);
                }
            }
        }


        public void Initialize() {
            Task.Factory.StartNew(() => {
                while (!CancellationTokenSource.Token.IsCancellationRequested) {
                    Logging.WriteDebug("Starting SlowLuaCaching Thread!");
                    Task thread = Task.Factory.StartNew(() => {
                        while (!CancellationTokenSource.Token.IsCancellationRequested) {
                            try {
                                Update();
                            } catch (Exception e) {
                                Logging.WriteError("Something went wrong while updating the Lua Cache\n" + e.Message);
                            }

                            Thread.Sleep(10);
                        }
                    }, CancellationTokenSource.Token);
                    while (thread.Status != TaskStatus.RanToCompletion) {
                        Thread.Sleep(10);
                        // Logging.Write(thread.Status.ToString());
                        // foreach (KeyValuePair<uint, int> pair in LuaCache.UnitThreatSituations) {
                        //     Logging.Write($"{pair.Key}: {pair.Value}");
                        // }
                    }

                    Logging.WriteDebug("SlowLuaCaching has ran to completion.");
                    thread.Dispose();
                }
            }, CancellationTokenSource.Token);
        }

        public void Dispose() {
            CancellationTokenSource.Cancel();
        }
    }
}