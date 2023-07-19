using AIO.Combat.Common;
using AIO.Lists;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using wManager.Events;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Shaman
{
    internal class WeaponHelper : ICycleable
    {
        private readonly BaseCombatClass CombatClass;
        private Spec Spec => CombatClass.Specialisation;

        internal WeaponHelper(BaseCombatClass combatClass) => CombatClass = combatClass;

        public void Initialize()
        {
            FightEvents.OnFightLoop += OnFightLoop;
            MovementEvents.OnMovementPulse += OnMovementPulse;
        }

        public void Dispose()
        {
            FightEvents.OnFightLoop -= OnFightLoop;
            MovementEvents.OnMovementPulse -= OnMovementPulse;
        }

        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable) => Enchant();
        private void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable) => Enchant();

        private readonly Spell RockbiterWeapon = new Spell("Rockbiter Weapon");
        private readonly Spell FlametongueWeapon = new Spell("Flametongue Weapon");
        private readonly Spell EarthlivingWeapon = new Spell("Earthliving Weapon");
        private readonly Spell WindfuryWeapon = new Spell("Windfury Weapon");

        private bool HasMainHandEnchant => Lua.LuaDoString<bool>
            (@"local hasMainHandEnchant, _, _, _, _, _, _, _, _ = GetWeaponEnchantInfo()
            if (hasMainHandEnchant) then 
               return '1'
            else
               return '0'
            end");

        private bool HasOffHandEnchant => Lua.LuaDoString<bool>
            (@"local _, _, _, _, hasOffHandEnchant, _, _, _, _ = GetWeaponEnchantInfo()
            if (hasOffHandEnchant) then 
               return '1'
            else
               return '0'
            end");

        private bool HasOffHandWeapon => Lua.LuaDoString<bool>(@"local hasWeapon = OffhandHasWeapon()
            return hasWeapon");

        private void Enchant()
        {
            switch (Spec)
            {
                case Spec.Shaman_SoloEnhancement:
                case Spec.Shaman_GroupEnhancement:
                    if (!HasMainHandEnchant)
                    {
                        if (WindfuryWeapon.KnownSpell)
                        {
                            WindfuryWeapon.Launch();
                        }
                        else
                        {
                            RockbiterWeapon.Launch();
                        }
                    }
                    if (HasOffHandWeapon && !HasOffHandEnchant)
                    {
                        if (FlametongueWeapon.KnownSpell)
                        {
                            FlametongueWeapon.Launch();
                        }
                        else
                        {
                            RockbiterWeapon.Launch();
                        }
                    }
                    break;
                case Spec.Shaman_GroupRestoration:
                    if (!HasMainHandEnchant)
                    {
                        if (EarthlivingWeapon.KnownSpell)
                        {
                            EarthlivingWeapon.Launch();
                        }
                        else
                        {
                            FlametongueWeapon.Launch();
                        }
                    }
                    break;
                case Spec.Shaman_SoloElemental:
                    if (!HasMainHandEnchant)
                    {
                        FlametongueWeapon.Launch();
                    }
                    break;
                case Spec.LowLevel:
                    if (!HasMainHandEnchant)
                    {
                        RockbiterWeapon.Launch();
                    }
                    if (HasOffHandWeapon && !HasOffHandEnchant)
                    {
                        RockbiterWeapon.Launch();
                    }
                    break;
            }
        }
    }
}

