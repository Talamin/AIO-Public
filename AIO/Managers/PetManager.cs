using wManager.Wow.Helpers;

public static class PetManager
{
    #region Pet

    // Returns the index of the pet spell passed as argument
    public static int GetPetSpellIndex(string spellName)
    {
        int spellindex = Lua.LuaDoString<int>
            ($"for i=1,10 do " +
                "local name, _, _, _, _, _, _ = GetPetActionInfo(i); " +
                "if name == '" + spellName + "' then " +
                "return i " +
                "end " +
            "end");

        return spellindex;
    }

    // Returns the cooldown of the pet spell passed as argument
    public static int GetPetSpellCooldown(string spellName)
    {
        int _spellIndex = GetPetSpellIndex(spellName);
        return Lua.LuaDoString<int>("local startTime, duration, enable = GetPetActionCooldown(" + _spellIndex + "); return duration - (GetTime() - startTime)");
    }

    // Returns whether a pet spell is available (off cooldown)
    public static bool GetPetSpellReady(string spellName)
    {
        return GetPetSpellCooldown(spellName) <= 0;
    }

    // Casts the pet spell passed as argument
    public static void PetSpellCast(string spellName)
    {
        int spellIndex = GetPetSpellIndex(spellName);
        if (GetPetSpellReady(spellName))
        {
            Lua.LuaDoString("CastPetAction(" + spellIndex + ");");
        }
    }

    public static void PetSpellCastFocus(string spellName)
    {
        int spellIndex = GetPetSpellIndex(spellName);
        if (GetPetSpellReady(spellName))
        {
            Lua.LuaDoString($"CastPetAction({spellIndex}, \'focus\');");
        }
    }

    // Toggles Pet spell autocast (pass true as second argument to toggle on, or false to toggle off)
    public static void TogglePetSpellAuto(string spellName, bool toggle)
    {
        string spellIndex = GetPetSpellIndex(spellName).ToString();

        if (!spellIndex.Equals("0"))
        {
            bool autoCast = Lua.LuaDoString<bool>("local _, autostate = GetSpellAutocast(" + spellIndex + ", 'pet'); " +
                "return autostate == 1") || Lua.LuaDoString<bool>("local _, autostate = GetSpellAutocast('" + spellName + "', 'pet'); " +
                "return autostate == 1");

            if ((toggle && !autoCast) || (!toggle && autoCast))
            {
                Lua.LuaDoString("ToggleSpellAutocast(" + spellIndex + ", 'pet');");
                Lua.LuaDoString("ToggleSpellAutocast('" + spellName + "', 'pet');");
            }
        }
    }
    public static string GetCurrentWarlockPetLUA => Lua.LuaDoString<string>
        ($@"
            for i=1,10 do
                local name, _, _, _, _, _, _ = GetPetActionInfo(i);
                if name == 'Firebolt' then
                    return 'Imp';
                end
                if name == 'Torment' then
                    return 'Voidwalker';
                end
                if name == 'Anguish' then
                    return 'Felguard';
                end
                if name == 'Devour Magic' then
                    return 'Felhunter';
                end
            end
            return 'None';");
    #endregion
}

