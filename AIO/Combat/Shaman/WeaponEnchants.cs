using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using wManager.Events;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Shaman
{
    internal class WeaponEnchants : IAddon
    {
        private readonly BaseCombatClass CombatClass;
        private Spec Spec => CombatClass.Specialisation;

        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;
        public List<RotationStep> Rotation => new List<RotationStep>();

        private readonly Spell _rockbiterWeaponSpell = new Spell("Rockbiter Weapon");
        private readonly Spell _flametongueWeaponSpell = new Spell("Flametongue Weapon");
        private readonly Spell _earthlivingWeaponSpell = new Spell("Earthliving Weapon");
        private readonly Spell _windfuryWeaponSpell = new Spell("Windfury Weapon");
        private readonly Timer _weaponCHeckTimer = new Timer(3000);

        internal WeaponEnchants(BaseCombatClass combatClass)
        {
            CombatClass = combatClass;
        }

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

        private void ApplyEnchant(Spell enchant)
        {
            enchant.Launch();
            Task.Run(async delegate
            {
                await Task.Delay(100);
                Lua.LuaDoString("StaticPopup1Button1:Click()");
            });
            
        }

        private void Enchant()
        {
            if (!_weaponCHeckTimer.IsReady) return;
            _weaponCHeckTimer.Reset();

            bool[] result = Lua.LuaDoString<bool[]>($@"
                local result = {{}};

                local hasWeapon = OffhandHasWeapon();                    
                local hasMainHandEnchant, _, _, _, hasOffHandEnchant, _, _, _, _ = GetWeaponEnchantInfo();
                table.insert(result, hasWeapon ~= nil);
                table.insert(result, hasMainHandEnchant ~= nil);
                table.insert(result, hasOffHandEnchant ~= nil);
                return unpack(result);
            ");

            if (result.Length < 3) return;

            bool hasOffHandWeapon = result[0];
            bool hasMainHandEnchant = result[1];
            bool hasOffHandEnchant = result[2];

            switch (Spec)
            {
                case Spec.Shaman_SoloEnhancement:
                case Spec.Shaman_GroupEnhancement:
                    if (!hasMainHandEnchant)
                    {
                        if (_windfuryWeaponSpell.KnownSpell)
                        {
                            ApplyEnchant(_windfuryWeaponSpell);

                        }
                        else
                        {
                            ApplyEnchant(_rockbiterWeaponSpell);
                        }
                    }
                    if (hasOffHandWeapon && !hasOffHandEnchant)
                    {
                        if (_flametongueWeaponSpell.KnownSpell)
                        {
                            ApplyEnchant(_flametongueWeaponSpell);
                        }
                        else
                        {
                            ApplyEnchant(_rockbiterWeaponSpell);
                        }
                    }
                    break;
                case Spec.Shaman_GroupRestoration:
                    if (!hasMainHandEnchant)
                    {
                        if (_earthlivingWeaponSpell.KnownSpell)
                        {
                            ApplyEnchant(_earthlivingWeaponSpell);
                        }
                        else
                        {
                            ApplyEnchant(_flametongueWeaponSpell);
                        }
                    }
                    break;
                case Spec.Shaman_SoloElemental:
                    if (!hasMainHandEnchant)
                    {
                        ApplyEnchant(_flametongueWeaponSpell);
                    }
                    break;
                case Spec.LowLevel:
                    if (!hasMainHandEnchant)
                    {
                        ApplyEnchant(_rockbiterWeaponSpell);
                    }
                    if (hasOffHandWeapon && !hasOffHandEnchant)
                    {
                        ApplyEnchant(_rockbiterWeaponSpell);
                    }
                    break;
            }

        }
    }
}

