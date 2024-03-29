﻿using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers.Caching;
using AIO.Lists;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class GroupProtection : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private List<WoWUnit> _cleanseableUnits = new List<WoWUnit>();

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationAction("Pre-Calculations", DoPreCalculations), 0.0f, 200),
            new RotationStep(new RotationAction("Cache debuffed party members", RotationCombatUtil.CacheLUADebuffedPartyMembersStep), 0f, 1000),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Lay on Hands"), 1.1f, (s,t) => t.CHealthPercent() <= Settings.Current.GroupProtectionLoH && !Me.CHaveBuff("Forbearance"), RotationCombatUtil.FindMe, checkRange:false),
            new RotationStep(new RotationSpell("Divine Shield"), 1.2f, (s,t) => Me.HealthPercent < Settings.Current.GroupProtectionLoH && !Me.CHaveBuff("Forbearance"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Holy Light"), 1.3f, (s,t) => Me.HealthPercent < 35, RotationCombatUtil.FindMe),
            //TODO: add Holy Light combat usage to settings
            new RotationStep(new RotationSpell("Hand of Freedom"), 1.5f, (s,t) => !Me.Silenced && (Me.HaveImportantSlow() || Me.HaveImportantRoot()) && EnemiesAttackingGroup.ContainsAtLeast(enem => enem.CGetDistance() >= 5 && enem.IsTargetingMe, 1), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Divine Sacrifice"), 1.6f, UseDivineSacrifice, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Sacred Shield"), 1.8f, RotationCombatUtil.Always, _ => !Me.HaveBuff("Sacred Shield"), RotationCombatUtil.FindMe, checkRange:false),
            new RotationStep(new RotationSpell("Consecration"), 2f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.Count(unit => unit.CGetDistance() <=8) >= Settings.Current.GroupProtConsecration, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Divine Plea"), 2.5f, (s, t) => Me.CManaPercentage() < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaIC, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hand of Reckoning"), 3f, (s,t) => Me.InCombatFlagOnly && !t.CIsTargetingMe() && Settings.Current.GroupProtectionHoR, FindEnemyAttackingGroup, checkLoS:true),
            //maybe needs some better Targeting
            new RotationStep(new RotationSpell("Righteous Defense"), 4f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.Any(u => !u.CIsTargetingMe() && u.CIsTargetingMeOrMyPetOrPartyMember()),RotationCombatUtil.CFindPartyMemberWithoutMe,checkLoS:true),

            new RotationStep(new RotationSpell("Cleanse"), 4.2f, (s,t) => Settings.Current.GroupProtectionCleanse == "Group", p => RotationCombatUtil.GetPartyMemberWithCachedDebuff(p, new List<DebuffType>() { DebuffType.Magic, DebuffType.Poison, DebuffType.Disease }, true, 30)),
            new RotationStep(new RotationSpell("Cleanse"), 4.5f, (s,t) => Settings.Current.GroupProtectionCleanse == "Me" && RotationCombatUtil.IHaveCachedDebuff(new List<DebuffType>() { DebuffType.Magic, DebuffType.Poison, DebuffType.Disease }), RotationCombatUtil.FindMe),
            
            new RotationStep(new RotationSpell("Holy Shield"), 4.6f, (s,t) => Me.InCombat, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Judgement of Wisdom"), 4.8f,(s,t) => !t.CHaveBuff("Judgement of Wisdom") && t.HealthPercent > 35, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Divine Plea"), 5f, (s, t) => Me.CManaPercentage() < Settings.Current.GeneralDivinePlea, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Divine Protection"), 7f, (s,t) => Settings.Current.DivineProtection && (EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.CIsTargetingMe() && u.IsElite, 3) && Me.HealthPercent < 85 || BossList.MyTargetIsBoss && Me.HealthPercent < 85), RotationCombatUtil.FindMe, checkRange:false),
            new RotationStep(new RotationSpell("Hammer of Justice"), 8f, (s,t) => t.Fleeing || Me.HealthPercent < 50 || (t.CCanInterruptCasting() && Settings.Current.GroupProtectionHammerofJustice) || (BossList.MyTargetIsBoss && !t.IsStunned && !t.HaveBuff("Death Coil")), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Hand of Salvation"), 9.1f, (s,t) => EnemiesAttackingGroup.Any(unit => unit.CIsTargetingMeOrMyPetOrPartyMember()), RotationCombatUtil.CFindPartyMemberWithoutMe),
            new RotationStep(new RotationSpell("Hand of Protection"), 9.2f, (s,t) =>
                Settings.Current.GroupProtectionHoP
                && EnemiesAttackingGroup.Any(unit => unit.CIsTargetingMeOrMyPetOrPartyMember())
                && t.HealthPercent < 50
                && (t.WowClass == WoWClass.Mage || t.WowClass == WoWClass.Warlock || t.WowClass == WoWClass.Priest || t.WowClass == WoWClass.Druid), RotationCombatUtil.CFindPartyMember, checkLoS:true),
            new RotationStep(new RotationSpell("Avenger's Shield"), 10f, (s,t) => Me.CManaPercentage() > 20 && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() > 10 && u.CGetDistance() < 30 && u.IsTargetingPartyMember, 3), RotationCombatUtil.BotTargetFast, checkLoS:true),
            new RotationStep(new RotationSpell("Avenging Wrath"), 11f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.CIsTargetingMe(), 2) && Settings.Current.GroupAvengingWrathProtection,RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Judgement of Light"), 12f, (s,t) => !SpellManager.KnowSpell("Judgement of Wisdom"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Holy Wrath"), 12.5f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.IsElite && (u.IsCreatureType("Undead") || u.IsCreatureType("Demon")) && !u.IsStunned && u.CGetDistance() < 10, 2), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Exorcism"), 13f, (s,t) => (t.IsCreatureType("Undead") || t.IsCreatureType("Demon")) && Me.ManaPercentage > 25 && t.IsElite, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Hammer of Wrath"), 14f, (s,t) => t.CHealthPercent() < 20 && Me.CManaPercentage() > 50 , FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell("Hammer of the Righteous"), 16f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shield of Righteousness"), 17f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Holy Shield"), 18f, (s,t) => t.IsTargetingMe, RotationCombatUtil.FindMe, checkRange:false)
        };

        private bool DoPreCalculations()
        {
            Cache.Reset();
            EnemiesAttackingGroup = RotationFramework.Enemies.Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember())
                .ToArray();
            _cleanseableUnits.Clear();
            return false;
        }

        public WoWUnit FindEnemyAttackingGroup(Func<WoWUnit, bool> predicate) => EnemiesAttackingGroup.FirstOrDefault(predicate);

        private bool UseDivineSacrifice(IRotationAction s, WoWUnit t)
        {
            var nearbyFriendlies = RotationFramework.PartyMembers.Where(o => o.IsAlive && o.GetDistance <= 40).ToList();

            var under60 = nearbyFriendlies.Count(o => o.HealthPercent <= 60);
            var under75 = nearbyFriendlies.Count(o => o.HealthPercent <= 75);
            var under90 = nearbyFriendlies.Count(o => o.HealthPercent <= 65);

            return Me.IsInGroup && RotationFramework.PartyMembers.Count(u => u.IsAlive) >= 1 && (under60 >= 2 || under75 >= 3 || under90 >= 4);
        }
    }
}
