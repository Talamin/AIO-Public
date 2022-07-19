using wManager.Wow.Helpers;

public class Interface
{

    /// <summary>
    /// to write to the statusframe use: Lua.LuaDoString(@"AIOFrame.text:SetText(""Buffing Molten Armor"")");
    /// </summary>
    public static void CreateStatusFrame()
    {
        Lua.LuaDoString(@"
        if not AIOFrame then
            AIOFrame = CreateFrame(""Frame"")
            AIOFrame:ClearAllPoints()
            AIOFrame:SetBackdrop(StaticPopup1:GetBackdrop())
            AIOFrame:SetHeight(70)
            AIOFrame:SetWidth(210)

            AIOFrame.text = AIOFrame:CreateFontString(nil, ""BACKGROUND"", ""GameFontNormal"")
            AIOFrame.text:SetAllPoints()
            AIOFrame.text:SetText(""AIO Ready!"")
            AIOFrame.text:SetTextColor(1, 1, 1, 6)
            AIOFrame:SetPoint(""CENTER"", 0, -240)
            AIOFrame:SetBackdropBorderColor(0, 0, 0, 0)

            AIOFrame:SetMovable(true)
            AIOFrame:EnableMouse(true)
            AIOFrame:SetScript(""OnMouseDown"",function() AIOFrame:StartMoving() end)
            AIOFrame:SetScript(""OnMouseUp"",function() AIOFrame:StopMovingOrSizing() end)

            AIOFrame.Close = CreateFrame(""BUTTON"", nil, AIOFrame, ""UIPanelCloseButton"")
            AIOFrame.Close:SetWidth(15)
            AIOFrame.Close:SetHeight(15)
            AIOFrame.Close:SetPoint(""TOPRIGHT"", AIOFrame, -8, -8)
            AIOFrame.Close:SetScript(""OnClick"", function()
                AIOFrame:Hide()
                DEFAULT_CHAT_FRAME:AddMessage(""AIOStatusFrame |cffC41F3Bclosed |cffFFFFFFWrite /AIO to enable again."") 	
            end)

            SLASH_WHATEVERYOURFRAMESARECALLED1=""/AIO""
            SlashCmdList.WHATEVERYOURFRAMESARECALLED = function()
                if AIOFrame:IsShown() then
                    AIOFrame:Hide()
                else
                    AIOFrame:Show()
                end
            end
        end");
    }
}

