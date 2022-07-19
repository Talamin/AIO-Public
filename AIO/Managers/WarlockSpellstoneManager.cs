using System.Collections.Generic;
using System.Threading;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public static class WarlockSpellstoneManager
{
    private static List<WoWItem> _bagItems;
    private static Spell CreateSpellstone = new Spell("Create Spellstone");
    private static string SpellstoneinBag = "";
    private static bool haveSpellstone;
    private static List<string> Spellstones()
    {
        return new List<string>
        {
            "Spellstone",
            "Greater Spellstone",
            "Major Spellstone",
            "Master Spellstone",
            "Demonic Spellstone",
            "Grand Spellstone"
        };
    }


    private static void CheckIfHaveManaStone()
    {
        if (!Fight.InFight && ItemsManager.GetItemCountByIdLUA(6265) > 0)
        {
            _bagItems = Bag.GetBagItem();
            haveSpellstone = false;
            foreach (WoWItem item in _bagItems)
            {
                if (Spellstones().Contains(item.Name))
                {
                    haveSpellstone = true;
                    SpellstoneinBag = item.Name;
                }
            }

            if (!haveSpellstone && Bag.GetContainerNumFreeSlotsNormalType > 1)
            {
                if (CreateSpellstone.KnownSpell)
                {
                    if (CreateSpellstone.IsSpellUsable)
                    {
                        CreateSpellstone.Launch();
                        Usefuls.WaitIsCasting();
                    }
                }
            }
        }
    }

    private static void UseSpellStone()
    {
        bool hasMainHandEnchant = Lua.LuaDoString<bool>
            (@"local hasMainHandEnchant, _, _, _, _, _, _, _, _ = GetWeaponEnchantInfo()
            if (hasMainHandEnchant) then 
               return '1'
            else
               return '0'
            end");
        if (!hasMainHandEnchant && haveSpellstone)
        {
            ItemsManager.UseItemByNameOrId(SpellstoneinBag);
            Thread.Sleep(10);
            Lua.RunMacroText("/use 16");
            Usefuls.WaitIsCasting();
        }
    }

}