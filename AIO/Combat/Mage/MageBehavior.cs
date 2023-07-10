using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class MageBehavior : BaseCombatClass
    {
        public override float Range => 29.0f;

        internal MageBehavior() : base(
            Settings.Current,
            new Dictionary<string, BaseRotation>
            {
                {"LowLevel", new LowLevel() },
                {"SoloFrost", new SoloFrost() },
                {"GroupFrost", new GroupFrost() },
                {"SoloArcane", new SoloArcane() },
                {"SoloFire", new SoloFire() },
                {"GroupFire", new GroupFire() },
                {"Default", new SoloFrost() },
            },
            new Buffs(),
            new ConditionalCycleable(() => Settings.Current.Backpaddle,
                new AutoBackpedal(
                    () => Target.GetDistance <= Settings.Current.BackpaddleRange && Target.HaveBuff("Frost Nova"),
                    Settings.Current.BackpaddleRange)))
        { }

        private readonly Spell WaterElemental = new Spell("Summon Water Elemental");

        protected override void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (!Pet.IsAlive)
            {
                if (WaterElemental.IsSpellUsable && WaterElemental.KnownSpell && !Me.IsMounted
                    && Settings.Current.GlyphOfEternalWater)
                {
                    WaterElemental.Launch();
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
            }

            if (Me.IsAlive && Me.ManaPercentage <= Settings.Current.GroupFireManastone)
            {
                MageFoodManager.UseManaStone();
            }
        }

        protected override void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
        {
            MageFoodManager.CheckIfEnoughFoodAndDrinks();
            MageFoodManager.CheckIfHaveManaStone();
        }
    }
}