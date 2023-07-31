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

namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class MageBehavior : BaseCombatClass
    {
        public override float Range => 29.0f;
        private readonly Spell _waterElementalSpell = new Spell("Summon Water Elemental");

        internal MageBehavior() : base(
            Settings.Current,
            new Dictionary<Spec, BaseRotation>
            {
                { Spec.LowLevel, new LowLevel() },
                { Spec.Mage_SoloFrost, new SoloFrost() },
                { Spec.Mage_GroupFrost, new GroupFrost() },
                { Spec.Mage_SoloArcane, new SoloArcane() },
                { Spec.Mage_SoloFire, new SoloFire() },
                { Spec.Mage_GroupFire, new GroupFire() },
                { Spec.Fallback, new SoloFrost() },
            })
        {
            Addons.Add(new Racials());
            Addons.Add(new OOCBuffs());
            Addons.Add(new CombatBuffs());
            if (Settings.Current.Backpaddle)
            {
                Addons.Add(new AutoBackpedal(() => Target.GetDistance <= Settings.Current.BackpaddleRange && Target.HaveBuff("Frost Nova"), Settings.Current.BackpaddleRange));
            }
        }

        public override void Initialize()
        {
            FightEvents.OnFightStart += OnFightStart;
            FightEvents.OnFightLoop += OnFightLoop;
            MovementEvents.OnMovementPulse += OnMovementPulse;
            base.Initialize();
        }

        public override void Dispose()
        {
            FightEvents.OnFightStart -= OnFightStart;
            FightEvents.OnFightLoop -= OnFightLoop;
            MovementEvents.OnMovementPulse -= OnMovementPulse;
            base.Dispose();
        }

        private void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (!Pet.IsAlive)
            {
                if (_waterElementalSpell.IsSpellUsable && _waterElementalSpell.KnownSpell && !Me.IsMounted
                    && Settings.Current.GlyphOfEternalWater)
                {
                    _waterElementalSpell.Launch();
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
            }

            if (Me.IsAlive && Me.ManaPercentage <= Settings.Current.ManaGemThreshold)
            {
                MageFoodManager.UseManaStone();
            }
        }

        private void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
        {
            MageFoodManager.CheckIfEnoughFoodAndDrinks();
            MageFoodManager.CheckIfHaveManaStone();
        }
    }
}