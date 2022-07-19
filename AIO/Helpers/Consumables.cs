using System.Collections.Generic;

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

    // Checks if we have a Healthstone
    public static bool HaveHealthstone()
    {
        if (Extension.HaveOneInList(HealthStones()))
            return true;
        return false;
    }

    // Use Healthstone
    public static void UseHealthstone()
    {
        Extension.UseFirstMatchingItem(HealthStones());
    }
}