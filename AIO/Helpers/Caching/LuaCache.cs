using System;
using System.Collections.Generic;
using System.Linq;
using robotManager.Helpful;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Helpers.Caching {
    public static class LuaCache {
        public static readonly object LockThreat = new object();
        private static readonly object LockGroup = new object();
        public static readonly Dictionary<uint, int> UnitThreatSituations = new Dictionary<uint, int>();
        private static readonly Dictionary<string, byte> RaidGroups = new Dictionary<string, byte>();
        
        public static int GetCachedThreatSituation(this WoWUnit unit) {
            lock (LockThreat) {
                return UnitThreatSituations.TryGetValue(unit.GetBaseAddress, out int threatSituation)
                    ? threatSituation
                    : -1;
            }
        }

        public static void UpdateRaidGroups() {
            var luaGroups = Lua.LuaDoString<List<string>>(@"
                outTable = {}
                for i=1, GetNumRaidMembers() do
                    name, _, subgroup, _, _, _, _, _, _, _, _ = GetRaidRosterInfo(i)
                    outTable[i] = name .. ""="" .. subgroup
                end
                return unpack(outTable)");

            lock (LockGroup) {
                RaidGroups.Clear();
                foreach (string[] split in luaGroups.Select(luaGroup
                    => luaGroup.Split('=')).Where(split => split.Length == 2)) {
                    RaidGroups.Add(split[0], Convert.ToByte(split[1]));
                }
            }
        }

        public static byte GetRaidGroup(this WoWPlayer unit) => GetRaidGroup(unit.Name);
        
        public static byte CGetRaidGroup(this WoWPlayer unit) => GetRaidGroup(unit.CName());
        
        public static byte GetRaidGroup(string name) {
            lock (LockGroup) {
                return RaidGroups.TryGetValue(name, out byte group) ? group : (byte) 0;
            }
        }
    }
}