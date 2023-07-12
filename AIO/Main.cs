using AIO.Combat.Common;
using AIO.Combat.DeathKnight;
using AIO.Combat.Druid;
using AIO.Combat.Hunter;
using AIO.Combat.Mage;
using AIO.Combat.Paladin;
using AIO.Combat.Priest;
using AIO.Combat.Rogue;
using AIO.Combat.Shaman;
using AIO.Combat.Warlock;
using AIO.Combat.Warrior;
using AIO.Events;
using AIO.Framework;
using AIO.Settings;
using robotManager.Helpful;
using robotManager.Products;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using AIO.Helpers;
using AIO.Lists;
using wManager;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using WholesomeWOTLKAIO;
using MarsSettingsGUI;

public class Main : ICustomClass {
    private readonly string version = FileVersionInfo.GetVersionInfo(Others.GetCurrentDirectory + @"\FightClass\" + wManager.wManagerSetting.CurrentSetting.CustomClass).FileVersion;
    public float Range => CombatClass?.Range ?? 5.0f;

    private CancellationTokenSource TokenRelativePositionFix;
    private CancellationTokenSource TokenKeyboardHook;

    private static BaseSettings CombatSettings {
        get {
            switch (Me.WowClass) {
                case WoWClass.Hunter:
                    return HunterLevelSettings.Current;
                case WoWClass.Paladin:
                    return PaladinLevelSettings.Current;
                case WoWClass.DeathKnight:
                    return DeathKnightLevelSettings.Current;
                case WoWClass.Warrior:
                    return WarriorLevelSettings.Current;
                case WoWClass.Priest:
                    return PriestLevelSettings.Current;
                case WoWClass.Warlock:
                    return WarlockLevelSettings.Current;
                case WoWClass.Druid:
                    return DruidLevelSettings.Current;
                case WoWClass.Mage:
                    return MageLevelSettings.Current;
                case WoWClass.Rogue:
                    return RogueLevelSettings.Current;
                case WoWClass.Shaman:
                    return ShamanLevelSettings.Current;
                default:
                    LogError("Class not supported.");
                    return null;
            }
        }
    }

    private static BaseCombatClass LazyCombatClass {
        get {
            switch (Me.WowClass) {
                case WoWClass.Hunter:
                    return new HunterBehavior();
                case WoWClass.Paladin:
                    return new PaladinBehavior();
                case WoWClass.DeathKnight:
                    return new DeathKnightBehavior();
                case WoWClass.Warrior:
                    return new WarriorBehavior();
                case WoWClass.Priest:
                    return new PriestBehavior();
                case WoWClass.Warlock:
                    return new WarlockBehavior();
                case WoWClass.Druid:
                    return new DruidBehavior();
                case WoWClass.Mage:
                    return new MageBehavior();
                case WoWClass.Rogue:
                    return new RogueBehavior();
                case WoWClass.Shaman:
                    return new ShamanBehavior();
                default:
                    LogError("Class not supported.");
                    Products.ProductStop();
                    return null;
            }
        }
    }

    private BaseCombatClass CombatClass;

    private readonly List<ICycleable> Components;

    public Main() => Components = new List<ICycleable> {
        new SyntheticEvents(),
        new RotationFramework(),
        new RacialManager(),
        new TalentsManager(),
        new DeferredCycleable(() => CombatClass)
    };

    private void ForceStepBackward(string name, List<string> ps) {
        if (name != "UI_ERROR_MESSAGE") {
            return;
        }

        switch (ps[0]) {
            case "Target needs to be in front of you.":
            case "Target too close":
                break;
            default:
                return;
        }
        switch (Me.WowClass)
        {
            case WoWClass.DeathKnight:
                if (DeathKnightLevelSettings.Current.Backwards)
                {
                    LogFight("Attempting to move backward due to wrong facet.");
                    Move.Backward(Move.MoveAction.PressKey, 300);
                }
                return;
            case WoWClass.Warrior:
                if (WarriorLevelSettings.Current.Backwards)
                {
                    LogFight("Attempting to move backward due to wrong facet.");
                    Move.Backward(Move.MoveAction.PressKey, 300);
                }
                return;
            default:
                return;
        }
    }

    private void ForceBindItem(string name, List<string> ps) {
        switch (name) {
            case "AUTOEQUIP_BIND_CONFIRM":
            case "EQUIP_BIND_CONFIRM":
            case "LOOT_BIND_CONFIRM":
            case "USE_BIND_CONFIRM":
                Usefuls.SelectGossipOption(1);
                return;
            default:
                break;
        }
    }

    private static void FixRelativePositionLag() {
        foreach (WoWUnit unit in ObjectManager.GetObjectWoWUnit()) {
            ulong transportGuid = unit.TransportGuid;
            if (transportGuid == 0) continue;
            var transportObject = ObjectManager.GetObjectByGuid(transportGuid);
            if (LaggyTransports.Entries.Contains(transportObject.Entry)) {
            // if(transportObject != null && transportObject.IsValid
            //                            && transportObject.FlagsInt == 40 // Transport & DoesNotDespawn
            //                            && transportObject.GOType == WoWGameObjectType.MoTransport){
                if (!WoWUnit.ForceRelativePosition) {
                    Logging.WriteDebug($"Forcing relative positions because {unit.Name} is on {transportObject.Name}.");
                    WoWUnit.ForceRelativePosition = true;
                }

                return;
            }
        }

        if (WoWUnit.ForceRelativePosition) {
            Logging.WriteDebug("Not forcing relative positions anymore.");
            WoWUnit.ForceRelativePosition = false;
        }
    }

    

    public void Initialize() {
        // Logging.Write("Started!");
        // Radar3D.OnDrawEvent += DrawArea;
        // Radar3D.Pulse();
        // Vector3 oldPosition = ClickToMove.GetClickToMovePosition();
        // while (Conditions.ProductIsStarted) {
        //     Vector3 newPosition = ClickToMove.GetClickToMovePosition();
        //     if (oldPosition.DistanceTo(newPosition) > 1) {
        //         Logging.Write("Found a new CTM at " + newPosition);
        //         _points.Add(newPosition);
        //         oldPosition = newPosition;
        //     }

        //     Thread.Sleep(100);
        // }

        // var watch = Stopwatch.StartNew();
        // while (Conditions.InGameAndConnectedAndProductStarted) {
        //     watch.Restart();
        //     for (int i = 0; i < 1000000; i++) {
        //         int test = Usefuls.Latency;
        //     }
        //     Logging.Write(watch.ElapsedMilliseconds.ToString());
        // }
        // 
        if (!Extension.DefaultRotations.ContainsKey(Me.WowClass))
        {
            Logging.WriteError($"ERROR: The default rotations dictionary (Helpers.Extension) doesn't contain an entry for {Me.WowClass}");
            Products.DisposeProduct();
            return;
        }

        AIOWOTLKSettings.Load();
        AutoUpdater.CheckUpdate(version);

        Log("Started " + version + " of FightClass");
        Log("Started " + version + " Discovering class and finding rotation...");
        if (Others.ParseInt(Information.Version.Replace(".", "").Substring(0, 3)) == 172) {
            Log($"AIO couldn't load (v {Information.Version})");
            return;
        }

        EventsLuaWithArgs.OnEventsLuaStringWithArgs += ForceBindItem;
        EventsLuaWithArgs.OnEventsLuaStringWithArgs += ForceStepBackward;
        EventsLuaWithArgs.OnEventsLuaStringWithArgs += CancelableSpell.CastStopHandler;
        EventsLuaWithArgs.OnEventsLuaStringWithArgs += CombatLogger.ParseCombatLog;

        wManagerSetting.CurrentSetting.UseLuaToMove = true;

        TokenRelativePositionFix = new CancellationTokenSource();
        Task.Factory.StartNew(() => {
            while (!TokenRelativePositionFix.IsCancellationRequested) {
                FixRelativePositionLag();
                Thread.Sleep(5000);
            }
        }, TokenRelativePositionFix.Token);
        
        TokenKeyboardHook = new CancellationTokenSource();
        Task.Factory.StartNew(() => {
            while (!TokenKeyboardHook.IsCancellationRequested) {
                Hotkeys.CheckKeyPress();
                Thread.Sleep(1000);
            }
        }, TokenKeyboardHook.Token);

        _ = CombatSettings;
        _ = CombatClass = LazyCombatClass;
        Components.ForEach(c => c?.Initialize());
    }

    

    public void Dispose() {
        // Radar3D.OnDrawEvent -= DrawArea;
        EventsLuaWithArgs.OnEventsLuaStringWithArgs -= ForceBindItem;
        EventsLuaWithArgs.OnEventsLuaStringWithArgs -= ForceStepBackward;
        EventsLuaWithArgs.OnEventsLuaStringWithArgs -= CancelableSpell.CastStopHandler;
        EventsLuaWithArgs.OnEventsLuaStringWithArgs -= CombatLogger.ParseCombatLog;

        TokenRelativePositionFix?.Cancel();
        TokenKeyboardHook?.Cancel();
        
        Hotkeys.DisableRangeCircles();

        Components.ForEach(c => c?.Dispose());
    }

    public void ShowConfiguration() => CombatSettings?.ShowConfiguration();

    private static string wowClass => Me.WowClass.ToString();
    private static bool _debug => false;

    public static void LogFight(string message) {
        Logging.Write($"[WOTLK - {wowClass}]: {message}", Logging.LogType.Fight, Color.ForestGreen);
    }

    public static void LogError(string message) {
        Logging.Write($"[WOTLK - {wowClass}]: {message}", Logging.LogType.Error, Color.DarkRed);
    }

    public static void Log(string message) {
        Logging.Write($"[WOTLK - {wowClass}]: {message}");
    }

    public static void LogDebug(string message) {
        if (_debug)
            Logging.WriteDebug($"[WOTLK - {wowClass}]: {message}");
    }
}