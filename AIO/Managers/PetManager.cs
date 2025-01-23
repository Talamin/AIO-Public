using robotManager.Helpful;
using System.Threading;
using wManager.Wow.Helpers;
using static AIO.Constants;

public static class PetManager
{
    // Casts the pet spell if it's ready in one single call. Does not check for focus/mana
    public static void CastPetSpellIfReady(string spellName, bool onFocus = false)
    {
        string onFocusLua = onFocus ? "1" : "0";
        string lua = $@"
            local petSpellIndex = 0;
            for i=1, 10 do
                local name, _, _, _, _, _, _ = GetPetActionInfo(i);
                if name == ""{spellName}"" then
                    petSpellIndex = i;
                end
            end
            if petSpellIndex > 0 then
                local startTime, duration, enable = GetPetActionCooldown(petSpellIndex); 
                local coolDown = duration - (GetTime() - startTime)
                if coolDown <= 0 then
                    if {onFocusLua} == 1 then
                        CastPetAction(petSpellIndex, 'focus');
                    else
                        CastPetAction(petSpellIndex);
                    end
                    return true;
                end
            end
            return false;
        ";
        bool spellCast = Lua.LuaDoString<bool>(lua);

        if (spellCast)
        {
            Main.LogFight($"[Pet] {spellName}");
        }
    }

    // Toggles Pet spell autocast (pass true as second argument to toggle on, or false to toggle off)
    public static void TogglePetSpellAuto(string spellName, bool toggle)
    {
        string toggleLua = toggle ? "1" : "0";
        string lua = $@"
                local petSpellIndex = 0;
                local shouldToggleOn = {toggleLua} == 1;
                for i=1, 10 do
                    local name, _, _, _, _, _, _ = GetPetActionInfo(i);
                    if name == ""{spellName}"" then
                        petSpellIndex = i;
                    end
                end
                if petSpellIndex > 0 then
                    local _, autostate = GetSpellAutocast(""{spellName}"", 'pet');
                    if shouldToggleOn and not autostate then
                        ToggleSpellAutocast(""{spellName}"", 'pet');
                        return true;
                    end
                    if not shouldToggleOn and autostate then
                        ToggleSpellAutocast(""{spellName}"", 'pet');
                        return true;
                    end
                end
                return false;
            ";
        bool toggled = Lua.LuaDoString<bool>(lua);

        if (toggled)
        {
            Main.LogFight($"Toggled pet spell {spellName}");
        }
    }

    public static void PreventPetDoubleSummon()
    {
        Thread.Sleep(500);
        // Avoid occasional double summon
        while (Me.IsCast)
        {
            Thread.Sleep(500);
            if (IsPetAliveLUA)
            {
                Lua.LuaDoString("SpellStopCasting();");
                break;
            }
        }
    }

    public static bool IsPetAliveLUA => Lua.LuaDoString<bool>($@"for i=1,10 do
        local name, _, _, _, _, _, _ = GetPetActionInfo(i);
            if name ~= nil then
                return true;
            end
        end
        return false;
    ");

    public static string GetCurrentWarlockPetLUA => Lua.LuaDoString<string>($@"
            for i=1,10 do
                local name, _, _, _, _, _, _ = GetPetActionInfo(i);
                if name == 'Firebolt' or name == 'Fire Shield' or name == 'Blood Pact' then
                    return 'Imp';
                end
                if name == 'Torment' or name == 'Shadow Bulwark' or name == 'Consume Shadows' or name == 'Consuming Shadows' or name == 'Sacrifice' then
                    return 'Voidwalker';
                end
                if name == 'Anguish' or name == 'Intercept' or name == 'Demonic Pact' then
                    return 'Felguard';
                end
                if name == 'Devour Magic' or name == 'Shadow Bite' or name == 'Fel Intelligence' then
                    return 'Felhunter';
                end
                if name == 'Seduction' or name == 'Lash of Pain' or name == 'Soothing kiss' then
                    return 'Succubus';
                end
            end
            return 'None';
        ");
}

