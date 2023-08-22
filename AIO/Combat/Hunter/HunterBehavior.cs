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
        private readonly Timer _petFeedTimer = new Timer(1000 * 5);
        private readonly Spell _revivePetSpell = new Spell("Revive Pet");
        private readonly Spell _callPetSpell = new Spell("Call Pet");
        private readonly Timer _petCastTimer = new Timer(300);

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
            RefreshPet();
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
            if (!_petCastTimer.IsReady) return;
            _petCastTimer.Reset();

            if (ObjectManager.Pet.HealthPercent <= 40)
                PetManager.CastPetSpellIfReady("Cower");

            if (ObjectManager.Pet.Focus >= 50
                && ObjectManager.Pet.Position.DistanceTo(ObjectManager.Pet.TargetObject.Position) <= 7)
            {
                PetManager.CastPetSpellIfReady("Bite");
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
                _revivePetSpell.Launch(true, false);
                PetManager.PreventPetDoubleSummon();
            }

            if (_callPetSpell.IsSpellUsable
                && _callPetSpell.KnownSpell
                && !Pet.IsValid
                && !Me.IsMounted)
            {
                _callPetSpell.Launch(true, false);
                PetManager.PreventPetDoubleSummon();
            }
        }

        private void FeedPet()
        {
            if (!_petFeedTimer.IsReady) return;
            _petFeedTimer.Reset();

            if (Pet.IsAlive &&
                PetHelper.Happiness < 3)
            {
                PetHelper.Feed();
            }
        }
    }
}