using System.Collections.Generic;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public static class MageFoodManager
{
    private static List<WoWItem> _bagItems;
    private static Spell ConjureWater = new Spell("Conjure Water");
    private static Spell ConjureFood = new Spell("Conjure Food");
    private static Spell ConjureManaGem = new Spell("Conjure Mana Gem");
    private static Spell ConjureRefreshement = new Spell("Conjure Refreshment");
    public static string ManaStone = "";

    private static List<string> Drink()
    {
        return new List<string>
        {
            "Conjured Water",
            "Conjured Fresh Water",
            "Conjured Purified Water",
            "Conjured Spring Water",
            "Conjured Mineral Water",
            "Conjured Sparkling Water",
            "Conjured Crystal Water",
            "Conjured Mountain Spring Water",
            "Conjured Glacier Water",
            "Conjured Mana Biscuit"
        };
    }

    private static List<string> Food()
    {
        return new List<string>
        {
            "Conjured Muffin",
            "Conjured Bread",
            "Conjured Rye",
            "Conjured Pumpernickel",
            "Conjured Sourdough",
            "Conjured Sweet Roll",
            "Conjured Cinnamon Roll",
            "Conjured Croissant",
            "Conjured Mana Pie",
            "Conjured Mana Strudel"
        };
    }

    private static List<string> ManaStones()
    {
        return new List<string>
        {
            "Mana Agate",
            "Mana Jade",
            "Mana Citrine",
            "Mana Ruby",
            "Mana Emerald",
            "Mana Sapphire"
        };
    }

    public static void CheckIfEnoughFoodAndDrinks()
    {
        _bagItems = Bag.GetBagItem();
        int stacksWater = 0;
        int stacksFood = 0;
        if (Bag.GetContainerNumFreeSlotsNormalType <= 1)
        {
            return;
        }
        foreach (WoWItem item in _bagItems)
        {
            if (Drink().Contains(item.Name))
            {
                stacksWater += ItemsManager.GetItemCountByNameLUA(item.Name);
            }
            if (Food().Contains(item.Name))
            {
                stacksFood += ItemsManager.GetItemCountByNameLUA(item.Name);
            }
        }
        if (stacksWater < 10 && ConjureWater.IsSpellUsable && ConjureWater.KnownSpell)
        {
            ConjureWater.Launch();
            Usefuls.WaitIsCasting();
        }
        if (stacksFood < 10 && ConjureFood.IsSpellUsable && ConjureFood.KnownSpell && !ConjureRefreshement.KnownSpell)
        {
            ConjureFood.Launch();
            Usefuls.WaitIsCasting();
        }
    }

    public static void CheckIfThrowFoodAndDrinks()
    {
        if (!Fight.InFight)
        {
            _bagItems = Bag.GetBagItem();
            int bestDrink = 0;
            int bestFood = 0;
            foreach (WoWItem item in _bagItems)
            {
                if (Drink().Contains(item.Name))
                {
                    bestDrink = Drink().IndexOf(item.Name) > bestDrink ? Drink().IndexOf(item.Name) : bestDrink;
                }
                if (Food().Contains(item.Name))
                {
                    bestFood = Food().IndexOf(item.Name) > bestFood ? Food().IndexOf(item.Name) : bestFood;
                }
            }
            foreach (WoWItem item in _bagItems)
            {
                if (Drink().Contains(item.Name) && Drink().IndexOf(item.Name) < bestDrink)
                {
                    LuaDeleteItem(item.Name);
                }
                if (Food().Contains(item.Name) && Food().IndexOf(item.Name) < bestFood)
                {
                    LuaDeleteItem(item.Name);
                }
            }
        }
    }

    public static void CheckIfHaveManaStone()
    {
        if (!Fight.InFight && ManaStone == "")
        {
            _bagItems = Bag.GetBagItem();
            bool haveManaStone = false;
            foreach (WoWItem item in _bagItems)
            {
                if (ManaStones().Contains(item.Name))
                {
                    haveManaStone = true;
                    ManaStone = item.Name;
                }
            }

            if (!haveManaStone && Bag.GetContainerNumFreeSlotsNormalType > 1)
            {
                if (ConjureManaGem.KnownSpell)
                {
                    if (ConjureManaGem.IsSpellUsable)
                    {
                        ConjureManaGem.Launch();
                        Usefuls.WaitIsCasting();
                    }
                }
            }
        }
    }

    public static void UseManaStone()
    {
        ItemsManager.UseItemByNameOrId(ManaStone);
    }

    public static void LuaDeleteItem(string item)
    {
        Lua.LuaDoString("for bag = 0, 4, 1 do for slot = 1, 32, 1 do local name = GetContainerItemLink(bag, slot); " +
            "if name and string.find(name, \"" + item + "\") then PickupContainerItem(bag, slot); " +
            "DeleteCursorItem(); end; end; end", false);
    }
}