using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using wManager.Events;
using wManager.Wow.Class;
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
        private readonly Timer _petFeedTimer = new Timer();
        private readonly Spell _revivePetSpell = new Spell("Revive Pet");
        private readonly Spell _callPetSpell = new Spell("Call Pet");

        internal HunterBehavior() : base(
            Settings.Current,
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.LowLevel, new LowLevel() },
                { Spec.Hunter_SoloBeastMastery, new SoloBeastMastery() },
                { Spec.Hunter_GroupBeastMastery, new GroupBeastMastery() },
                { Spec.Hunter_SoloMarksmanship, new SoloMarksmanship() },
                { Spec.Hunter_SoloSurvival, new SoloSurvival() },
                { Spec.Fallback, new SoloBeastMastery() },
            })
        {
            Addons.Add(new Racials());
            Addons.Add(new OOCBuffs());
            Addons.Add(new CombatBuffs());
            Addons.Add(new PetAutoTarget("Growl"));
            if (Settings.Current.Backpaddle)
            {
                Addons.Add(new AutoBackpedal(
                    () => Target.GetDistance <= Settings.Current.BackpaddleRange && (Target.IsTargetingMyPet || Target.IsTargetingPartyMember),
                    Settings.Current.BackpaddleRange));
            }
        }

        public override void Initialize()
        {
            FightEvents.OnFightLoop += OnFightLoop;
            FightEvents.OnFightStart += OnFightStart;
            FightEvents.OnFightEnd += OnFightEnd;
            MovementEvents.OnMovementPulse += OnMovementPulse;
            base.Initialize();
        }

        public override void Dispose()
        {
            FightEvents.OnFightLoop -= OnFightLoop;
            FightEvents.OnFightStart -= OnFightStart;
            FightEvents.OnFightEnd -= OnFightEnd;
            MovementEvents.OnMovementPulse -= OnMovementPulse;
            base.Dispose();
        }

        private void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
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

        private void OnFightEnd(ulong guid)
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

        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (ObjectManager.Pet.HealthPercent <= 40 
                && PetManager.GetPetSpellReady("Cower")
                && PetManager.GetPetSpellCooldown("Cower") <= 0 
                && ObjectManager.Pet.InCombat)
            {
                PetManager.PetSpellCast("Cower");
            }

            if (PetManager.GetPetSpellReady("Bite")
                && PetManager.GetPetSpellCooldown("Bite") <= 0
                && ObjectManager.Pet.Focus >= 40
                && ObjectManager.Pet.Position.DistanceTo(ObjectManager.Target.Position) <= 7)
            {
                PetManager.PetSpellCast("Bite");
            }

            RefreshPet();
        }


        private void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
        {
            RefreshPet();
            if (Settings.Current.Petfeed)
            {
                FeedPet();
            }
        }

        private void RefreshPet()
        {
            if (_revivePetSpell.IsSpellUsable
                && _revivePetSpell.KnownSpell
                && Pet.IsDead
                && !Me.IsMounted)
            {
                _revivePetSpell.Launch();
                Usefuls.WaitIsCasting();
            }

            if (_callPetSpell.IsSpellUsable
                && _callPetSpell.KnownSpell
                && !Pet.IsValid
                && !Me.IsMounted)
            {
                _callPetSpell.Launch();
                Usefuls.WaitIsCasting();
            }
        }

        private void FeedPet()
        {
            if (Pet.IsAlive &&
                _petFeedTimer.IsReady &&
                PetHelper.Happiness < 3)
            {
                PetHelper.Feed();
            }

            _petFeedTimer.Reset(1000 * 5);
        }
    }
}