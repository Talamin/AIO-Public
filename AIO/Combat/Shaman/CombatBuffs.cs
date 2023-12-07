using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Shaman
{
    using Settings = ShamanLevelSettings;

    internal class CombatBuffs : IAddon
    {
        private readonly Spell _rockbiterWeaponSpell = new Spell("Rockbiter Weapon");
        private readonly Spell _flametongueWeaponSpell = new Spell("Flametongue Weapon");
        private readonly Spell _earthlivingWeaponSpell = new Spell("Earthliving Weapon");
        private readonly Spell _windfuryWeaponSpell = new Spell("Windfury Weapon");
        private readonly ShamanBehavior _combatClass;
        private readonly Totems _totemsAddon;
        private Spec Spec => _combatClass.Specialisation;

        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;

        internal CombatBuffs(ShamanBehavior combatClass, Totems totemsAddon)
        {
            _combatClass = combatClass;
            _totemsAddon = totemsAddon;
        }

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationAction("Weapons Enchants", EnchantStep), 0f, 5000),
            new RotationStep(new RotationAction("Cache debuffed party members", RotationCombatUtil.CacheLUADebuffedPartyMembersStep), 0f, 1000),

            new RotationStep(new RotationBuff("Water Shield"), 2f, (s,t) => !Me.IsMounted && (Spec == Spec.Shaman_GroupRestoration || Spec == Spec.Shaman_SoloElemental || (Spec == Spec.Shaman_SoloEnhancement && Me.ManaPercentage <= 50)), RotationCombatUtil.FindMe, Exclusive.ShamanShield),
            new RotationStep(new RotationBuff("Lightning Shield"), 3f, (s,t) => !Me.IsMounted && (Me.ManaPercentage > 50 || !SpellManager.KnowSpell("Water Shield")) && !Me.HaveBuff("Water Shield"), RotationCombatUtil.FindMe, Exclusive.ShamanShield),

            new RotationStep(new RotationSpell("Totemic Recall"), 10f, (s,t) => !Me.IsMounted && Totems.ShouldRecall() && !Totems.HasAny("Earth Elemental Totem", "Mana Tide Totem","Stoneclaw Totem"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Call of the Elements"), 11f, (s,t) => Fight.InFight && !Me.IsMounted && Settings.Current.UseCotE && !MovementManager.InMovement && _totemsAddon.MissingDefaults() && !Totems.HasTemporary(), RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Mana Tide Totem"), 20f, (s,t) => !Me.IsMounted && Me.ManaPercentage <= 30, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Earth Elemental Totem"), 30f, (s,t) =>
                !Me.IsInGroup &&
                !Totems.HasAny("Stoneclaw Totem") &&
                RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 20) >= 3, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Stoneclaw Totem"), 31f, (s,t) =>
                !Me.IsInGroup &&
                !Totems.HasAny("Earth Elemental Totem") &&
                RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 20) >= 2, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Magma Totem"), 40f, (s,t) =>
                Spec == Spec.Shaman_SoloEnhancement &&
                Target.GetDistance <= 15 &&
                !Totems.HasAny("Magma Totem") &&
                Me.ManaPercentage > 40, RotationCombatUtil.FindMe),

             new RotationStep(new RotationSpell("Searing Totem"), 45f, (s,t) =>
                Target.GetDistance <= 15 &&
                !Totems.HasAny("Searing Totem") &&
                !Totems.HasAny("Magma Totem") &&
                Settings.Current.RedeploySearingTotem &&
                Me.ManaPercentage > 30, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Cleansing Totem"), 50f, (s,t) =>
                Settings.Current.UseCleansingTotem
                && RotationCombatUtil.GetPartyMemberWithCachedDebuff((p) => true, new List<DebuffType>() { DebuffType.Poison, DebuffType.Disease }, false, 30) != null
                && !Totems.HasAny("Cleansing Totem"), RotationCombatUtil.FindMe),
            /*
             * There is no "Fear","Charm","Sleep" in debuffTypes
            new RotationStep(new RotationSpell("Tremor Totem"), 51f, (s,t) =>
                RotationFramework.PartyMembers.Count(o =>
                o.HasDebuffType("Fear","Charm","Sleep")) > 0, RotationCombatUtil.FindMe),
            */
            new RotationStep(new RotationSpell("Grounding Totem"), 52f, (s,t) =>
                Settings.Current.UseGroundingTotem &&
                RotationFramework.Enemies.Count(o => o.GetDistance < 30 && o.IsCast) > 0, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Earthbind Totem"), 53f, (s,t) =>
                Settings.Current.UseEarthbindTotem &&
                RotationFramework.Enemies.Count(o => o.GetDistance < 10 && o.CreatureTypeTarget=="Humanoid") > 0, RotationCombatUtil.FindMe)
        };

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

        private bool EnchantStep()
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