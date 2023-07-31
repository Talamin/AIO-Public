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
    internal class SoloRetribution : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Divine Plea"), 1.1f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hand of Freedom"), 1.2f, (s, t) => Me.Rooted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Divine Protection"), 1.3f,  (s,t) => Settings.Current.DivineProtection && RotationFramework.Enemies.Count(u=> u.IsTargetingMe) >=2, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Sacred Shield"), 1.5f, (s,t) => !Me.HaveBuff("Sacred Shield"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Purify"), 2f, (s,t) => !Me.IsInGroup && (Me.HasDebuffType("Disease") || Me.HasDebuffType("Poison")), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Purify"), 3f, (s,t) => (t.HasDebuffType("Disease") || t.HasDebuffType("Poison")) &&  Settings.Current.SoloRetributionPurify, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Divine Plea"), 3.5f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaIC, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Flash of Light"), 4f, (s,t) => (!Me.IsInGroup &&  Me.HaveBuff("The Art of War") && Me.HealthPercent <= 60) || (Me.IsInGroup &&  Me.HaveBuff("The Art of War") && Me.HealthPercent <= 60 && Settings.Current.SoloRetributionHealInCombat) , RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Flash of Light"), 4.1f, (s,t) => Settings.Current.SoloRetributionHealGroup &&  Me.HaveBuff("The Art of War") && t.HealthPercent <= 60 && Settings.Current.SoloRetributionHealInCombat , RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hammer of Justice"), 5f, (s, t) => t.IsCasting() , RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell("Hammer of Justice"), 5.1f, (s,t) => t.Fleeing, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Hammer of Justice"), 6f, (s, t) => RotationFramework.Enemies.Count(o => o.GetDistance <=5) >=2 , RotationCombatUtil.FindEnemy),
            new RotationStep(new RotationSpell("Hammer of Wrath"), 7f, (s,t) => t.HealthPercent <20 , RotationCombatUtil.FindEnemy),
            new RotationStep(new RotationSpell("Hammer of Wrath"), 8f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Holy Light"), 9f, (s,t) => (!Me.IsInGroup && Me.HealthPercent <=  Settings.Current.SoloRetributionHL) || (Me.IsInGroup && Me.HealthPercent <=  Settings.Current.SoloRetributionHL && Settings.Current.SoloRetributionHealInCombat), RotationCombatUtil.FindMe),      
            new RotationStep(new RotationSpell("Flash of Light"), 10f, (s,t) =>!Me.IsInGroup && Me.HealthPercent <=  Settings.Current.SoloRetributionFL && Settings.Current.SoloRetributionHealInCombat, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell("Holy Light"), 10.1f, (s,t) => Settings.Current.SoloRetributionHealGroup && t.HealthPercent <= Settings.Current.SoloRetributionHL && Settings.Current.SoloRetributionHealInCombat, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Flash of Light"), 10.2f, (s,t) =>Settings.Current.SoloRetributionHealGroup && t.HealthPercent <= Settings.Current.SoloRetributionFL && Settings.Current.SoloRetributionHealInCombat, RotationCombatUtil.FindPartyMember),

            new RotationStep(new RotationSpell("Hand of Reckoning"), 11f, (s,t) => t.GetDistance <= 25 && !t.IsTargetingMe && !Me.IsInGroup &&  Settings.Current.SoloRetributionHOR, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Avenger's Shield"), 12f, (s,t) => t.GetDistance <= 25 && Me.ManaPercentage > 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Avenging Wrath"), 13f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=20) >=3 &&  Settings.Current.SoloAvengingWrathRetribution,RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Judgement of Light"), 14f, (s,t) => !SpellManager.KnowSpell("Judgement of Wisdom"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Judgement of Wisdom"), 15f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Divine Storm"), 16f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Crusader Strike"), 17f, (s,t) => true, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Consecration"), 18f, (s,t) => EnemiesAttackingGroup.Count(ene => ene.CGetDistance() <= 10) >= Settings.Current.SoloRetributionConsecration, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Exorcism"), 19f, (s,t) => (Me.HaveBuff("The Art of War") && (t.HealthPercent > 10 || BossList.MyTargetIsBoss)) || !TalentsManager.HaveTalent(3, 17), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Exorcism"), 20f, (s,t) => !TalentsManager.HaveTalent(3, 17), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Holy Wrath"), 21f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
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
