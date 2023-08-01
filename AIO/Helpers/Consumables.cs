/*using System.Collections.Generic;
using System.Windows.Media.Imaging;

public static class Consumables
{
    // Healthstones list
    public static List<string> HealthStones()
    {
        return new List<string>
        {
            "Minor Healthstone",
            "Lesser Healthstone",
            "Healthstone",
            "Greater Healthstone",
            "Major Healthstone",
            "Master Healthstone",
            "Fel Healthstone",
            "Demonic Healthstone"
        };
    }

    // Soulstones list
    public static List<string> Soulstones()
    {
        return new List<string>
        {
            "Minor Soulstone",
            "Lesser Soulstone",
            "Soulstone",
            "Greater Soulstone",
            "Major Soulstone",
            "Master Soulstone",
            "Demonic Soulstone"
        };
    }
    
    // Checks if we have a Healthstone
    public static bool HaveHealthstone()
    {
        if (Extension.HaveOneInList(HealthStones()))
            return true;
        return false;
    }

    // Checks if we have a Soulstone
    public static bool HaveSoulstone()
    {
        if (Extension.HaveOneInList(Soulstones()))
            return true;
        return false;
    }

    // Use Healthstone
    public static void UseHealthstone()
    {
        Extension.UseFirstMatchingItem(HealthStones());
    }

    // Use Soulstone
    public static void UseSoulstone()
    {
        Extension.UseFirstMatchingItem(Soulstones());
    }
}*/