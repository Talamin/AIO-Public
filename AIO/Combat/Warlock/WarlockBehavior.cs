using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using System.ComponentModel;
using wManager.Events;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    using Settings = WarlockLevelSettings;
    internal class WarlockBehavior : BaseCombatClass
    {
        private float _range = 29f;
        public override float Range => _range;

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
            Addons.Add(new PetHandler());
            Addons.Add(new PetAutoTarget("Torment"));
            if (Specialisation == Spec.Warlock_GroupAffliction)
                _range = 25f;
        }

        public override void Initialize()
        {
            FightEvents.OnFightLoop += OnFightLoop;
            base.Initialize();
        }

        public override void Dispose()
        {
            FightEvents.OnFightLoop -= OnFightLoop;
            base.Dispose();
        }

        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (Me.HealthPercent < 20
                && Me.IsAlive)
                Extension.UseFirstMatchingItem(HealthStones);
        }
    }
}

