using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    using Settings = WarlockLevelSettings;
    internal class OOCBuffs : IAddon
    {
        private readonly Spell _createSoulstoneSpell = new Spell("Create Soulstone");
        private readonly Spell _createSpellstoneSpell = new Spell("Create Spellstone");
        private readonly Spell _createHealthStoneSpell = new Spell("Create Healthstone");

        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        public List<RotationStep> Rotation => new List<RotationStep> 
        {
            new RotationStep(new RotationAction("Consumables Management", ConsumablesManagement), 0f, 5000),
            new RotationStep(new RotationBuff("Unending Breath"), 1f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Unending Breath"), 2f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Fel Armor"), 3f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.WarlockSkin),
            new RotationStep(new RotationBuff("Demon Armor"), 4f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.WarlockSkin),
            new RotationStep(new RotationBuff("Demon Skin"), 5f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe, Exclusive.WarlockSkin),
            new RotationStep(new RotationBuff("Soul Link"), 6f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Life Tap"), 7f, (s, t) => !Me.IsMounted && Me.ManaPercentage < Me.HealthPercent && Settings.Current.LifeTapOOC, RotationCombatUtil.FindMe),
        };

        public void Initialize() { }
        public void Dispose() { }

        private bool ConsumablesManagement()
        {
            if (Me.IsMounted) return false;

            string lua = $@"
                local result = {{}};
                local allSpellStones = {{{ConcatenateForLUA(WarlockBehavior.Spellstones)}}};
                local allHealthStones = {{{ConcatenateForLUA(WarlockBehavior.HealthStones)}}};
                local allSoulStones = {{{ConcatenateForLUA(WarlockBehavior.SoulStones)}}};
                local nbSpellStones = 0;
                local nbHealthStones = 0;
                local nbSoulStones = 0;
                for i = 1, #allSpellStones do
                  nbSpellStones = nbSpellStones + GetItemCount(allSpellStones[i])
                end
                for i = 1, #allHealthStones do
                  nbHealthStones = nbHealthStones + GetItemCount(allHealthStones[i])
                end
                for i = 1, #allSoulStones do
                  nbSoulStones = nbSoulStones + GetItemCount(allSoulStones[i])
                end
                local hasMainHandEnchant, _, _, _, _, _, _, _, _ = GetWeaponEnchantInfo();
                hasMainHandEnchant = hasMainHandEnchant ~= nil and 1 or 0;
                table.insert(result, GetItemCount('Soul Shard'));
                table.insert(result, nbSpellStones);
                table.insert(result, nbHealthStones);
                table.insert(result, nbSoulStones);
                table.insert(result, hasMainHandEnchant);
                return unpack(result);
            ";

            int[] stats = Lua.LuaDoString<int[]>(lua);
            if (stats.Length == 5)
            {
                int nbSoulShards = stats[0];
                int nbSpellStones = stats[1];
                int nbHealthStones = stats[2];
                int nbSoulStones = stats[3];
                int hasMainHandEnchant = stats[4];

                // SpellStone creation
                if (nbSpellStones <= 0
                    && _createSpellstoneSpell.KnownSpell
                    && _createSpellstoneSpell.IsSpellUsable)
                {
                    Main.Log($"Creating SpellStone");
                    _createSpellstoneSpell.Launch();
                    Usefuls.WaitIsCasting();
                    if (hasMainHandEnchant == 0)
                        FindAndUseSpellStone();
                }

                // SpellStone use
                if (hasMainHandEnchant == 0
                    && nbSpellStones > 0)
                {
                    FindAndUseSpellStone();
                }

                // Excess Soul shard deletion
                if (nbSoulShards > 5)
                {
                    Main.Log($"Deleting excess Soul Shard ({nbSoulShards}/5)");
                    ItemsHelper.DeleteItems(6265, 5);
                }

                // Health Stone creation
                if (nbHealthStones == 0
                    && _createHealthStoneSpell.KnownSpell
                    && _createHealthStoneSpell.IsSpellUsable)
                {
                    Main.Log($"Creating HealthStone");
                    _createHealthStoneSpell.Launch();
                    Usefuls.WaitIsCasting();
                }

                // Soul Stone creation
                if (nbSoulStones == 0
                    && _createSoulstoneSpell.KnownSpell
                    && _createSoulstoneSpell.IsSpellUsable)
                {
                    Main.Log($"Creating SoulStone");
                    _createSoulstoneSpell.Launch();
                    Usefuls.WaitIsCasting();
                }

                // Soul Stone use
                switch (Settings.Current.GeneralSoulstoneTarget)
                {
                    case "On self":
                        FindAndUseSoulStoneOn(ObjectManager.Me.Name);
                        break;
                    case "Tank":
                        FindAndUseSoulStoneOn(RotationFramework.TankName);
                        break;
                    case "Healer":
                        FindAndUseSoulStoneOn(RotationFramework.HealName);
                        break;
                }
            }
            return false;
        }

        private void FindAndUseSpellStone()
        {
            WoWItem spellStone = Bag.GetBagItem().FirstOrDefault(item => WarlockBehavior.Spellstones.Contains(item.Name));
            if (spellStone != null)
            {
                Main.Log($"Using Spell Stone");
                ItemsManager.UseItemByNameOrId(spellStone.Name);
                Thread.Sleep(100);
                Lua.RunMacroText("/use 16");
                Lua.LuaDoString("StaticPopup1Button1:Click()");
                Usefuls.WaitIsCasting();
                Lua.LuaDoString("ClearCursor();");
            }
        }

        private void FindAndUseSoulStoneOn(string unitName)
        {
            if (string.IsNullOrEmpty(unitName)) return;

            WoWUnit unit = RotationCombatUtil.FindFriendlyPlayer(u =>
                u.Name == unitName
                && u.GetDistance < 25
                && !u.HaveBuff("Soulstone Resurrection")
                && !TraceLine.TraceLineGo(u.PositionWithoutType));

            if (unit == null) return;

            WoWItem soulStone = Bag.GetBagItem()
                .FirstOrDefault(item => WarlockBehavior.SoulStones.Contains(item.Name));
            if (soulStone != null)
            {
                // Check item CD
                bool soulStoneItemReady = Lua.LuaDoString<bool>($@"
                    local startTime, duration, enable = GetItemCooldown({soulStone.Entry});
                    local time = duration - (GetTime() - startTime);
                    return time < 0;
                ");
                if (soulStoneItemReady)
                {
                    Main.Log($"Using {soulStone.Name} on {unit.Name}");
                    MovementManager.StopMove();
                    Lua.RunMacroText($"/target {unit.Name}");
                    Thread.Sleep(500);
                    if (Target.Name == unit.Name)
                    {
                        ItemsManager.UseItemByNameOrId(soulStone.Name);
                        Usefuls.WaitIsCasting();
                        Interact.ClearTarget();
                    }
                    Lua.LuaDoString("ClearCursor();");
                }
            }
        }

        private string ConcatenateForLUA(List<string> list)
        {
            string result = "";
            foreach (string item in list)
                result += $"'{item}',";
            return result;
        }
    }
}
