using AIO.Framework;
using System.Collections.Generic;
using System.Threading;
using wManager.Wow.Helpers;

public class ItemsHelper
{
    //returns cooldown in seconds
    public static float GetItemCooldown(string itemName)
    {
        string luaString = $@"
        for bag=0,4 do
            for slot=1,36 do
                local itemLink = GetContainerItemLink(bag,slot);
                if (itemLink) then
                    local itemString = string.match(itemLink, ""item[%-?%d:]+"");
                    if (GetItemInfo(itemString) == ""{itemName}"") then
                        local start, duration, enabled = GetContainerItemCooldown(bag, slot);
                        if enabled == 1 and duration > 0 and start > 0 then
                            return (duration - (GetTime() - start));
                        end
                    end
                end;
            end;
        end
        return 0;";
        return Lua.LuaDoString<float>(luaString);
    }

    public static float GetItemCooldown(uint id)
    {
        return GetItemCooldown(ItemsManager.GetNameById(id));
    }

    public static void DeleteItems(uint itemID, int leaveAmount)
    {
        var itemQuantity = ItemsManager.GetItemCountById(itemID) - leaveAmount;
        RotationLogger.Debug($"Found Items to delete: {itemQuantity}");
        RotationLogger.Debug($"Found Items to delete: {itemID}");
        if (itemQuantity > 0) Delete((int)itemID);
    }
    public static void Delete(int item)
    {
        List<int> BagAndSlot = Bag.GetItemContainerBagIdAndSlot(item);
        Lua.LuaDoString($"PickupContainerItem({BagAndSlot[0]}, {BagAndSlot[1]}); DeleteCursorItem()", false);
        Thread.Sleep(10);
    }

    public static int GetItemCountSave(uint itemId)
    {
        int itemCount = ItemsManager.GetItemCountById(itemId);
        if (itemCount > 0)
        {
            return itemCount;
        }

        Thread.Sleep(250);
        return ItemsManager.GetItemCountById(itemId);
    }

    public static int GetItemCountSave(string itemName)
    {
        int itemCount = GetItemCount(itemName);
        if (itemCount > 0)
        {
            return itemCount;
        }

        Thread.Sleep(250);
        return GetItemCount(itemName);
    }

    public static int GetItemCount(string itemName)
    {
        string countLua = $@"
        local fullCount = 0;
        for bag=0,4 do
            for slot=1,36 do
                local itemLink = GetContainerItemLink(bag, slot);
                if (itemLink) then
                    local itemString = string.match(itemLink, ""item[%-?%d:]+"");
                    if (GetItemInfo(itemString) == ""{itemName}"") then
                        local texture, count = GetContainerItemInfo(bag, slot);
                        fullCount = fullCount + count;
                    end
                end
            end
        end
        return fullCount;";
        return Lua.LuaDoString<int>(countLua);
    }

    // Count the amount of the specified item stacks in your bags
    public static int CountItemStacks(string itemArg)
    {
        return Lua.LuaDoString<int>("local count = GetItemCount('" + itemArg + "'); return count");
    }

    // Deletes item passed as string
    public static void LuaDeleteItem(string item)
    {
        Lua.LuaDoString("for bag = 0, 4, 1 do for slot = 1, 32, 1 do local name = GetContainerItemLink(bag, slot); " +
            "if name and string.find(name, \"" + item + "\") then PickupContainerItem(bag, slot); " +
            "DeleteCursorItem(); end; end; end", false);
    }

}