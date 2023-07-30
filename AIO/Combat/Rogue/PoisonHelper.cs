using robotManager.Helpful;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Rogue
{
    internal static class PoisonHelper
    {
        private static readonly Dictionary<int, uint> InstantPoisonDictionary = new Dictionary<int, uint>
        {
            { 79, 43231 },
            { 73, 43230 },
            { 68, 21927 },
            { 60, 8928 },
            { 52, 8927 },
            { 44, 8926 },
            { 36, 6950 },
            { 28, 6949 },
            { 20, 6947 }
        };

        private static readonly Dictionary<int, uint> DeadlyPoisonDictionary = new Dictionary<int, uint>
        {
            { 80, 43233 },
            { 76, 43232 },
            { 70, 22054 },
            { 62, 22053 },
            { 30, 2892 },
            { 60, 20844 },
            { 54, 8985 },
            { 46, 8984 },
            { 38, 2893 }
        };


        private static bool hasMainHandEnchant => Lua.LuaDoString<bool>
            (@"local hasMainHandEnchant, _, _, _, _, _, _, _, _ = GetWeaponEnchantInfo()
            if (hasMainHandEnchant) then 
               return '1'
            else
               return '0'
            end");

        private static bool hasOffHandEnchant => Lua.LuaDoString<bool>
            (@"local _, _, _, _, hasOffHandEnchant, _, _, _, _ = GetWeaponEnchantInfo()
            if (hasOffHandEnchant) then 
               return '1'
            else
               return '0'
            end");

        private static bool hasoffHandWeapon => Lua.LuaDoString<bool>(@"local hasWeapon = OffhandHasWeapon()
            return hasWeapon");


        private static IEnumerable<uint> MP => InstantPoisonDictionary
            .Where(i => i.Key <= Me.Level && ItemsManager.HasItemById(i.Value))
            .OrderByDescending(i => i.Key)
            .Select(i => i.Value);

        private static IEnumerable<uint> OP => DeadlyPoisonDictionary
            .Where(i => i.Key <= Me.Level && ItemsManager.HasItemById(i.Value))
            .OrderByDescending(i => i.Key)
            .Select(i => i.Value);

        public static void CheckPoison()
        {
            if (!hasMainHandEnchant)
            {
                if (Me.Level >= 20 && Me.Level <= 29)
                {
                    if (OP.Any())
                    {
                        var MHPoison = OP.First();
                        if (Me.GetMove)
                        {
                            MovementManager.StopMoveTo(true, 1000);
                        }
                        Logging.Write("Trying to apply" + MHPoison);
                        ItemsManager.UseItem(MHPoison);
                        Thread.Sleep(100 + Usefuls.Latency);
                        Lua.RunMacroText("/use 16");
                        Lua.LuaDoString("StaticPopup1Button1:Click()");
                        Usefuls.WaitIsCasting();
                    }
                }
                if (Me.Level > 29)
                {
                    if (MP.Any())
                    {
                        var MHPoison = MP.First();
                        if (Me.GetMove)
                        {
                            MovementManager.StopMoveTo(true, 1000);
                        }
                        Logging.Write("Trying to apply" + MHPoison);
                        ItemsManager.UseItem(MHPoison);
                        Thread.Sleep(100 + Usefuls.Latency);
                        Lua.RunMacroText("/use 16");
                        Thread.Sleep(100 + Usefuls.Latency);
                        Lua.LuaDoString("StaticPopup1Button1:Click()");
                        Usefuls.WaitIsCasting();
                    }
                }
            }
            if (!hasOffHandEnchant && hasoffHandWeapon)
            {
                //Logging.Write("Missing Offhandhandpoison");
                if (OP.Any())
                {
                    var OHPoison = OP.First();
                    if (Me.GetMove)
                    {
                        MovementManager.StopMoveTo(true, 1000);
                    }
                    ItemsManager.UseItem(OHPoison);
                    Thread.Sleep(100 + Usefuls.Latency);
                    Lua.RunMacroText("/use 17");
                    Thread.Sleep(100 + Usefuls.Latency);
                    Lua.LuaDoString("StaticPopup1Button1:Click()");
                    Usefuls.WaitIsCasting();
                }
            }
        }
    }
}

