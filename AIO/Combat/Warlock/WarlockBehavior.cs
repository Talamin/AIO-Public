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
            },
            new Buffs(),
            new PetAutoTarget("Torment"))
        { }

        protected override void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
        {
            RefreshPet();

            Lua.LuaDoString("PetDefensiveMode();");
        }
        protected override void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (Me.HealthPercent < 20 && Me.IsAlive)
            {
                Consumables.UseHealthstone();
            }
            if (!Fight.InFight || Settings.Current.PetInfight)
            {
                RefreshPet();
            }
            if (RotationFramework.Enemies.Count(o => o.Position.DistanceTo(Pet.Position) <= 8) > 1 && PetManager.CurrentWarlockPet == "Voidwalker")
            {
                PetManager.PetSpellCast("Suffering");
            }
            var Devour = RotationFramework.PartyMembers.Where(u => u.IsAlive && u.HaveImportantMagic()).FirstOrDefault();
            if (PetManager.CurrentWarlockPet == "Felhunter" && Devour != null)
            {
                Me.FocusGuid = Devour.Guid;
                if (Pet.Position.DistanceTo(Target.Position) <= 30)
                {
                    PetManager.PetSpellCastFocus("Devour Magic");
                    Thread.Sleep(50);
                    Lua.LuaDoString("ClearFocus();");
                }
            }
            var Interrupt = RotationFramework.Enemies.Where(u => u.IsCast && u.IsTargetingMeOrMyPetOrPartyMember).FirstOrDefault();
            if (PetManager.CurrentWarlockPet == "Felhunter" && Interrupt != null)
            {
                if (Pet.Target != Interrupt.Guid)
                {
                    Logging.Write("Found Target to Interrupt" + Interrupt);
                    Me.FocusGuid = Interrupt.Guid;
                    Logging.Write("Attacking Target with Pet to Interrupt");
                    Lua.RunMacroText("/petattack [@focus]");
                    Lua.LuaDoString("ClearFocus();");
                }
                if (Pet.Target == Interrupt.Guid)
                {
                    if (Pet.Position.DistanceTo(Interrupt.Position) <= 30)
                        PetManager.PetSpellCast("Spell Lock");
                }
            }
        }
        protected override void OnFightEnd(ulong guid)
        {
            if (Pet.IsAlive && Pet.HealthPercent < 80 && !Pet.HaveBuff("Consume Shadows") && PetManager.CurrentWarlockPet == "Voidwalker")
            {
                PetManager.PetSpellCast("Consume Shadows");
            }
            if (!Me.IsInGroup && !Pet.InCombat)
            {
                Lua.LuaDoString("PetDefensiveMode();");
            }
            if (Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.IsTargetingPartyMember) <= 0 && !Pet.InCombat)
            {
                Lua.LuaDoString("PetDefensiveMode();");
            }
            if (ItemsHelper.CountItemStacks("Soul Shard") >= 5)
            {
                ItemsHelper.DeleteItems(6265, 5);
            }
        }

        private readonly Spell CreateHealthStone = new Spell("Create Healthstone");
        private readonly Spell SummonImp = new Spell("Summon Imp");
        private readonly Spell SummonVoidWalker = new Spell("Summon Voidwalker");
        private readonly Spell SummonFelguard = new Spell("Summon Felguard");
        private readonly Spell SummonFelhunter = new Spell("Summon Felhunter");
        private readonly Spell LifeTapOOC = new Spell("Life Tap");

        //protected override void OnMovementCalculation(Vector3 from, Vector3 to, string continentnamempq, CancelEventArgs cancelable)
        //{
        //    RefreshPet();
        //    SpellstoneHelper.Refresh();
        //    HealthstoneRefresh();
        //}

        //protected override void OnObjectManagerPulse()
        //{
        //    RefreshPet();
        //}

        protected override void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
        {
            RefreshPet();
            SpellstoneHelper.Refresh();
            HealthstoneRefresh();
            LifeTapOutOfCombat();
        }

        private void HealthstoneRefresh()
        {
            if (!Consumables.HaveHealthstone())
            {
                if (CreateHealthStone.KnownSpell && CreateHealthStone.IsSpellUsable)
                {
                    CreateHealthStone.Launch();
                    Usefuls.WaitIsCasting();
                }
            }
            if (ItemsHelper.CountItemStacks("Soul Shard") >= 5)
            {
                ItemsHelper.DeleteItems(6265, 5);
            }

            if (!Fight.InFight || Settings.Current.PetInfight)
            {
                RefreshPet();
            }
        }

        private void RefreshPet()
        {
            if ((!Pet.IsAlive || (Pet.IsAlive && PetManager.CurrentWarlockPet != Settings.Current.Pet)) &&
                !Me.IsMounted)
            {
                if (Settings.Current.Pet == "Felhunter" && SummonFelhunter.KnownSpell && SummonFelhunter.IsSpellUsable)
                {
                    SummonFelhunter.Launch();
                    Usefuls.WaitIsCasting();
                    return;
                }
                if (Settings.Current.Pet == "Voidwalker" && SummonVoidWalker.KnownSpell && SummonVoidWalker.IsSpellUsable)
                {
                    SummonVoidWalker.Launch();
                    Usefuls.WaitIsCasting();
                    return;
                }
                if (Settings.Current.Pet == "Felguard" && SummonFelguard.KnownSpell && SummonFelguard.IsSpellUsable)
                {
                    SummonFelguard.Launch();
                    Usefuls.WaitIsCasting();
                    return;
                }
                if (PetManager.CurrentWarlockPet != "Imp" && SummonImp.KnownSpell && SummonImp.IsSpellUsable)
                {
                    SummonImp.Launch();
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

       private void LifeTapOutOfCombat()
        {
            while (!Fight.InFight 
                && LifeTapOOC.KnownSpell 
                && Me.ManaPercentage < 93 
                && Settings.Current.LifeTapOOC 
                && Me.IsInParty 
                && !Me.IsMounted 
                && !Me.InCombat 
                && !Me.HaveBuff("Drink") 
                && !Me.HaveBuff("Food") && Me.IsAlive)
            {
                LifeTapOOC.Launch();
            }         
        }
    }
}

