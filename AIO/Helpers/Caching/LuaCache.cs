using AIO.Framework;
using AIO.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Helpers.Caching
{
    public static class LuaCache
    {
        public static readonly object LockThreat = new object();
        private static readonly object LockGroup = new object();
        public static readonly Dictionary<uint, int> UnitThreatSituations = new Dictionary<uint, int>();
        private static readonly Dictionary<string, byte> RaidGroups = new Dictionary<string, byte>();

        public static int GetCachedThreatSituation(this WoWUnit unit)
        {
            lock (LockThreat)
            {
                return UnitThreatSituations.TryGetValue(unit.GetBaseAddress, out int threatSituation)
                    ? threatSituation
                    : -1;
            }
        }

        public static Dictionary<DebuffType, List<WoWUnit>> GetLUADebuffedPartyMembers()
        {
            Dictionary<DebuffType, List<WoWUnit>> cachedDebuffedPlayers = new Dictionary<DebuffType, List<WoWUnit>>();
            if (RotationFramework.PartyMembers.Length <= 0) return cachedDebuffedPlayers;

            string partyMembersforLua = string.Join(", ", RotationFramework.PartyMembers.Select(m => $"'{m.Name}'"));

            string lua = $@"
                local result = {{}};
                local playerNames = {{ {partyMembersforLua} }};
                for key,name in pairs(playerNames) do
                    for i=1,10 do
                        local _, _, _, _, debuffType, _, _ = UnitDebuff(name, i);
                        if (debuffType ~= nil and debuffType ~= '') then
                            table.insert(result, debuffType .. '$' .. name);
                        end
                    end
                end
                return unpack(result);
            ";

            string[] debuffed = Lua.LuaDoString<string[]>(lua);
            if (debuffed == null || debuffed.Length <= 0) return cachedDebuffedPlayers;

            foreach (string debuff in debuffed.Where(d => !string.IsNullOrEmpty(d)))
            {
                string[] debuffedPlayer = debuff.Split('$');
                string debuffType = debuffedPlayer[0];
                string playerName = debuffedPlayer[1];
                WoWPlayer debuffedWoWPlayer = RotationFramework.PartyMembers.FirstOrDefault(m => m.Name == playerName);
                if (debuffedWoWPlayer != null && Enum.TryParse(debuffType, out DebuffType dbType))
                {
                    if (cachedDebuffedPlayers.ContainsKey(dbType))
                        cachedDebuffedPlayers[dbType].Add(debuffedWoWPlayer);
                    else
                        cachedDebuffedPlayers.Add(dbType, new List<WoWUnit>() { debuffedWoWPlayer });
                }
            }

            return cachedDebuffedPlayers;
        }

        public static void UpdateRaidGroups()
        {
            var luaGroups = Lua.LuaDoString<List<string>>(@"
                outTable = {}
                for i=1, GetNumRaidMembers() do
                    name, _, subgroup, _, _, _, _, _, _, _, _ = GetRaidRosterInfo(i)
                    outTable[i] = name .. ""="" .. subgroup
                end
                return unpack(outTable)");

            lock (LockGroup)
            {
                RaidGroups.Clear();
                foreach (string[] split in luaGroups.Select(luaGroup
                    => luaGroup.Split('=')).Where(split => split.Length == 2))
                {
                    RaidGroups.Add(split[0], Convert.ToByte(split[1]));
                }
            }
        }

        public static byte GetRaidGroup(this WoWPlayer unit) => GetRaidGroup(unit.Name);

        public static byte CGetRaidGroup(this WoWPlayer unit) => GetRaidGroup(unit.CName());

        public static byte GetRaidGroup(string name)
        {
            lock (LockGroup)
            {
                return RaidGroups.TryGetValue(name, out byte group) ? group : (byte)0;
            }
        }
    }
}