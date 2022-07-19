using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public static class TankManager
{
    public static uint GetThreatStatus(WoWUnit target)
    {
        return Lua.LuaDoString<uint>($"return UnitThreatSituation(\"{target.Name}\");");
    }

    //Gives the difference in  Threat
    public static int GetAggroDifferenceFor(WoWUnit target, IEnumerable<WoWPlayer> partyMembers)
    {
        uint myThreat = GetThreatStatus(target);
        uint highestParty = (from p in partyMembers
                             let tVal = GetThreatStatus(p)
                             orderby tVal descending
                             select tVal).FirstOrDefault();

        int result = (int)myThreat - (int)highestParty;
        return result;
    }
}

