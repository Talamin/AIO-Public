using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Lists;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class GroupRetribution : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();

        protected override List<RotationStep> Rotation => new List<RotationStep> 
        {
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f,(action,unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true, ignoreGCD: true),
            new RotationStep(new RotationAction("Cache debuffed party members", RotationCombatUtil.CacheLUADebuffedPartyMembersStep), 0f, 1000),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Divine Plea"), 1.1f, (s, t) => Me.CManaPercentage() < Settings.Current.GeneralDivinePlea, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hand of Freedom"), 1.2f, (s, t) => Me.Rooted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Divine Protection"), 1.3f,  (s,t) => Settings.Current.DivineProtection && EnemiesAttackingGroup.ContainsAtLeast(enem=> enem.CIsTargetingMe(), 2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Sacred Shield"), 1.5f, (s,t) => !Me.CHaveBuff("Sacred Shield"), RotationCombatUtil.FindMe),
            
            new RotationStep(new RotationSpell("Purify"), 2f, (s,t) => 
                Settings.Current.GroupRetributionPurify 
                && RotationCombatUtil.IHaveCachedDebuff(new List<DebuffType>() { DebuffType.Disease, DebuffType.Poison }), 
                RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Purify"), 2.1f, (s,t) => 
                Settings.Current.GroupRetributionPurifyMember,
                p => RotationCombatUtil.GetPartyMemberWithCachedDebuff(new List<DebuffType>() { DebuffType.Disease, DebuffType.Poison }, true, 30)),
            
            new RotationStep(new RotationSpell("Divine Plea"), 3.5f, (s, t) => Me.CManaPercentage() < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaIC, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Flash of Light"), 4f, (s,t) => Me.HaveBuff("The Art of War") && Me.HealthPercent <= 60 && Settings.Current.GroupRetributionHealInCombat , RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hammer of Justice"), 5f, (s, t) => t.CIsCast() && t.CGetDistance() >= 10, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Hammer of Justice"), 5.1f, (s,t) => t.Fleeing, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Hammer of Wrath"), 7f, (s,t) => t.CHealthPercent() <20 , RotationCombatUtil.FindEnemy, checkLoS: true),

            new RotationStep(new RotationSpell("Holy Light"), 9f, (s,t) => Me.CHealthPercent() <=  Settings.Current.GroupRetributionHL && Settings.Current.GroupRetributionHealInCombat, RotationCombatUtil.FindMe),      

            new RotationStep(new RotationSpell("Avenging Wrath"), 13f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(enem=> enem.CGetDistance() <= 20, 3) &&  Settings.Current.GroupAvengingWrathRetribution, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Judgement of Light"), 14f, (s,t) => !SpellManager.KnowSpell("Judgement of Wisdom"), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Judgement of Wisdom"), 15f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Divine Storm"), 16f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Crusader Strike"), 17f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Consecration"), 18f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.Count(unit => unit.CGetDistance() <=8) >= Settings.Current.GroupRetributionConsecration, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Exorcism"), 19f, (s,t) => (Me.CHaveBuff("The Art of War") && (t.CHealthPercent() > 10 || BossList.MyTargetIsBoss)) || !TalentsManager.HaveTalent(3, 17), RotationCombatUtil.BotTargetFast, checkRange: true),
            new RotationStep(new RotationSpell("Holy Wrath"), 21f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast, checkRange: true),
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
