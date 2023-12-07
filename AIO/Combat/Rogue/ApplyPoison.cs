using AIO.Combat.Addons;
using AIO.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Rogue
{
    internal class ApplyPoison : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        public List<RotationStep> Rotation => new List<RotationStep>
        {
            new RotationStep(new RotationAction("Apply poisons", ApplyPoisons), 0f, 5000)
        };

        public void Initialize() { }
        public void Dispose() { }

        private void Applypoison(uint poisonId, bool mainHand)
        {
            string slot = mainHand ? "MainHandSlot" : "SecondaryHandSlot";
            string poisonName = Lua.LuaDoString<string>($@"
                local poisonName = GetItemInfo({poisonId});
                return poisonName;
            ");
            Main.Log($"Applying {poisonName} ({poisonId}) on {slot}");
            MovementManager.StopMove();
            MovementManager.StopMoveTo(false, 1000);
            ItemsManager.UseItem(poisonName);
            Lua.LuaDoString($@"
                PickupInventoryItem(GetInventorySlotInfo(""{slot}""));
                StaticPopup1Button1:Click();
            ");
            Usefuls.WaitIsCasting();
            Thread.Sleep(1000); // avoid double casts
        }

        private bool ApplyPoisons()
        {
            string[] luaResult = Lua.LuaDoString<string[]>($@"
                local result = {{}};
                local hasMainHandEquipped = GetInventoryItemID(""player"", GetInventorySlotInfo(""MainHandSlot"")) ~= nil;
                local hasOffHandEquipped = GetInventoryItemID(""player"", GetInventorySlotInfo(""SecondaryHandSlot"")) ~= nil;
                local hasMainHandEnchant, mainHandExpiration, _, hasOffHandEnchant, offHandExpiration, _ = GetWeaponEnchantInfo();

                local remainingTimeMain = 0;
                local remainingTimeOff = 0;
                if hasMainHandEquipped and hasMainHandEnchant then
                    remainingTimeMain = mainHandExpiration;
                end
                if hasOffHandEquipped and hasOffHandEnchant then
                    remainingTimeOff = offHandExpiration;
                end

                table.insert(result, hasMainHandEquipped);
                table.insert(result, hasOffHandEquipped);
                table.insert(result, remainingTimeMain);
                table.insert(result, remainingTimeOff);

                return unpack(result);
            ");

            if (luaResult.Count() != 4) return false;

            bool hasMainHandEquipped = bool.Parse(luaResult[0]);
            bool hasOffHandEquipped = bool.Parse(luaResult[1]);
            int timeRemainingMain = int.Parse(luaResult[2]) / 60000; // remaining minutes
            int timeRemainingoff = int.Parse(luaResult[3]) / 60000; // remaining minutes

            uint usableDeadlyPoison = 0;
            uint usableInstantPoison = 0;

            List<WoWItem> allItems = Bag.GetBagItem();

            foreach (KeyValuePair<int, uint> instantPoison in _instantPoisonDictionary)
            {
                if (instantPoison.Key <= Me.Level && allItems.Exists(item => item.Entry == instantPoison.Value))
                {
                    usableInstantPoison = instantPoison.Value;
                    break;
                }
            }

            foreach (KeyValuePair<int, uint> deadlyPoison in _deadlyPoisonDictionary)
            {
                if (deadlyPoison.Key <= Me.Level && allItems.Exists(item => item.Entry == deadlyPoison.Value))
                {
                    usableDeadlyPoison = deadlyPoison.Value;
                    break;
                }
            }

            if (hasMainHandEquipped && timeRemainingMain < 5 && usableInstantPoison > 0)
            {
                Applypoison(usableInstantPoison, true);
                return true;
            }

            if (hasOffHandEquipped && timeRemainingoff < 5)
            {
                if (usableDeadlyPoison > 0)
                {
                    Applypoison(usableDeadlyPoison, false);
                    return true;
                }

                if (usableInstantPoison > 0)
                {
                    Applypoison(usableInstantPoison, false);
                    return true;
                }
            }

            return false;
        }

        private readonly Dictionary<int, uint> _instantPoisonDictionary = new Dictionary<int, uint>
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

        private readonly Dictionary<int, uint> _deadlyPoisonDictionary = new Dictionary<int, uint>
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
    }
}
