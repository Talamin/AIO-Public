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

        public static readonly List<string> Spellstones = new List<string>
        {
            "Spellstone",
            "Greater Spellstone",
            "Major Spellstone",
            "Master Spellstone",
            "Demonic Spellstone",
            "Grand Spellstone"
        };

        public static readonly List<string> HealthStones = new List<string>
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

        public static readonly List<string> SoulStones = new List<string>
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
                Extension.UseFirstMatchingItem(HealthStones);
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
            RefreshPet();
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
    }
}

