using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Settings;
using robotManager.Helpful;
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
    internal class PetHandler : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        private readonly Spell _summonImpSpell = new Spell("Summon Imp");
        private readonly Spell _summonVoidWalkerSpell = new Spell("Summon Voidwalker");
        private readonly Spell _summonFelguardSpell = new Spell("Summon Felguard");
        private readonly Spell _summonFelhunterSpell = new Spell("Summon Felhunter");
        private string _currentPet;
        private readonly string _desiredPet = Settings.Current.Pet;

        public List<RotationStep> Rotation => new List<RotationStep>
        {
            new RotationStep(new RotationAction("Pet Manager", ManagePet), 0f, 1000),
        };

        public void Initialize()
        {
            _currentPet = PetManager.GetCurrentWarlockPetLUA;
            RefreshPet();
        }

        public void Dispose() { }

        private bool ManagePet()
        {
            RefreshPet();

            if (Me.InCombat
                && Pet.IsAlive
                && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(Pet.Position) <= 8) > 1
                && _currentPet == "Voidwalker")
            {
                PetManager.CastPetSpellIfReady("Suffering");
            }

            if (Me.InCombat
                && Pet.IsAlive
                && _currentPet == "Felhunter")
            {
                WoWPlayer unitToDevour = RotationFramework.PartyMembers
                    .Where(u => u.IsAlive && u.HaveImportantMagic())
                    .FirstOrDefault();
                if (unitToDevour != null)
                {
                    Me.FocusGuid = unitToDevour.Guid;
                    if (Pet.Position.DistanceTo(Target.Position) <= 30)
                    {
                        PetManager.CastPetSpellIfReady("Devour Magic", true);
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
                            PetManager.CastPetSpellIfReady("Spell Lock");
                    }
                }
            }

            // Voidwalker's Consume Shadows
            if (!Me.InCombat
                && Pet.IsAlive
                && Pet.HealthPercent < 80
                && _currentPet == "Voidwalker"
                && !Pet.HaveBuff("Consume Shadows"))
            {
                PetManager.CastPetSpellIfReady("Consume Shadows");
            }

            if (Me.IsInGroup
                && !Pet.InCombat
                && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.IsTargetingPartyMember) <= 0)
            {
                Lua.LuaDoString("PetDefensiveMode();");
            }

            return false;
        }

        private bool SummonPet(Spell spell)
        {
            if (!spell.KnownSpell || !spell.IsSpellUsable) return false;
            Lua.LuaDoString("PetDismiss();");
            spell.Launch(true, false);
            PetManager.PreventPetDoubleSummon();

            Thread.Sleep(200);
            _currentPet = PetManager.GetCurrentWarlockPetLUA;

            Lua.LuaDoString("PetDefensiveMode();");

            if (_currentPet == "Imp")
            {
                Thread.Sleep(100);
                PetManager.TogglePetSpellAuto("Blood Pact", true);
                Thread.Sleep(100);
                PetManager.TogglePetSpellAuto("Firebolt", true);
            }

            if (_currentPet == "Felhunter")
            {
                Thread.Sleep(100);
                PetManager.TogglePetSpellAuto("Fel Intelligence", true);
                Thread.Sleep(100);
                PetManager.TogglePetSpellAuto("Shadow Bite", true);
            }

            return true;
        }

        private void RefreshPet()
        {
            if (Me.IsMounted) return;
            if (!Settings.Current.ReSummonPetInfight && Me.InCombat) return;

            if (!Pet.IsAlive)
                _currentPet = "None"; ;

            if (!Pet.IsAlive || _currentPet != _desiredPet)
            {
                if (_desiredPet == "Felhunter"
                && SummonPet(_summonFelhunterSpell))
                    return;

                if (_desiredPet == "Voidwalker"
                    && SummonPet(_summonVoidWalkerSpell))
                    return;

                if (_desiredPet == "Felguard"
                    && SummonPet(_summonFelguardSpell))
                    return;

                if (_currentPet != "Imp"
                    && SummonPet(_summonImpSpell))
                    return;
            }
        }
    }
}
