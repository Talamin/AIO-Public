/*using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using System.Collections.Generic;
using System.Threading.Tasks;
using wManager.Wow.Class;
using wManager.Wow.Helpers;

namespace AIO.Combat.Shaman
{
    internal class WeaponEnchants : IAddon
    {
        private readonly BaseCombatClass CombatClass;
        private Spec Spec => CombatClass.Specialisation;

        private readonly Spell _rockbiterWeaponSpell = new Spell("Rockbiter Weapon");
        private readonly Spell _flametongueWeaponSpell = new Spell("Flametongue Weapon");
        private readonly Spell _earthlivingWeaponSpell = new Spell("Earthliving Weapon");
        private readonly Spell _windfuryWeaponSpell = new Spell("Windfury Weapon");

        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;
        public List<RotationStep> Rotation => new List<RotationStep>()
        {
            new RotationStep(new RotationCode("Weapon Enchants", Enchant), 1f, 3000),
        };

        internal WeaponEnchants(BaseCombatClass combatClass)
        {
            CombatClass = combatClass;
        }

        public void Initialize() { }

        public void Dispose() { }

        private void ApplyEnchant(Spell enchant)
        {
            enchant.Launch();
            Task.Run(async delegate
            {
                await Task.Delay(100);
                Lua.LuaDoString("StaticPopup1Button1:Click()");
            });

        }

        private bool Enchant()
        {
            bool[] result = Lua.LuaDoString<bool[]>($@"
                local result = {{}};

                local hasWeapon = OffhandHasWeapon();                    
                local hasMainHandEnchant, _, _, _, hasOffHandEnchant, _, _, _, _ = GetWeaponEnchantInfo();
                table.insert(result, hasWeapon ~= nil);
                table.insert(result, hasMainHandEnchant ~= nil);
                table.insert(result, hasOffHandEnchant ~= nil);
                return unpack(result);
            ");

            if (result.Length < 3) return false;

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
                            return true;
                        }
                        else
                        {
                            ApplyEnchant(_rockbiterWeaponSpell);
                            return true;
                        }
                    }
                    if (hasOffHandWeapon && !hasOffHandEnchant)
                    {
                        if (_flametongueWeaponSpell.KnownSpell)
                        {
                            ApplyEnchant(_flametongueWeaponSpell);
                            return true;
                        }
                        else
                        {
                            ApplyEnchant(_rockbiterWeaponSpell);
                            return true;
                        }
                    }
                    break;
                case Spec.Shaman_GroupRestoration:
                    if (!hasMainHandEnchant)
                    {
                        if (_earthlivingWeaponSpell.KnownSpell)
                        {
                            ApplyEnchant(_earthlivingWeaponSpell);
                            return true;
                        }
                        else
                        {
                            ApplyEnchant(_flametongueWeaponSpell);
                            return true;
                        }
                    }
                    break;
                case Spec.Shaman_SoloElemental:
                    if (!hasMainHandEnchant)
                    {
                        ApplyEnchant(_flametongueWeaponSpell);
                        return true;
                    }
                    break;
                case Spec.LowLevel:
                    if (!hasMainHandEnchant)
                    {
                        ApplyEnchant(_rockbiterWeaponSpell);
                        return true;
                    }
                    if (hasOffHandWeapon && !hasOffHandEnchant)
                    {
                        ApplyEnchant(_rockbiterWeaponSpell);
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
}

*/