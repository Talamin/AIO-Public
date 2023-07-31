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

namespace AIO.Combat.Shaman
{
    using Settings = ShamanLevelSettings;
    internal class Totems : IAddon
    {
        private readonly BaseCombatClass CombatClass;
        private Spec Spec => CombatClass.Specialisation;
        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;

        internal Totems(BaseCombatClass combatClass)
        {
            CombatClass = combatClass;
        }

        public List<RotationStep> Rotation => new List<RotationStep>();
        public void Initialize() => MovementEvents.OnMovementPulse += OnMovementPulse;
        public void Dispose() => MovementEvents.OnMovementPulse -= OnMovementPulse;

        private static IEnumerable<WoWUnit> Pets => RotationFramework.AllUnits.Where(o => o.IsMyPet);
        private static IEnumerable<WoWUnit> NearbyPets => Pets.Where(o => o.GetDistance < 30);
        private static IEnumerable<WoWUnit> DistantPets => Pets.Where(o => o.GetDistance >= 30);

        public static bool HasAny(params string[] totems) => totems.Any(t => NearbyPets.Any(o => o.Name.Contains(t)));
        public static bool HasAll(params string[] totems) => totems.All(t => t == null || NearbyPets.Any(o => o.Name.Contains(t)));

        public static float DistanceToTotem(string totem)
        {
            WoWUnit totemUnit = NearbyPets.FirstOrDefault(t => t.Name.Contains(totem));
            return totemUnit != null ? totemUnit.GetDistance : 0f;
        }

        public static bool ShouldRecall() => DistantPets.Any();

        public static bool HasTemporary() => HasAny("Mana Tide Totem", "Earth Elemental Totem", "Tremor Totem", "Grounding Totem", "Earthbind Totem", "Stoneclaw Totem");

        private void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable) => SetCall();

        private static readonly Spell StoneskinTotem = new Spell("Stoneskin Totem");
        private static readonly Spell StrengthOfEarthTotem = new Spell("Strength of Earth Totem");
        private static readonly Spell TremorTotem = new Spell("Tremor Totem");

        private static readonly Spell MagmaTotem = new Spell("Magma Totem");
        private static readonly Spell SearingTotem = new Spell("Searing Totem");
        private static readonly Spell FlametongueTotem = new Spell("Flametongue Totem");
        private static readonly Spell TotemOfWrath = new Spell("Totem of Wrath");
        private static readonly Spell FrostResistanceTotem = new Spell("Frost Resistance Totem");

        private static readonly Spell WrathOfAirTotem = new Spell("Wrath of Air Totem");
        private static readonly Spell WindfuryTotem = new Spell("Windfury Totem");
        private static readonly Spell NatureResistanceTotem = new Spell("Nature Resistance Totem");

        private static readonly Spell ManaSpringTotem = new Spell("Mana Spring Totem");
        private static readonly Spell HealingStreamTotem = new Spell("Healing Stream Totem");
        private static readonly Spell CleansingTotem = new Spell("Cleansing Totem");

        private readonly Dictionary<string, Spell> totems = new Dictionary<string, Spell> {
            {"Stoneskin Totem", StoneskinTotem},
            {"Strength of Earth Totem", StrengthOfEarthTotem},
            {"Tremor Totem", TremorTotem},             
            {"Magma Totem", MagmaTotem},
            {"Searing Totem", SearingTotem},
            {"Flametongue Totem", FlametongueTotem},
            {"Totem of Wrath", TotemOfWrath},
            {"Frost Resistance Totem", FrostResistanceTotem},
            {"Wrath of Air Totem", WrathOfAirTotem},
            {"Windfury Totem", WindfuryTotem},
            {"Nature Resistance Totem", NatureResistanceTotem},
            {"Mana Spring Totem", ManaSpringTotem},
            {"Healing Stream Totem", HealingStreamTotem},
            {"Cleansing Totem", CleansingTotem}
        };

        private void SetTotems(Spell earthTotem, Spell fireTotem, Spell airTotem, Spell waterTotem)
        {
            Lua.LuaDoString($"SetMultiCastSpell(133, {fireTotem?.Id ?? 0})");
            Lua.LuaDoString($"SetMultiCastSpell(134, {earthTotem?.Id ?? 0})");
            Lua.LuaDoString($"SetMultiCastSpell(135, {waterTotem?.Id ?? 0})");
            Lua.LuaDoString($"SetMultiCastSpell(136, {airTotem?.Id ?? 0})");
        }

        private (Spell Earth, Spell Fire, Spell Air, Spell Water) DefaultTotems
        {
            get
            {
                Spell earth = null;
                Spell fire = null;
                Spell air = null;
                Spell water = null;

                switch (Spec)
                {
                    case Spec.Shaman_SoloEnhancement:
                    case Spec.Shaman_GroupEnhancement:
                        {
                            if (StrengthOfEarthTotem.KnownSpell)
                            {
                                earth = StrengthOfEarthTotem;
                            }
                            else if (StoneskinTotem.KnownSpell)
                            {
                                earth = StoneskinTotem;
                            }

                            if (MagmaTotem.KnownSpell)
                            {
                                fire = MagmaTotem;
                            }
                            else if (SearingTotem.KnownSpell)
                            {
                                fire = SearingTotem;
                            }
                            else if (FlametongueTotem.KnownSpell)
                            {
                                fire = FlametongueTotem;
                            }

                            if (HealingStreamTotem.KnownSpell)
                            {
                                water = HealingStreamTotem;
                            }
                            else if (ManaSpringTotem.KnownSpell)
                            {
                                water = ManaSpringTotem;
                            }

                            if (WindfuryTotem.KnownSpell)
                            {
                                air = WindfuryTotem;
                            }
                            else if (WrathOfAirTotem.KnownSpell)
                            {
                                air = WrathOfAirTotem;
                            }
                            else if (NatureResistanceTotem.KnownSpell)
                            {
                                air = NatureResistanceTotem;
                            }


                            break;
                        }
                    case Spec.Shaman_GroupRestoration:
                    case Spec.Shaman_SoloElemental:
                        {
                            if (StoneskinTotem.KnownSpell)
                            {
                                earth = StoneskinTotem;
                            }
                            else if (StrengthOfEarthTotem.KnownSpell)
                            {
                                earth = StrengthOfEarthTotem;
                            }

                            if (TotemOfWrath.KnownSpell)
                            {
                                fire = TotemOfWrath;
                            }
                            else if (FlametongueTotem.KnownSpell)
                            {
                                fire = FlametongueTotem;
                            }

                            if (ManaSpringTotem.KnownSpell)
                            {
                                water = ManaSpringTotem;
                            }
                            else if (HealingStreamTotem.KnownSpell)
                            {
                                water = HealingStreamTotem;
                            }

                            if (WrathOfAirTotem.KnownSpell)
                            {
                                air = WrathOfAirTotem;
                            }
                            else if (WindfuryTotem.KnownSpell)
                            {
                                air = WindfuryTotem;
                            }

                            break;
                        }
                }
                if (Settings.Current.GeneralTotemsEarthTotem == "None")
                {
                    earth = null;
                }
                else if (Settings.Current.GeneralTotemsEarthTotem != "Auto")
                {
                    earth = totems[Settings.Current.GeneralTotemsEarthTotem];
                }

                if (Settings.Current.GeneralTotemsFireTotem == "None")
                {
                    fire = null;
                }
                else if (Settings.Current.GeneralTotemsFireTotem != "Auto")
                {
                    fire = totems[Settings.Current.GeneralTotemsFireTotem];
                }

                if (Settings.Current.GeneralTotemsAirTotem == "None")
                {
                    air = null;
                }
                else if (Settings.Current.GeneralTotemsAirTotem != "Auto")
                {
                    air = totems[Settings.Current.GeneralTotemsAirTotem];
                }

                if (Settings.Current.GeneralTotemsWaterTotem == "None")
                {
                    water = null;
                }
                else if (Settings.Current.GeneralTotemsWaterTotem != "Auto")
                {
                    water = totems[Settings.Current.GeneralTotemsWaterTotem];
                }
                return (earth, fire, air, water);
            }
        }

        private void SetCall()
        {
            var (Earth, Fire, Air, Water) = DefaultTotems;
            SetTotems(Earth, Fire, Air, Water);
        }

        public bool MissingDefaults()
        {
            var (Earth, Fire, Air, Water) = DefaultTotems;
            return !HasAll(Earth?.Name, Fire?.Name, Air?.Name, Water?.Name);
        }
    }
}
