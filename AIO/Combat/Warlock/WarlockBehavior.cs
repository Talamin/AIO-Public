using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using wManager.Events;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    using Settings = WarlockLevelSettings;
    internal class WarlockBehavior : BaseCombatClass
    {
        public override float Range => 29.0f;

        private readonly Spell _summonImpSpell = new Spell("Summon Imp");
        private readonly Spell _summonVoidWalkerSpell = new Spell("Summon Voidwalker");
        private readonly Spell _summonFelguardSpell = new Spell("Summon Felguard");
        private readonly Spell _summonFelhunterSpell = new Spell("Summon Felhunter");

        private readonly Spell _createSoulstoneSpell = new Spell("Create Soulstone");
        private readonly Spell _createSpellstoneSpell = new Spell("Create Spellstone");
        private readonly Spell _createHealthStoneSpell = new Spell("Create Healthstone");

        private readonly List<string> _spellstones = new List<string> {
            "Spellstone",
            "Greater Spellstone",
            "Major Spellstone",
            "Master Spellstone",
            "Demonic Spellstone",
            "Grand Spellstone"
        };
        private readonly List<string> _healthStones = new List<string>
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
        private readonly List<string> _soulStones = new List<string>
        {
            "Minor Soulstone",
            "Lesser Soulstone",
            "Soulstone",
            "Greater Soulstone",
            "Major Soulstone",
            "Master Soulstone",
            "Demonic Soulstone"
        };

        internal WarlockBehavior() : base(
            Settings.Current,
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.LowLevel, new LowLevel() },
                { Spec.Warlock_SoloAffliction, new SoloAffliction() },
                { Spec.Warlock_GroupAffliction, new GroupAffliction() },
                { Spec.Warlock_SoloDestruction, new SoloDestruction() },
                { Spec.Warlock_SoloDemonology, new SoloDemonology() },
                { Spec.Fallback, new SoloAffliction() },
            })
        {
            Addons.Add(new Racials());
            Addons.Add(new OOCBuffs());
            Addons.Add(new PetAutoTarget("Torment"));
        }

        public override void Initialize()
        {
            FightEvents.OnFightStart += OnFightStart;
            FightEvents.OnFightLoop += OnFightLoop;
            FightEvents.OnFightEnd += OnFightEnd;
            MovementEvents.OnMovementPulse += OnMovementPulse;
            base.Initialize();
        }

        public override void Dispose()
        {
            FightEvents.OnFightStart -= OnFightStart;
            FightEvents.OnFightLoop -= OnFightLoop;
            FightEvents.OnFightEnd -= OnFightEnd;
            MovementEvents.OnMovementPulse -= OnMovementPulse;
            base.Dispose();
        }

        private void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
        {
            RefreshPet();
            Lua.LuaDoString("PetDefensiveMode();");
        }

        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (Me.HealthPercent < 20
                && Me.IsAlive)
            {
                Extension.UseFirstMatchingItem(_healthStones);
            }

            if (Settings.Current.ReSummonPetInfight)
            {
                RefreshPet();
            }

            if (RotationFramework.Enemies.Count(o => o.Position.DistanceTo(Pet.Position) <= 8) > 1
                && PetManager.CurrentWarlockPet == "Voidwalker")
            {
                PetManager.PetSpellCast("Suffering");
            }

            if (PetManager.CurrentWarlockPet == "Felhunter")
            {
                WoWPlayer unitToDevour = RotationFramework.PartyMembers
                    .Where(u => u.IsAlive && u.HaveImportantMagic())
                    .FirstOrDefault();
                if (unitToDevour != null)
                {
                    Me.FocusGuid = unitToDevour.Guid;
                    if (Pet.Position.DistanceTo(Target.Position) <= 30)
                    {
                        PetManager.PetSpellCastFocus("Devour Magic");
                        Thread.Sleep(50);
                        Lua.LuaDoString("ClearFocus();");
                    }
                }

                WoWUnit unitToInterrupt = RotationFramework.Enemies
                    .Where(u => u.IsCast && u.IsTargetingMeOrMyPetOrPartyMember)
                    .FirstOrDefault();
                if (unitToInterrupt != null)
                {
                    if (Pet.Target != unitToInterrupt.Guid)
                    {
                        Logging.Write("Found Target to Interrupt" + unitToInterrupt);
                        Me.FocusGuid = unitToInterrupt.Guid;
                        Logging.Write("Attacking Target with Pet to Interrupt");
                        Lua.RunMacroText("/petattack [@focus]");
                        Lua.LuaDoString("ClearFocus();");
                    }
                    if (Pet.Target == unitToInterrupt.Guid)
                    {
                        if (Pet.Position.DistanceTo(unitToInterrupt.Position) <= 30)
                            PetManager.PetSpellCast("Spell Lock");
                    }
                }
            }
        }
        private void OnFightEnd(ulong guid)
        {
            // Voidwalker's Consume Shadows
            if (Pet.IsAlive
                && Pet.HealthPercent < 80
                && PetManager.CurrentWarlockPet == "Voidwalker"
                && !Pet.HaveBuff("Consume Shadows"))
            {
                PetManager.PetSpellCast("Consume Shadows");
            }

            if (!Me.IsInGroup
                && !Pet.InCombat)
            {
                Lua.LuaDoString("PetDefensiveMode();");
            }

            if (Me.IsInGroup
                && !Pet.InCombat
                && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.IsTargetingPartyMember) <= 0)
            {
                Lua.LuaDoString("PetDefensiveMode();");
            }
        }

        private void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
        {
            string lua = $@"
                local result = {{}};
                local allSpellStones = {{{ConcatenateForLUA(_spellstones)}}};
                local allHealthStones = {{{ConcatenateForLUA(_healthStones)}}};
                local allSoulStones = {{{ConcatenateForLUA(_soulStones)}}};
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

            RefreshPet();
        }

        private void FindAndUseSpellStone()
        {
            WoWItem spellStone = Bag.GetBagItem().FirstOrDefault(item => _spellstones.Contains(item.Name));
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
                && u.GetDistance < 20
                && !u.HaveBuff("Soulstone Resurrection")
                && !TraceLine.TraceLineGo(u.PositionWithoutType));

            if (unit == null) return;

            WoWItem soulStone = Bag.GetBagItem()
                .FirstOrDefault(item => _soulStones.Contains(item.Name));
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
                    Thread.Sleep(100);
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

        private void RefreshPet()
        {
            if ((!Pet.IsAlive || (Pet.IsAlive && PetManager.CurrentWarlockPet != Settings.Current.Pet))
                && !Me.IsMounted)
            {
                if (Settings.Current.Pet == "Felhunter"
                    && _summonFelhunterSpell.KnownSpell
                    && _summonFelhunterSpell.IsSpellUsable)
                {
                    _summonFelhunterSpell.Launch();
                    Usefuls.WaitIsCasting();
                    return;
                }

                if (Settings.Current.Pet == "Voidwalker"
                    && _summonVoidWalkerSpell.KnownSpell
                    && _summonVoidWalkerSpell.IsSpellUsable)
                {
                    _summonVoidWalkerSpell.Launch();
                    Usefuls.WaitIsCasting();
                    return;
                }

                if (Settings.Current.Pet == "Felguard"
                    && _summonFelguardSpell.KnownSpell
                    && _summonFelguardSpell.IsSpellUsable)
                {
                    _summonFelguardSpell.Launch();
                    Usefuls.WaitIsCasting();
                    return;
                }

                if (PetManager.CurrentWarlockPet != "Imp"
                    && _summonImpSpell.KnownSpell
                    && _summonImpSpell.IsSpellUsable)
                {
                    _summonImpSpell.Launch();
                    Usefuls.WaitIsCasting();
                }
            }

            if (Pet.IsAlive && PetManager.CurrentWarlockPet == "Imp")
            {
                PetManager.TogglePetSpellAuto("Blood Pact", true);
                Thread.Sleep(50);
                PetManager.TogglePetSpellAuto("Firebolt", true);
            }

            if (Pet.IsAlive && PetManager.CurrentWarlockPet == "Felhunter")
            {
                PetManager.TogglePetSpellAuto("Fel Intelligence", true);
                Thread.Sleep(50);
                PetManager.TogglePetSpellAuto("Shadow Bite", true);
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

