using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using Timer = robotManager.Helpful.Timer;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class HunterBehavior : BaseCombatClass
    {
        public override float Range => Settings.Current.CombatRange;

        private readonly Timer PetFeedTimer = new Timer();

        internal HunterBehavior() : base(
            Settings.Current,
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"SoloBeastMastery", new SoloBeastMastery() },
                {"GroupBeastMastery", new GroupBeastMastery() },
                {"SoloMarksmanship", new SoloMarksmanship() },
                {"SoloSurvival", new SoloSurvival() },
                {"Default", new SoloBeastMastery() },
            },
            new Buffs(),
            new PetAutoTarget("Growl"),
            new ConditionalCycleable(() => Settings.Current.Backpaddle,
                new AutoBackpedal(
                    () => Target.GetDistance <= Settings.Current.BackpaddleRange && (Target.IsTargetingMyPet || Target.IsTargetingPartyMember),
                    Settings.Current.BackpaddleRange)))
        { }

        protected override void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (Settings.Current.UseMacro)
            {
                Lua.RunMacroText("/petdefensive");
            }
            if (!Settings.Current.UseMacro)
            {
                Lua.LuaDoString("PetDefensiveMode();");
            }
        }

        protected override void OnFightEnd(ulong guid)
        {
            if (!Me.IsInGroup && !Pet.InCombat)
            {
                if (Settings.Current.UseMacro)
                {
                    Lua.RunMacroText("/petdefensive");
                }
                if (!Settings.Current.UseMacro)
                {
                    Lua.LuaDoString("PetDefensiveMode();");
                }
            }
            if (Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMe || o.IsTargetingPartyMember) <= 0 && !Pet.InCombat)
            {
                if (Settings.Current.UseMacro)
                {
                    Lua.RunMacroText("/petdefensive");
                }
                if (!Settings.Current.UseMacro)
                {
                    Lua.LuaDoString("PetDefensiveMode();");
                }
            }
            if (Settings.Current.Petfeed)
            {
                FeedPet();
            }
        }

        protected override void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
        {
            if(ObjectManager.Pet.HealthPercent <= 40 && PetManager.GetPetSpellReady("Cower")
                && PetManager.GetPetSpellCooldown("Cower") <= 0 && ObjectManager.Pet.InCombat)
            {
                Logging.WriteDebug("Petspell: Cower");
                PetManager.PetSpellCast("Cower");
            }

            //if(PetManager.GetPetSpellReady("Gore")
            //    && !PetManager.GetPetSpellReady("Charge")
            //    && PetManager.GetPetSpellCooldown("Gore") <= 0 
            //    && ObjectManager.Pet.Focus >= 40 
            //    && ObjectManager.Pet.Position.DistanceTo(ObjectManager.Target.Position) <= 7)
            //{
            //    Logging.WriteDebug("Petspell: Gore");
            //    PetManager.PetSpellCast("Gore");
            //}

            //if (PetManager.GetPetSpellReady("Thunderstomp")
            //    && PetManager.GetPetSpellCooldown("Thunderstomp") <= 0
            //    && ObjectManager.Pet.Focus >= 40
            //    && ObjectManager.GetObjectWoWUnit().Where(u=> u.IsTargetingMyPet 
            //    && ObjectManager.Pet.Position.DistanceTo(u.Position) <= 8).Count() >= 2 )
            //{
            //    //Logging.WriteDebug("Petspell: Thunderstomp");
            //    PetManager.PetSpellCast("Thunderstomp");
            //}

            if (PetManager.GetPetSpellReady("Bite") 
                && PetManager.GetPetSpellCooldown("Bite") <= 0 
                && ObjectManager.Pet.Focus >= 40 
                && ObjectManager.Pet.Position.DistanceTo(ObjectManager.Target.Position) <= 7)
            {
                //Logging.WriteDebug("Petspell: Bite");
                PetManager.PetSpellCast("Bite");
            }

            RefreshPet();
        }


        protected override void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
        {
            RefreshPet();

            if (Settings.Current.Petfeed)
            {
                FeedPet();
            }
        }


        private readonly Spell RevivePet = new Spell("Revive Pet");
        private readonly Spell CallPet = new Spell("Call Pet");

        private void RefreshPet()
        {
            if (RevivePet.IsSpellUsable
                && RevivePet.KnownSpell
                && Pet.IsDead
                && !Me.IsMounted)
            {
                RevivePet.Launch();
                Usefuls.WaitIsCasting();
            }

            if (CallPet.IsSpellUsable
                && CallPet.KnownSpell
                && !Pet.IsValid
                && !Me.IsMounted)
            {
                CallPet.Launch();
                Usefuls.WaitIsCasting();
            }
        }

        private void FeedPet()
        {
            if (Pet.IsAlive &&
                PetFeedTimer.IsReady &&
                PetHelper.Happiness < 3)
            {
                PetHelper.Feed();
            }

            PetFeedTimer.Reset(1000 * 5);
        }
    }
}