using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Lists;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using wManager.Events;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.DeathKnight
{
    using Settings = DeathKnightLevelSettings;
    internal class DeathKnightBehavior : BaseCombatClass
    {
        public override float Range => 5.0f;
        private readonly Spell _raiseDeadSpell = new Spell("Raise Dead");

        internal DeathKnightBehavior() : base(
            Settings.Current,
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.DK_SoloBlood, new SoloBlood() },
                { Spec.DK_GroupBloodTank, new GroupBloodTank() },
                { Spec.DK_SoloUnholy, new SoloUnholy() },
                { Spec.DK_SoloFrost, new SoloFrost() },
                { Spec.DK_PVPUnholy, new PVPUnholy() },
                { Spec.Fallback, new SoloBlood() },
            })
        {
            Addons.Add(new Racials());
            Addons.Add(new CombatBuffs());
        }

        public override void Initialize()
        {
            FightEvents.OnFightStart += OnFightStart;
            FightEvents.OnFightLoop += OnFightLoop;
            base.Initialize();
        }

        public override void Dispose()
        {
            FightEvents.OnFightStart -= OnFightStart;
            FightEvents.OnFightLoop -= OnFightLoop;
            base.Dispose();
        }

        private void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (!Pet.IsAlive)
            {
                if (_raiseDeadSpell.IsSpellUsable && _raiseDeadSpell.KnownSpell && !Me.IsMounted
                    && Settings.Current.RaiseDead && (ItemsManager.HasItemById(37201) || Settings.Current.GlyphRaiseDead))
                {
                    _raiseDeadSpell.Launch();
                    Usefuls.WaitIsCasting();
                }
            }
        }
        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (Pet.IsAlive)
            {
                if (Pet.Target != Me.Target)
                {
                    Lua.RunMacroText("/petattack");
                    Logging.WriteFight($"Changing pet target to {Target.Name} [{Target.Guid}]");
                }
                if (Pet.Target == Me.Target)
                {
                    if (Target.IsCast && Pet.Position.DistanceTo(Target.Position) <= 6)
                    {
                        PetManager.PetSpellCast("Gnaw");
                    }
                    if (Pet.Position.DistanceTo(Target.Position) >= 7)
                    {
                        PetManager.PetSpellCast("Leap");
                    }
                }
            }
        }
    }
}

