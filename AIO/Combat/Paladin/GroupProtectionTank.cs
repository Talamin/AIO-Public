using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
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
    internal class GroupProtectionTank : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations", ignoresGlobal: true), 0.0f,(action,unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Lay on Hands"), 1.1f, (s,t) => t.CHealthPercent() <= Settings.Current.GroupProtectionLoH && !Me.CHaveBuff("Forbearance"), RotationCombatUtil.FindMe,checkRange:false),
            new RotationStep(new RotationSpell("Holy Light"), 1.2f, (s,t) => Me.HealthPercent < 35, RotationCombatUtil.FindMe),
            //TODO: add Holy Light combat usage to settings
            new RotationStep(new RotationSpell("Hand of Freedom"), 1.5f, (s,t) => (Me.HaveImportantSlow() || Me.HaveImportantRoot()) && EnemiesAttackingGroup.ContainsAtLeast(enem => enem.CGetDistance() <= 8 && enem.IsTargetingMe, 1), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Sacred Shield"), 1.8f, RotationCombatUtil.Always, _ => !Me.HaveBuff("Sacred Shield"), RotationCombatUtil.FindMe,checkRange:false),
            new RotationStep(new RotationSpell("Consecration"), 2f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.Count(unit => unit.CGetDistance() <=8) >= Settings.Current.GroupProtConsecration, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Divine Plea"), 2.5f, (s, t) => Me.CManaPercentage() < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaIC, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Hand of Reckoning"), 3f, (s,t) => !t.CIsTargetingMe(),_ => Settings.Current.GroupProtectionHoR, FindEnemyAttackingGroup, checkLoS:true),
            //maybe needs some better Targeting
            new RotationStep(new RotationSpell("Righteous Defense"), 4f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.Any(u => !u.CIsTargetingMe() && u.CIsTargetingMeOrMyPetOrPartyMember()),RotationCombatUtil.CFindPartyMemberWithoutMe,checkLoS:true),
            new RotationStep(new RotationSpell("Judgement of Wisdom"), 4.5f,(s,t) => !t.CHaveBuff("Judgement of Wisdom") && t.HealthPercent > 35, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Cleanse"), 4.6f, (s,t) => Settings.Current.GroupProtectionCleanse == "Group" && t.HasDebuffType("Poison","Disease","Magic"), RotationCombatUtil.CFindPartyMember,checkLoS:true),
            new RotationStep(new RotationSpell("Cleanse"), 4.7f, (s,t) => Settings.Current.GroupProtectionCleanse == "Me" && Me.HasDebuffType("Poison","Disease","Magic"), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Divine Plea"), 5f, (s, t) => Me.CManaPercentage() < Settings.Current.GeneralDivinePlea, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Divine Protection"), 7f, (s,t) => Settings.Current.DivineProtection && (EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.CIsTargetingMe(), 3) && Me.HealthPercent < 85 || BossList.isboss && Me.HealthPercent < 85), RotationCombatUtil.FindMe, checkRange:false),
            new RotationStep(new RotationSpell("Hammer of Justice"), 8f, (s,t) => t.CCanInterruptCasting() && Settings.Current.GroupProtectionHammerofJustice, RotationCombatUtil.BotTargetFast),            
            new RotationStep(new RotationSpell("Hand of Salvation"), 9.1f, (s,t) => EnemiesAttackingGroup.Any(unit => unit.CIsTargetingMeOrMyPetOrPartyMember()), RotationCombatUtil.CFindHeal),
            new RotationStep(new RotationSpell("Hand of Protection"), 9.2f, (s,t) => 
            Settings.Current.GroupProtectionHoP
            && EnemiesAttackingGroup.Any(unit => unit.CIsTargetingMeOrMyPetOrPartyMember()) 
            && t.HealthPercent < 50 
            && (t.WowClass == WoWClass.Mage || t.WowClass == WoWClass.Warlock || t.WowClass == WoWClass.Priest || t.WowClass == WoWClass.Druid), RotationCombatUtil.CFindPartyMember,checkLoS:true),
            new RotationStep(new RotationSpell("Avenger's Shield"), 10f, (s,t) => Me.CManaPercentage() > 20, RotationCombatUtil.BotTargetFast, checkLoS:true),
            new RotationStep(new RotationSpell("Avenging Wrath"), 11f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.CIsTargetingMe(), 2) && Settings.Current.GroupAvengingWrathProtection,RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Judgement of Light"), 12f, (s,t) => !SpellManager.KnowSpell("Judgement of Wisdom"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Exorcism"), 13f, (s,t) => (t.IsCreatureType("Undead") || t.IsCreatureType("Demon")) && Me.ManaPercentage > 25, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Hammer of Wrath"), 14f, (s,t) => t.CHealthPercent() < 20 && Me.CManaPercentage() > 50 , FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell("Hammer of the Righteous"), 16f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shield of Righteousness"), 17f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Holy Shield"), 18f, RotationCombatUtil.Always, RotationCombatUtil.FindMe, checkRange:false)
        };

        private bool DoPreCalculations()
        {
            if (LimitExecutionSpeed(100))
            {
                return true;
            }
            Cache.Reset();
            EnemiesAttackingGroup = RotationFramework.Enemies.Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember())
                .ToArray();
            return false;
        }

        private bool LimitExecutionSpeed(int delay)
        {
            if (watch.ElapsedMilliseconds > delay)
            {
                watch.Restart();
                return false;
            }
            return true;
        }

        public WoWUnit FindEnemyAttackingGroup(Func<WoWUnit, bool> predicate) => EnemiesAttackingGroup.FirstOrDefault(predicate);
    }
}
