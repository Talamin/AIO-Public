using robotManager.Helpful;
using System.Collections.Generic;
using wManager.Wow.Helpers;

namespace AIO.Combat.Hunter
{
    internal static class PetHelper
    {
        private static readonly Dictionary<string, List<string>> Buffet = new Dictionary<string, List<string>>() {
            {
                "Meat",
                new List<string> {
                    "Tough Jerky",
                    "Haunch of Meat",
                    "Mutton Chop",
                    "Wild Hog Shank",
                    "Cured Ham Steak",
                    "Roasted Quail",
                    "Smoked Talbuk Venison",
                    "Clefthoof Ribs",
                    "Salted Venison",
                    "Mead Basted Caribou",
                    "Mystery Meat",
                    "Red Wolf Meat"
            }},
            {
                "Fungus",
                new List<string> {
                    "Raw Black Truffle"
            }},
            {
                "Fish",
                new List<string> {
                    "Slitherskin Mackerel",
                    "Longjaw Mud Snapper",
                    "Bristle Whisker Catfish",
                    "Rockscale Cod",
                    "Striped Yellowtail",
                    "Spinefin Halibut",
                    "Sunspring Carp",
                    "Zangar Trout",
                    "Fillet of Icefin",
                    "Poached Emperor Salmon"
                }
            },
            {
                "Fruit",
                new List<string> {
                    "Shiny Red Apple",
                    "Tel'Abim Banana",
                    "Snapvine Watermelon",
                    "Goldenbark Apple",
                    "Heaven Peach",
                    "Moon Harvest Pumpkin",
                    "Deep Fried Plantains",
                    "Skethyl Berries",
                    "Telaari Grapes",
                    "Tundra Berries",
                    "Savory Snowplum"
                }
            },
            {
                "Bread",
                new List<string> {
                    "Tough Hunk of Bread",
                    "Freshly Baked Bread",
                    "Moist Cornbread",
                    "Mulgore Spice Bread",
                    "Soft Banana Bread",
                    "Homemade Cherry Pie",
                    "Mag'har Grainbread",
                    "Crusty Flatbread"
                }
            }
        };

        public static string FoodType => Lua.LuaDoString<string>("return GetPetFoodTypes();", "");

        public static int Happiness => Lua.LuaDoString<int>("happiness, damagePercentage, loyaltyRate = GetPetHappiness() return happiness", "");

        public static void Feed()
        {
            var type = FoodType;
            foreach (var entry in Buffet)
            {
                if (!type.Contains(entry.Key))
                {
                    continue;
                }

                foreach (var food in entry.Value)
                {
                    if (ItemsManager.GetItemCountByNameLUA(food) == 0)
                    {
                        continue;
                    }

                    Lua.LuaDoString("CastSpellByName('Feed Pet')", false);
                    Lua.LuaDoString($"UseItemByName('{ food }')", false);
                    Logging.WriteFight($"[RTF] Feeding hungry Pet");
                    return;
                }
            }
        }
    }
}
