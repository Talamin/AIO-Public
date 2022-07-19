using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using AIO.Helpers.Caching;
using AIO.Settings;
using robotManager.Helpful;
using robotManager.Helpful.Win32;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Helpers {
    public static class Hotkeys {
        [DllImport("user32.dll")]
        private static extern ushort GetAsyncKeyState(int vKey);

        private static bool _rangeCircleOn = false;
        
        public static void CheckKeyPress() {
            if ((GetAsyncKeyState((int) VK.NUMPAD1) & 1) != 0) ForceNewTank();
            if ((GetAsyncKeyState((int) VK.NUMPAD2) & 1) != 0) LogCurrentData();
            if ((GetAsyncKeyState((int) VK.NUMPAD3) & 1) != 0) ForceStatisticalData();
            if ((GetAsyncKeyState((int) VK.NUMPAD4) & 1) != 0) ToggleRangeCircles();
        }

        private static void PrintCircles() {
            Vector3 myPos = ObjectManager.Me.PositionWithoutType;
            Radar3D.DrawCircle(myPos, 40, Color.Yellow, alpha: 24);
        }

        private static void ToggleRangeCircles() {
            if (!_rangeCircleOn) {
                Logging.Write("Turning Range Circles on.");
                Radar3D.OnDrawEvent += PrintCircles;
                Radar3D.Pulse();
                _rangeCircleOn = true;
            } else {
                DisableRangeCircles();
            }
        }

        public static void DisableRangeCircles() {
            if (_rangeCircleOn) {
                Logging.Write("Turning Range Circles off.");
                _rangeCircleOn = false;
                Radar3D.OnDrawEvent -= PrintCircles;
            }
        }

        private static void ForceStatisticalData() {
            Logging.Write("Forcing statistical data.");
            var filePath = $"Settings/Hotswitch-{ObjectManager.Me.Name}-{Usefuls.RealmName}.txt";
            if (!File.Exists(filePath)) {
                Logging.Write("Creating new hotswitch file at " + filePath);
                File.Create(filePath);
            }

            foreach (string line in File.ReadLines(filePath)) {
                string[] parsedArgs = line.Split('=');
                if (parsedArgs.Length != 2) continue;

                var foundSpell = new Spell(parsedArgs[0], false);
                if (foundSpell.Id <= 0) continue;

                try {
                    var parsedInt = Convert.ToInt32(parsedArgs[1]);
                    Logging.Write(line);
                    CombatLogger.ForceAverage(foundSpell.Id, parsedInt);
                } catch {
                    // Just ignore it.
                    // I don't care if the user entered it wrong. He will see that it's missing
                }
            }
        }

        private static void LogCurrentData() {
            Logging.Write($"Current custom tank: < {PriestLevelSettings.Current.HolyCustomTank} >");
            Logging.Write(" ### Logging statistical values ### ");
            foreach (KeyValuePair<uint, StatisticEntry> entry in CombatLogger.GetDictionary()) {
                Logging.Write($"{new Spell(entry.Key).Name}: {entry.Value.Average} ({entry.Value.Count})");
            }
        }

        private static void ForceNewTank() {
            string tankName = ObjectManager.Me.TargetObject?.Name ?? "";
            PriestLevelSettings.Current.HolyCustomTank = tankName;
            Logging.Write($"[Hotkey] Set custom tank to: < {tankName} >");
        }
    }
}