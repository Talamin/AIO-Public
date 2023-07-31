using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Helpers.Caching
{
    public static class Cache
    {
        private const byte CacheSize = 12;
        private static readonly Dictionary<uint, object[]> Units = new Dictionary<uint, object[]>();
        private static bool _frameTimeCached;
        private static long _frameTime;
        private static bool _inGroup;
        private static bool _inGroupCached;

        private static readonly Func<WoWUnit, object>[] Access = {
            unit => unit.ManaPercentage, // 0 = ManaPercentage
            unit => BuffManager.GetAuras(unit.GetBaseAddress).ToArray(), // 1 = GetAuras
            unit => unit.InCombat, // 2 = InCombat
            unit => unit.Health, // 3 = Health
            unit => unit.IsAlive, // 4 = IsAlive
            unit => unit.PositionWithoutType, // 5 = PositionWithoutType
            unit => unit.Rage, // 6 = Rage
            unit => unit.IsTargetingMe, // 7 = IsTargetingMe
            unit => unit.CastingSpellId, // 8 = CastingSpellId
            unit => unit.IsTargetingMeOrMyPetOrPartyMember, // 9 = IsTargetingMeOrMyPetOrPartyMember
            unit => unit.MaxHealth, // 10 = MaxHealth
            unit => unit.Name // 11 = Name
        };

        // private static readonly Func<WoWUnit, object>[] Access = { // For debugging purposes
        //     unit => 100U, // 0 = ManaPercentage
        //     unit => null, // 1 = GetAuras
        //     unit => false, // 2 = InCombat
        //     unit => 100d, // 3 = HealthPercent
        //     unit => true, // 4 = IsAlive
        //     unit => 100f // 5 = GetDistance
        // };

        private static object[] GetCUnit(WoWObject unit)
        {
            uint uBaseAddress = unit.GetBaseAddress;

            if (!Units.TryGetValue(uBaseAddress, out object[] cache))
            {
                cache = new object[CacheSize];
                Units.Add(uBaseAddress, cache);
            }

            return cache;
        }

        private static T GetProperty<T>(this WoWUnit unit, Entry entry)
        {
            T propertyValue;
            var entryIndex = (int)entry;
            object[] unitCache = GetCUnit(unit);

            if (unitCache[entryIndex] != null)
            {
                propertyValue = (T)unitCache[entryIndex];
            }
            else
            {
                propertyValue = (T)Access[entryIndex](unit); // here
                unitCache[entryIndex] = propertyValue;
            }

            return propertyValue;
        }

        public static void Reset()
        {
            Units.Clear();
            _frameTimeCached = false;
            _inGroupCached = false;
        }

        public static long CGetFrameTime()
        {
            if (!_frameTimeCached)
            {
                _frameTime = Usefuls.FrameTime_GetCurTimeMs64();
                _frameTimeCached = true;
            }

            return _frameTime;
        }

        public static uint CManaPercentage(this WoWUnit unit) => unit.GetProperty<uint>(Entry.ManaPercentage);

        public static Aura[] CGetAuras(this WoWUnit unit) => unit.GetProperty<Aura[]>(Entry.GetAuras);

        public static bool CHaveBuff(this WoWUnit unit, string spellName) => unit.CGetAuraByName(spellName) != null;

        public static bool CInCombat(this WoWUnit unit) => unit.GetProperty<bool>(Entry.InCombat);

        public static long CHealth(this WoWUnit unit) => unit.GetProperty<long>(Entry.Health);

        public static long CMaxHealth(this WoWUnit unit) => unit.GetProperty<long>(Entry.MaxHealth);

        public static double CHealthPercent(this WoWUnit unit) => unit.CHealth() * 100.0 / unit.CMaxHealth();

        public static bool CIsAlive(this WoWUnit unit) => unit.GetProperty<bool>(Entry.IsAlive);

        public static float CGetDistance(this WoWUnit unit) => ObjectManager.Me.CGetPosition().DistanceTo(unit.CGetPosition());

        public static Vector3 CGetPosition(this WoWUnit unit) => unit.GetProperty<Vector3>(Entry.PositionWithoutType);

        public static uint CRage(this WoWUnit unit) => unit.GetProperty<uint>(Entry.Rage);

        public static string CName(this WoWUnit unit) => unit.GetProperty<string>(Entry.Name);

        public static bool CIsTargetingMe(this WoWUnit unit) => unit.GetProperty<bool>(Entry.IsTargetingMe);

        public static int CCastingSpellId(this WoWUnit unit) => unit.GetProperty<int>(Entry.CastingSpellId);

        public static bool CIsCast(this WoWUnit unit) => unit.CCastingSpellId() > 0;

        public static bool CCanInterruptCasting(this WoWUnit unit) => unit.CIsCast() &&
                                                                      Convert.ToBoolean(
                                                                          Memory.WowMemory.Memory.ReadByte(
                                                                              unit.GetBaseAddress + 2612U) & 8);

        public static bool CIsTargetingMeOrMyPetOrPartyMember(this WoWUnit unit) =>
            unit.GetProperty<bool>(Entry.IsTargetingMeOrMyPetOrPartyMember);

        public static bool CHaveMyBuff(this WoWUnit unit, string name)
        {
            List<uint> targetSpellIDs = SpellListManager.SpellIdByName(name);
            ulong myGuid = ObjectManager.Me.Guid;

            Aura[] auras = unit.CGetAuras();
            for (var i = 0; i < auras.Length; i++)
            {
                Aura aura = auras[i];
                if (aura.Owner == myGuid && targetSpellIDs.Contains(aura.SpellId)) return true;
            }

            return false;
        }

        public static int CMyBuffStack(this WoWUnit unit, string name)
        {
            List<uint> targetSpellIDs = SpellListManager.SpellIdByName(name);
            ulong myGuid = ObjectManager.Me.Guid;

            Aura[] auras = unit.CGetAuras();
            for (var i = 0; i < auras.Length; i++)
            {
                Aura aura = auras[i];
                if (aura.Owner == myGuid && targetSpellIDs.Contains(aura.SpellId)) return aura.Stack;
            }

            return 0;
        }

        public static long CBuffTimeLeft(this WoWUnit unit, string name)
        {
            Aura aura = unit.CGetAuraByName(name);
            if (aura == null) return 0;

            long timeLeft = aura.TimeEnd - CGetFrameTime();
            return (timeLeft > 0) ? timeLeft : 0;
        }

        public static long CMyBuffTimeLeft(this WoWUnit unit, string name)
        {
            List<uint> targetSpellIDs = SpellListManager.SpellIdByName(name);
            ulong myGuid = ObjectManager.Me.Guid;

            Aura[] auras = unit.CGetAuras();
            for (var i = 0; i < auras.Length; i++)
            {
                Aura aura = auras[i];
                if (aura.Owner == myGuid && targetSpellIDs.Contains(aura.SpellId))
                {
                    long timeLeft = aura.TimeEnd - CGetFrameTime();

                    return (timeLeft > 0) ? timeLeft : 0;
                }
            }

            return 0;
        }

        public static Aura CGetAuraByName(this WoWUnit unit, string name)
        {
            List<uint> targetSpellIDs = SpellListManager.SpellIdByName(name);
            Aura[] auras = unit.CGetAuras();
            for (var i = 0; i < auras.Length; i++)
            {
                Aura aura = auras[i];
                if (targetSpellIDs.Contains(aura.SpellId)) return aura;
            }
            return null;
        }

        public static int CBuffStack(this WoWUnit unit, string name) => unit.CGetAuraByName(name)?.Stack ?? 0;

        public static bool CIsResting(this WoWUnit unit) => unit.CHaveBuff("Food") || unit.CHaveBuff("Drink");

        public static bool CIsInGroup(this WoWLocalPlayer player)
        {
            if (!_inGroupCached)
            {
                _inGroup = player.IsInGroup;
                _inGroupCached = true;
            }

            return _inGroup;
        }
    }
}