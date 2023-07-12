using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Linq;
using AIO.Helpers;
using AIO.Helpers.Caching;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using AIO.Lists;
using wManager.Wow.Enums;

static class Extension
{
    public static Dictionary<WoWClass, string> DefaultRotations = new Dictionary<WoWClass, string>()
    {
        { WoWClass.Paladin, nameof(Spec.Paladin_SoloRetribution) },
        { WoWClass.Shaman, nameof(Spec.Shaman_SoloEnhancement) },
        { WoWClass.Mage, nameof(Spec.Mage_SoloFrost) },
        { WoWClass.Warlock, nameof(Spec.Warlock_SoloAffliction) },
        { WoWClass.Druid, nameof(Spec.Druid_SoloFeral) },
        { WoWClass.DeathKnight, nameof(Spec.DK_SoloBlood) },
        { WoWClass.Priest, nameof(Spec.Priest_SoloShadow) },
        { WoWClass.Rogue, nameof(Spec.Rogue_SoloCombat) },
        { WoWClass.Warrior, nameof(Spec.Warrior_SoloFury) },
        { WoWClass.Hunter, nameof(Spec.Hunter_SoloBeastMastery) },
    };

    public static void RunAdaptive(this Timer timer, Action action, TimeSpan maxDuration, int factor = 8)
    {
        if (!timer.IsReady)
        {
            return;
        }
        var watch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            action();
        }
        finally
        {
            watch.Stop();
            var elapsed = watch.Elapsed;

            var nextDuration = elapsed.TotalMilliseconds * factor;
            if (nextDuration > maxDuration.TotalMilliseconds)
            {
                nextDuration = maxDuration.TotalMilliseconds;
            }

            timer.Reset(nextDuration);
        }
    }

    public static long CGetDamage(this WoWUnit unit) => unit.CMaxHealth() - unit.CHealth();

    public static bool CShouldHeal(this WoWUnit unit, Spell spell, double percentage, double factor = 1) {
        StatisticEntry statistic = spell.GetStatisticEntry();
        
        // Default behavior
        if (statistic == null || statistic.Count < 2 || statistic.Average < 1)
            return unit.CHealthPercent() < percentage;

        // Fallback safety net for low-level healing targets
        if (unit.CHealthPercent() < percentage * 0.5) return true;

        // Do precise healing
        return unit.CGetDamage() > statistic.Average * factor;
    }

    public static bool ContainsAtLeast<TSource>(this IEnumerable<TSource> source,
        Func<TSource, bool> predicate, int amount) {
        var count = 0;
        foreach (TSource sourceItem in source) {
            if (predicate(sourceItem))
                count++;
            if (count >= amount)
                return true;
        }

        return false;
    }

    public static bool HasPoisonDebuff()
    {
        bool hasPoisonDebuff = Lua.LuaDoString<bool>
            (@"for i=1,25 do 
	            local _, _, _, _, d  = UnitDebuff('player',i);
	            if d == 'Poison' then
                return true
                end
            end");
        return hasPoisonDebuff;
    }
    

    public static bool HasDiseaseDebuff()
    {
        bool hasDiseaseDebuff = Lua.LuaDoString<bool>
            (@"for i=1,25 do 
	            local _, _, _, _, d  = UnitDebuff('player',i);
	            if d == 'Disease' then
                return true
                end
            end");
        return hasDiseaseDebuff;
    }

    //Get Item Amount
    public static int GetItemQuantity(string itemName)
    {
        var execute =
            "local itemCount = 0; " +
            "for b=0,4 do " +
                "if GetBagName(b) then " +
                    "for s=1, GetContainerNumSlots(b) do " +
                        "local itemLink = GetContainerItemLink(b, s) " +
                        "if itemLink then " +
                            "local _, stackCount = GetContainerItemInfo(b, s)\t " +
                            "if string.find(itemLink, \"" + itemName + "\") then " +
                                "itemCount = itemCount + stackCount; " +
                            "end " +
                       "end " +
                    "end " +
                "end " +
            "end; " +
            "return itemCount; ";
        return Lua.LuaDoString<int>(execute);
    }

    public static string GetSpec()
    {
        var Talents = new Dictionary<string, int>();
        for (int i = 1; i <= 3; i++)
        {
            Talents.Add(
                Lua.LuaDoString<string>($"local name, iconTexture, pointsSpent = GetTalentTabInfo({i}); return name"),
                Lua.LuaDoString<int>($"local name, iconTexture, pointsSpent = GetTalentTabInfo({i}); return pointsSpent")
            );
        }
        var highestTalents = Talents.Max(x => x.Value);
        return Talents.FirstOrDefault(t => t.Value == highestTalents).Key.Replace(" ", "");
    }

    // Uses the first item found in your bags that matches any element from the list
    // Added Cooldowncheck for Items
    public static void UseFirstMatchingItem(List<string> list)
    {
        List<WoWItem> _bagItems = Bag.GetBagItem();
        foreach (WoWItem item in _bagItems)
        {
            if (list.Contains(item.Name) && GetItemCooldown(item.Name) <= 0)
            {
                ItemsManager.UseItemByNameOrId(item.Name);
                Main.Log("Using " + item.Name);
                return;
            }
        }
    }

    public static void AddSorted<T, TN>(this List<T> list, T item, Func<T, TN> pred) where TN : IComparable<TN>
    {
        if (list.Count == 0)
        {
            list.Add(item);
            return;
        }
        if (pred(list[list.Count - 1]).CompareTo(pred(item)) <= 0)
        {
            list.Add(item);
            return;
        }
        if (pred(list[0]).CompareTo(pred(item)) >= 0)
        {
            list.Insert(0, item);
            return;
        }
        int index = list.BinarySearch(item, new FuncComp<T, TN>(pred));
        if (index < 0)
            index = ~index;
        list.Insert(index, item);
    }

    // Checks if you have any of the listed items in your bags
    public static bool HaveOneInList(List<string> list)
    {
        List<WoWItem> _bagItems = Bag.GetBagItem();
        return _bagItems.Any(item => list.Contains(item.Name));
    }

    //Get Item Cooldown
    public static int GetItemCooldown(string itemName)
    {
        int entry = GetItemID(itemName);
        List<WoWItem> _bagItems = Bag.GetBagItem();
        if (_bagItems.Any(item => entry == item.Entry))
            return Lua.LuaDoString<int>("local startTime, duration, enable = GetItemCooldown(" + entry + "); " +
                                        "return duration - (GetTime() - startTime)");

        Main.Log("Couldn't find item" + itemName);
        return 0;
    }

    // Get item ID in bag from a string passed as argument (good to check CD)
    public static int GetItemID(string itemName) =>
        (from item in Bag.GetBagItem() where itemName.Equals(item.Name) select item.Entry).FirstOrDefault();

    public static bool HaveRangedWeaponEquipped => Me.GetEquipedItemBySlot(wManager.Wow.Enums.InventorySlot.INVSLOT_RANGED) != 0;
}

internal class FuncComp<T, TN> : IComparer<T> where TN : IComparable<TN>
{
    private readonly Func<T, TN> Pred;

    public FuncComp(Func<T, TN> pred)
    {
        Pred = pred;
    }

    public int Compare(T entry1, T entry2)
    {
        return Pred(entry1).CompareTo(Pred(entry2));
    }
}