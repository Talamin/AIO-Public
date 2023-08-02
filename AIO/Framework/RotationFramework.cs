using AIO.Combat.Common;
using AIO.Helpers.Caching;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Events;
using wManager.Wow;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using Math = System.Math;

namespace AIO.Framework
{
    public class RotationFramework : ICycleable
    {
        public static event EventHandler OnCacheUpdated;
        public static bool CacheDirectTransmission = false;
        //public static bool UseSynthetic = false;

        private static bool UseFramelock = true;

        private static int ScanRange = 50;

        private static int LoSCreditsPlayers = 5;
        private static int LoSCreditsNPCs = 10;

        public static void Setup(bool framelock = true, int losCreditsPlayers = 5, int losCreditsNPCs = 10, int scanRange = 50)
        {
            UseFramelock = framelock;
            ScanRange = scanRange;
            LoSCreditsPlayers = losCreditsPlayers;
            LoSCreditsNPCs = losCreditsNPCs;
        }

        public void Initialize()
        {
            LuaCache.UpdateRaidGroups();
            DetectHealAndTank();
            ObjectManagerEvents.OnObjectManagerPulsed += OnObjectManagerPulsed;
            EventsLuaWithArgs.OnEventsLuaStringWithArgs += OnEventsLuaStringWithArgs;
        }

        public void Dispose()
        {
            ObjectManagerEvents.OnObjectManagerPulsed -= OnObjectManagerPulsed;
            EventsLuaWithArgs.OnEventsLuaStringWithArgs -= OnEventsLuaStringWithArgs;
        }

        private readonly TimeSpan UpdateCacheMaxDelay = new TimeSpan(hours: 0, minutes: 0, seconds: 3);
        private readonly Timer UpdateTimer = new Timer();

        private void OnObjectManagerPulsed()
        {
            if (CacheDirectTransmission)
            {
                Run(UpdateCache);
            }
            else
            {
                UpdateTimer.RunAdaptive(
                    () => Run(UpdateCache), UpdateCacheMaxDelay
                );
            }
        }

        public void OnEventsLuaStringWithArgs(string eventId, List<string> args)
        {
            switch (eventId)
            {
                case "PLAYER_ENTERING_WORLD":
                case "INSTANCE_BOOT_START":
                case "WORLD_MAP_UPDATE":
                case "PARTY_MEMBERS_CHANGED":
                case "GROUP_ROSTER_CHANGED":
                case "PARTY_MEMBER_DISABLE":
                case "PARTY_MEMBER_ENABLE":
                    DetectHealAndTank();
                    break;
                case "RAID_ROSTER_UPDATE":
                    LuaCache.UpdateRaidGroups();
                    break;
            }
        }

        private void DetectHealAndTank()
        {
            if (Me.IsInGroup)
            {
                string[] luaResult = Lua.LuaDoString<string[]>($@"
                    local result = {{}};
                    local tankName = '';
                    local healName = '';
                    local group = {{""party1"", ""party2"", ""party3"", ""party4"", ""player""}};
                    for i = 1, #group do
                      local isTank, isHeal,_ = UnitGroupRolesAssigned(group[i]);
                        if isTank then                                    
                            tankName = UnitName(group[i]);
                        end
                        if isHeal then                                    
                            healName = UnitName(group[i]);
                        end    
                    end
                    table.insert(result, tankName);
                    table.insert(result, healName);
                    return unpack(result);
                ");

                if (luaResult.Length != 2) return;

                string tankName = luaResult[0];
                string healName = luaResult[1];

                if (tankName != "" && tankName != TankName)
                    Main.Log($"Group tank found: {tankName}");
                if (healName != "" && healName != HealName)
                    Main.Log($"Group healer found: {healName}");

                TankName = tankName;
                HealName = healName;
            }
        }

        private static void UpdateCache()
        {
            List<WoWUnit> omUnits = ObjectManager.GetObjectWoWUnit();
            List<WoWPlayer> omPlayers = ObjectManager.GetObjectWoWPlayer();

            Vector3 myPosition = Me.PositionWithoutType;

            int losCreditsPlayers = LoSCreditsPlayers;
            int losCreditsMonsters = LoSCreditsNPCs;

            var allUnits = new List<WoWUnit>(capacity: omUnits.Count + omPlayers.Count);
            for (var i = 0; i < omUnits.Count; i++)
            {
                WoWUnit unit = omUnits[i];
                if (ScanRange != 0 && myPosition.DistanceTo(unit.PositionWithoutType) > ScanRange)
                {
                    continue;
                }
                if (losCreditsMonsters > 0)
                {
                    losCreditsMonsters--;
                    if (TraceLine.TraceLineGo(unit.Position))
                    {
                        continue;
                    }
                }
                allUnits.AddSorted(unit, u => myPosition.DistanceTo(u.PositionWithoutType));
            }

            var players = new List<WoWPlayer>(capacity: omPlayers.Count + 1) { Me };
            for (var i = 0; i < omPlayers.Count; i++)
            {
                WoWPlayer player = omPlayers[i];
                if (ScanRange != 0 && myPosition.DistanceTo(player.PositionWithoutType) > ScanRange)
                {
                    continue;
                }
                if (losCreditsPlayers > 0)
                {
                    losCreditsPlayers--;
                    if (TraceLine.TraceLineGo(player.Position))
                    {
                        continue;
                    }
                }
                allUnits.AddSorted(player, p => myPosition.DistanceTo(p.PositionWithoutType));
                players.AddSorted(player, p => myPosition.DistanceTo(p.PositionWithoutType));
            }

            var enemies = new List<WoWUnit>(capacity: allUnits.Count);
            for (var i = 0; i < allUnits.Count; i++)
            {
                WoWUnit unit = allUnits[i];
                if (!unit.IsEnemy() || (/*!UseSynthetic && */!unit.IsAttackable))
                // if(!unit.IsAttackable)
                {
                    continue;
                }
                enemies.AddSorted(unit, u => myPosition.DistanceTo(u.PositionWithoutType));
            }

            List<ulong> guidHomeAndInstance = Party.GetPartyGUIDHomeAndInstance();

            var partyMembers = new List<WoWPlayer>(capacity: players.Count);
            for (var i = 0; i < players.Count; i++)
            {
                WoWPlayer player = players[i];
                if (!guidHomeAndInstance.Contains(player.Guid) && !player.IsLocalPlayer)
                {
                    continue;
                }
                partyMembers.AddSorted(player, p => p.HealthPercent);
            }

            PlayerUnits = players.ToArray();
            Enemies = enemies.ToArray();
            AllUnits = allUnits.ToArray();
            PartyMembers = partyMembers.ToArray();

            if (CacheDirectTransmission) OnCacheUpdated?.Invoke(null, EventArgs.Empty);
        }

        public static string HealName { get; private set; } = "";
        public static string TankName { get; private set; } = "";

        public static WoWUnit[] AllUnits { get; private set; } = new WoWUnit[0];
        public static WoWUnit[] Enemies { get; private set; } = new WoWUnit[0];
        public static WoWPlayer[] PlayerUnits { get; private set; } = new WoWPlayer[0];
        public static WoWPlayer[] PartyMembers { get; private set; } = new WoWPlayer[0];

        public static void RunRotation(string caller, List<RotationStep> rotation, bool alreadyLocked = false)
        {
            //GetGlobalCooldown picked directly of Memory, needs approval.
            var globalCd = GetGlobalCooldown();
            var gcdEnabled = globalCd != 0;

            var watch = Stopwatch.StartNew();
            Run(() => RunRotation(rotation, gcdEnabled), alreadyLocked);
            watch.Stop();

            //if (watch.ElapsedMilliseconds > 64)
            //{
            //    Logging.WriteDebug($"[{caller}] Iteration took { watch.ElapsedMilliseconds } ms");
            //    RotationLogger.Debug($"[{caller}] Iteration took { watch.ElapsedMilliseconds } ms");
            //}
        }

        private static void Run(Action action, bool alreadyLocked = false)
        {
            if (alreadyLocked)
            {
                action();
                return;
            }

            if (UseFramelock)
            {
                RunInFrameLock(action);
            }
            else
            {
                action();
                //RunInLock(action);
            }
        }
        /*
        private static void RunInLock(Action action)
        {
            lock (ObjectManager.Locker)
            {
                action();
            }
        }
        */
        private static void RunInFrameLock(Action action)
        {
            lock (Memory.WowMemory.LockFrameLocker)
            {
                try
                {
                    Memory.WowMemory.LockFrame();
                    action();
                }
                finally
                {
                    Memory.WowMemory.UnlockFrame();
                }
            }
        }

        private static Dictionary<ushort, List<long>> Stats = new Dictionary<ushort, List<long>>();
        //private static ushort _ticks = 0;

        private static void PrintStats()
        {
            foreach (KeyValuePair<ushort, List<long>> stat
                in Stats.Where(stat => stat.Value.Max() >= 1))
            {
                Logging.Write($"--- Step {stat.Key + 1} ---\n" +
                              $"Average: {Math.Round(stat.Value.Average(), 2)}ms\n" +
                              $"Min: {stat.Value.Min()}ms\n" +
                              $"Max: {stat.Value.Max()}ms\n" +
                              $"Count: {stat.Value.Count} ticks");
            }

            Logging.Write($"Where step " +
                          $"{Stats.OrderByDescending(stat => stat.Value.Average()).First().Key + 1} " +
                          $"has the highest average and step " +
                          $"{Stats.OrderByDescending(stat => stat.Value.Max()).First().Key + 1} " +
                          $"has the highest maximum.");
        }

        private static void RunRotation(IReadOnlyList<RotationStep> rotation, bool gcdEnabled)
        {
            try
            {
                // _ticks = (ushort) (_ticks > 10000 ? 0 : _ticks + 1);
                // if(_ticks % 500 == 0) PrintStats();

                var exclusives = new Exclusives();

                // var watch = new Stopwatch();

                for (ushort i = 0; i < rotation.Count; i++)
                {
                    RotationStep step = rotation[i];
                    // watch.Restart();
                    try
                    {
                        if (step.Execute(gcdEnabled, exclusives))
                        {
                            break;
                        }
                    }
                    finally
                    {
                        // watch.Stop();
                        // if(!Stats.ContainsKey(i)) {
                        //     Stats.Add(i, new List<long> {watch.ElapsedMilliseconds});
                        // } else {
                        //     Stats[i].Add(watch.ElapsedMilliseconds);
                        // }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.WriteError($"{e.Message} \n{e.StackTrace}", true);
            }
        }

        public static int GetGlobalCooldown()
        {
            foreach (var spellCooldown in SpellCooldownTimeLeft())
            {
                if (spellCooldown.Duration == 1500)
                    return spellCooldown.TimeLeft;
            }

            return 0;
        }

        public static bool SpellReady(uint spellid)
        {
            return SpellCooldownTimeLeft(spellid) <= 0;
        }

        public static int SpellCooldownTimeLeft(uint spellid)
        {
            foreach (var spellCooldown in SpellCooldownTimeLeft())
            {
                if (spellCooldown.SpellId == spellid)
                    return spellCooldown.TimeLeft;
            }

            return 0;
        }
        public struct SpellCooldown
        {
            public uint SpellId;
            public int TimeLeft;
            public int StartTime;
            public int Duration;

            public SpellCooldown(uint spellId, int timeLeft, int startTime, int duration)
            {
                SpellId = spellId;
                TimeLeft = timeLeft;
                StartTime = startTime;
                Duration = duration;
            }
        }
        public static IEnumerable<SpellCooldown> SpellCooldownTimeLeft()
        {
            // based on https://www.ownedcore.com/forums/world-of-warcraft/world-of-warcraft-bots-programs/wow-memory-editing/248891-3-1-3-info-getting-spell-cooldowns-3.html#post1780758
            var now = Memory.WowMemory.Memory.ReadInt32(0xCD76AC);
            uint currentListObject = Memory.WowMemory.Memory.ReadPtr(0xD3F5AC + 8);

            while ((currentListObject != 0) && ((currentListObject & 1) == 0))
            {

                uint currentSpellId = Memory.WowMemory.Memory.ReadPtr(currentListObject + 8);

                int start = Memory.WowMemory.Memory.ReadInt32((currentListObject + 0x10));

                int cd1 = Memory.WowMemory.Memory.ReadInt32((currentListObject + 0x14));
                int cd2 = Memory.WowMemory.Memory.ReadInt32((currentListObject + 0x20));

                int length = cd1 + cd2;
                int globalLength = Memory.WowMemory.Memory.ReadInt32((currentListObject + 0x2C));

                int cdleft = System.Math.Max(System.Math.Max(length, globalLength) - (now - start), 0);

                if (cdleft > 0)
                {
                    yield return new SpellCooldown(currentSpellId, cdleft, start, System.Math.Max(length, globalLength));
                }

                currentListObject = Memory.WowMemory.Memory.ReadPtr(currentListObject + 4);
            }
        }
    }
}