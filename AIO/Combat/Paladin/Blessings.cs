using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class Blessings : BaseRotation
    {
        private readonly BaseCombatClass CombatClass;

        private Stopwatch watch = Stopwatch.StartNew();
        private readonly int MAX_CACHE_AGE = 5000;
        private readonly Dictionary<string, string> PlayerBuff = new Dictionary<string, string>();


        private static readonly string Sanctuary = "Blessing of Sanctuary";
        private static readonly string Wisdom = "Blessing of Wisdom";
        private static readonly string Kings = "Blessing of Kings";
        private static readonly string Might = "Blessing of Might";
        private static readonly string ManaSpring = "Mana Spring Totem";
        private static readonly string ManaSpring2 = "Mana Spring";


        private readonly bool KnowSanctuary = SpellManager.KnowSpell(Sanctuary);
        private readonly bool HaveWarrior = RotationFramework.PartyMembers.Count(o => o.WowClass == WoWClass.Warrior) != 0;
        private readonly bool HaveShaman = RotationFramework.PartyMembers.Count(o => o.WowClass == WoWClass.Shaman) != 0;

        internal Blessings(BaseCombatClass combatClass) : base(runInCombat: true, runOutsideCombat: true) => CombatClass = combatClass;

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f, (action, me) => SetupBuffs(), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(Sanctuary), 1f, NeedsBuff, RotationCombatUtil.FindPartyMember, Exclusive.PaladinBlessing),
            new RotationStep(new RotationBuff(Wisdom), 2f, NeedsBuff, RotationCombatUtil.FindPartyMember, Exclusive.PaladinBlessing),
            new RotationStep(new RotationBuff(Kings), 3f, NeedsBuff, RotationCombatUtil.FindPartyMember, Exclusive.PaladinBlessing),
            new RotationStep(new RotationBuff(Might), 5f, NeedsBuff, RotationCombatUtil.FindPartyMember, Exclusive.PaladinBlessing),
        };

        private bool NeedsBuff(IRotationAction action, WoWUnit player)
        {
            if (PlayerBuff.ContainsKey(player.Name) && action is RotationBuff buff)
            {
                if (PlayerBuff[player.Name] == buff.Name)
                {
                    if (player.HaveBuff(buff.Name))
                        return false;
                    // Check we still need it
                    PlayerBuff[player.Name] = GetBuff(player);
                    if (PlayerBuff[player.Name] == buff.Name)
                        return true;
                }
            }
            return false;
        }

        private bool SetupBuffs()
        {
            if (CacheIsValid(MAX_CACHE_AGE)) return false;
            Cache.Reset();
            if (Me.IsInGroup)
            {
                if ((String.IsNullOrEmpty(RotationFramework.TankName) || String.IsNullOrEmpty(RotationFramework.HealName)))
                {
                    Logging.Write("Updating Tank and Healer names");
                    RotationFramework.UpdatePartyMembers("INSTANCE_BOOT_START", null);
                    Logging.Write($"Tank : {RotationFramework.TankName}, Healer : {RotationFramework.HealName}");
                }
                foreach (WoWPlayer player in RotationFramework.PartyMembers)
                {
                    //if (!PlayerBuff.ContainsKey(player.Name) || String.IsNullOrEmpty(PlayerBuff[player.Name]))
                    PlayerBuff[player.Name] = GetBuff(player);
                }
            }
            else
            {
                PlayerBuff[Me.Name] = GetBuff(Me);
            }
            return false;
        }

        private string GetBuff(WoWUnit player)
        {
            if (player == Me)
            {
                if (CombatClass.Specialisation == "Protection" || CombatClass.Specialisation == "GroupProtectionTank")
                    return GetTankBuff(player);
                if (CombatClass.Specialisation == "Holy" || CombatClass.Specialisation == "GroupHolyHeal")
                    return GetHealerBuff(player);
                if (CombatClass.Specialisation == "Retribution")
                    return GetMeleeBuff(player);
            }
            if (RotationFramework.TankName == player.Name)
                return GetTankBuff(player);
            if (RotationFramework.HealName == player.Name)
                return GetHealerBuff(player);
            switch (player.WowClass)
            {
                case WoWClass.Priest:
                case WoWClass.Mage:
                case WoWClass.Warlock:
                    return GetCasterBuff(player);
                case WoWClass.DeathKnight:
                case WoWClass.Warrior:
                case WoWClass.Rogue:
                case WoWClass.Hunter:
                case WoWClass.Paladin:
                    return GetMeleeBuff(player);
                case WoWClass.Druid:
                    return GetDruidBuff(player);
                case WoWClass.Shaman:
                    return GetShamanBuff(player);
                default:
                    throw new NotImplementedException("Failed to find class buff for " + player);
            }
        }

        private string GetShamanBuff(WoWUnit player)
        {
            // TODO: Identify Enh Shammies and give them melee buffs
            return GetCasterBuff(player);
        }

        private string GetDruidBuff(WoWUnit player)
        {
            //TODO: This can probably be better, but should sort itself out after a couple of loops.
            if (player.CHaveBuff("Cat Form"))
                return GetMeleeBuff(player);
            else
                return GetCasterBuff(player);
        }

        private string GetMeleeBuff(WoWUnit player)
        {
            if (!player.CHaveBuff(Kings) || player.CHaveMyBuff(Kings))
                return Kings;
            if (!HaveWarrior && (!player.CHaveBuff(Might) || player.CHaveMyBuff(Might)))
                return Might;
            if (KnowSanctuary && (!player.CHaveBuff(Sanctuary) || player.CHaveMyBuff(Sanctuary)))
                return Sanctuary;
            if ((!player.CHaveBuff(Wisdom) || player.CHaveMyBuff(Wisdom)) && !player.CHaveBuff(ManaSpring) && !player.CHaveBuff(ManaSpring2)) return Wisdom;
            else
                return "";
        }

        private string GetCasterBuff(WoWUnit player)
        {
            if (!player.CHaveBuff(Kings) || player.CHaveMyBuff(Kings)) return Kings;
            if ((!player.CHaveBuff(Wisdom) || player.CHaveMyBuff(Wisdom)) && !player.CHaveBuff(ManaSpring) && !player.CHaveBuff(ManaSpring2)) return Wisdom;
            if (KnowSanctuary && (!player.CHaveBuff(Sanctuary) || player.CHaveMyBuff(Sanctuary))) return Sanctuary;
            if (!HaveWarrior && (!player.CHaveBuff(Might) || player.CHaveMyBuff(Might))) return Might;
            else
                return "";
        }

        private string GetHealerBuff(WoWUnit player)
        {
            if ((!player.CHaveBuff(Wisdom) || player.CHaveMyBuff(Wisdom)) && !player.CHaveBuff(ManaSpring) && !player.CHaveBuff(ManaSpring2)) return Wisdom;
            if (!player.CHaveBuff(Kings) || player.CHaveMyBuff(Kings)) return Kings;
            if (KnowSanctuary && (!player.CHaveBuff(Sanctuary) || player.CHaveMyBuff(Sanctuary))) return Sanctuary;
            if (!HaveWarrior && (!player.CHaveBuff(Might) || player.CHaveMyBuff(Might))) return Might;
            else
                return "";
        }

        private string GetTankBuff(WoWUnit player)
        {
            if (KnowSanctuary && (!player.CHaveBuff(Sanctuary) || !player.CHaveBuff(ManaSpring) || !player.CHaveBuff(ManaSpring2))) return Sanctuary;
            if (!player.CHaveBuff(Kings) || player.CHaveMyBuff(Kings)) return Kings;
            if (!HaveWarrior && (!player.CHaveBuff(Might) || player.CHaveMyBuff(Might))) return Might;
            if ((!player.CHaveBuff(Wisdom) || player.CHaveMyBuff(Wisdom)) && !player.CHaveBuff(ManaSpring) && !player.CHaveBuff(ManaSpring2)) return Wisdom;
            else
                return "";
        }

        private bool CacheIsValid(int age)
        {

            if (watch.ElapsedMilliseconds < age)
            {
                return true;
            }
            else
            {
                watch.Restart();
                return false;
            }
        }
        // Pally buffs
        /* 
         * Tank :   Sanctury (if known)
         *          Kings
         *          Might (if no warrior)
         *          Improved Wisdom
         *          Wisdom
         *          
         * Healer : Improved Wisdom (if known)
         *          Wisdom
         *          Kings
         *          Sanctury (if known)
         *          Might (if no warrior)
         
         * DK, Rogue, Hunter, Warrior, Paladin(since not healer or tank)
         *          Kings
         *          Might (if no warrior)
         *          Sanctury (if known)
         *          Improved Wisdom (if known)
         *          Wisdom
         *          
         * Priest, Mage, Lock
         *          Kings 
         *          Improved Wisdom (if known)
         *          Wisdom
         *          Sanctury (if known)
         *          Might (if no warrior)                
         *      
         * Druid - Cat form or caster/Moonkin form
         *
         * Shaman - Ele (staff or shield, flametongue weapon) or Enhancement (offhand weapon, rockbiter / windfury weapon)
         *   
         */

    }
}