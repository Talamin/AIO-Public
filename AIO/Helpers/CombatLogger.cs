using System;
using System.Collections.Generic;
using robotManager.Helpful;
using wManager.Wow.Class;
using wManager.Wow.ObjectManager;

namespace AIO.Helpers {
    public static class CombatLogger {
        // Access Data
        private const byte Type = 1;
        private const byte ActiveGuid = 2;

        // SPELL
        private const byte SpellId = 8;

        // SPELL_HEAL && SPELL_PERIODIC_HEAL
        private const byte AmountHealed = 11;

        // Data
        private static readonly object Locker = new object();
        private static readonly Dictionary<uint, StatisticEntry> _statistics = new Dictionary<uint, StatisticEntry>();

        public static void ParseCombatLog(string eventId, List<string> args) {
            if (!eventId.Equals("COMBAT_LOG_EVENT_UNFILTERED")
                || args.Count < 5
                || Convert.ToUInt64(args[ActiveGuid], 16) != ObjectManager.Me.Guid)
                return;

            // for (var i = 0; i < args.Count; i++) {
            //     string arg = args[i];
            //     Logging.Write($"Index {i}: {arg}");
            // }

            switch (args[Type]) {
                case "SPELL_HEAL":
                case "SPELL_PERIODIC_HEAL":
                    LogData(Convert.ToUInt32(args[SpellId]), Convert.ToInt32(args[AmountHealed]));
                    break;
            }
        }
        
        // public static void LogStatistics()

        public static double GetAverage(this Spell spell) => GetAverage(spell.Id);

        public static double GetAverage(uint spellId) => GetStatisticEntry(spellId).Average;


        public static StatisticEntry GetStatisticEntry(this Spell spell) => GetStatisticEntry(spell.Id);

        public static StatisticEntry GetStatisticEntry(uint spellId) {
            lock (Locker) {
                return _statistics.TryGetValue(spellId, out StatisticEntry entry) ? entry : null;
            }
        }

        public static Dictionary<uint, StatisticEntry> GetDictionary() {
            lock (Locker) {
                return _statistics;
            }
        }

        public static void ForceAverage(uint spellId, int average) {
            lock (Locker) {
                if (!_statistics.TryGetValue(spellId, out _))
                    _statistics.Add(spellId, new StatisticEntry(average));
                else
                    _statistics[spellId] = new StatisticEntry(average);
                _statistics[spellId].Add(average);
            }
        }

        private static void LogData(uint spellId, int amount) {
            lock (Locker) {
                if (!_statistics.TryGetValue(spellId, out StatisticEntry entry))
                    _statistics.Add(spellId, new StatisticEntry(amount));
                else
                    entry.Add(amount);
            }
        }
    }
}