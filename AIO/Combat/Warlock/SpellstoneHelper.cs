/*using System.Collections.Generic;
using System.Threading;
using wManager.Wow.Class;
using wManager.Wow.Helpers;

namespace AIO.Combat.Warlock
{
    internal static class SpellstoneHelper
    {
        private static readonly List<string> Spellstones = new List<string> {
            "Spellstone",
            "Greater Spellstone",
            "Major Spellstone",
            "Master Spellstone",
            "Demonic Spellstone",
            "Grand Spellstone"
        };

        private static readonly Spell CreateSpellstone = new Spell("Create Spellstone");

        private static bool HasEnchant => Lua.LuaDoString<bool>
                (@"local hasMainHandEnchant, _, _, _, _, _, _, _, _ = GetWeaponEnchantInfo()
                if (hasMainHandEnchant) then 
                    return '1'
                else
                    return '0'
                end");

        private static string FindSpellStone()
        {
            var bagItems = Bag.GetBagItem();
            foreach (var item in bagItems)
            {
                if (!Spellstones.Contains(item.Name))
                {
                    continue;
                }
                return item.Name;
            }
            return "";
        }
        
        public static void Refresh()
        {
            if (HasEnchant)
            {
                // Already enchanted.
                return;
            }

            var stoneName = FindSpellStone();
            if (stoneName == "")
            {
                if (!CreateSpellstone.KnownSpell || !CreateSpellstone.IsSpellUsable)
                {
                    // Cannot create stone at this time.
                    return;
                }

                if (Bag.GetContainerNumFreeSlotsNormalType < 1)
                {
                    // Not enough bag space to create the stone.
                    return;
                }

                if (ItemsManager.GetItemCountByIdLUA(6265) == 0)
                {
                    // No Soul Shard.
                    return;
                }

                CreateSpellstone.Launch();
                Usefuls.WaitIsCasting();

                stoneName = FindSpellStone();
                if (stoneName == "")
                {
                    // The cast failed.
                    return;
                }
            }

            ItemsManager.UseItemByNameOrId(stoneName);
            Thread.Sleep(10);
            Lua.RunMacroText("/use 16");
            Usefuls.WaitIsCasting();
        }
    }
}
*/
