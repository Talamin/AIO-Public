using AIO.Combat.Common;
using AIO.Lists;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
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
            }, new CombatBuffs())
        { }
        private readonly Spell RaiseDead = new Spell("Raise Dead");

        protected override void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (!Pet.IsAlive)
            {
                if (RaiseDead.IsSpellUsable && RaiseDead.KnownSpell && !Me.IsMounted
                    && Settings.Current.RaiseDead && (ItemsManager.HasItemById(37201) || Settings.Current.GlyphRaiseDead))
                {
                    RaiseDead.Launch();
                    Usefuls.WaitIsCasting();
                }
            }
        }
        protected override void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
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

