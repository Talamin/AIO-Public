using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    using Settings = WarlockLevelSettings;
    internal class GroupAffliction : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations", ignoresGlobal: true), 0.0f,(action,unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Drain Soul"), 2.5f, (s,t) => !t.IsBoss && t.HealthPercent <= 25 && ItemsHelper.GetItemCount("Soul Shard") <= 3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Demonic Empowerment"), 3f, (s,t) => !Pet.CHaveBuff("Demonic Empowerment") && Pet.IsAlive && Pet.IsMyPet, RotationCombatUtil.FindPet),            
            new RotationStep(new RotationSpell("Life Tap"), 4f, (s,t) => !Me.HaveBuff("Life Tap") && Settings.Current.GlyphLifeTap && Me.HealthPercent > 30,RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Life Tap"), 4.1f, (s,t) => Me.HealthPercent > 30 && Me.ManaPercentage < Settings.Current.GroupAfflictionLifetap,RotationCombatUtil.FindMe),

            //AOE
            new RotationStep(new RotationSpell("Seed of Corruption"), 4.4f, (s,t) =>  Settings.Current.GroupAfflictionUseSeedGroup 
            &&  !t.CHaveMyBuff("Seed of Corruption") 
            && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=15) >= Settings.Current.GroupAfflictionAOECount 
            && Settings.Current.GroupAfflictionUseAOE, RotationCombatUtil.FindEnemyAttackingGroupAndMe),

            new RotationStep(new RotationSpell("Corruption"), 8f, (s,t) => Settings.Current.GroupAfflictionUseCorruptionGroup &&
            !t.HaveMyBuff("Corruption") && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) < Settings.Current.GroupAfflictionAOECount && Settings.Current.GroupAfflictionUseAOE, RotationCombatUtil.FindEnemyAttackingGroupAndMe),
            
            new RotationStep(new RotationSpell("Shadow Bolt"), 5f, (s,t) => Me.HaveBuff("Shadow Trance"),RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Health Funnel"), 6f, (s,t) => !Pet.HaveBuff("Health Funnel") && Pet.HealthPercent < Settings.Current.GroupAfflictionHealthfunnelPet && Me.HealthPercent > Settings.Current.GroupAfflictionHealthfunnelMe && Pet.IsAlive && Pet.IsMyPet, RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell("Haunt"), 7.5f, (s,t) => !t.CHaveMyBuff("Haunt"), RotationCombatUtil.BotTargetFast),
            //Curses
            new RotationStep(new RotationSpell("Curse of Agony"), 10f, (s,t) => !t.CHaveMyBuff("Curse of Agony") && Settings.Current.GroupAfflictionAfflCurse == "Agony", RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Curse of Doom"), 10.1f, (s,t) => !t.CHaveMyBuff("Curse of Doom") && Settings.Current.GroupAfflictionAfflCurse == "Doom", RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Curse of the Elements"), 10.2f, (s,t) => !t.CHaveMyBuff("Curse of the Elements") && Settings.Current.GroupAfflictionAfflCurse == "Elements", RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Curse of Tongues"), 10.3f, (s,t) => !t.CHaveMyBuff("Curse of Tongues") && Settings.Current.GroupAfflictionAfflCurse == "Tongues", RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Curse of Weakness"), 10.4f, (s,t) => !t.CHaveMyBuff("Curse of Weakness") && Settings.Current.GroupAfflictionAfflCurse == "Weakness", RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Curse of Exhaustion"), 10.4f, (s,t) => !t.CHaveMyBuff("Curse of Exhaustion") && Settings.Current.GroupAfflictionAfflCurse == "Exhaustion", RotationCombatUtil.BotTargetFast),
            //
            new RotationStep(new RotationSpell("Corruption"), 11f, (s,t) => !t.CHaveMyBuff("Corruption"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Drain Life"), 12f, (s,t) => Me.HealthPercent < Settings.Current.GroupAfflictionDrainlife, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Unstable Affliction"), 13f, (s,t) => !t.CHaveMyBuff("Unstable Affliction"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Immolate"), 14f, (s,t) => !t.CHaveMyBuff("Immolate") && !SpellManager.KnowSpell("Unstable Affliction"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shadow Bolt"), 16f ,(s,t) => t.CHealthPercent() > Settings.Current.UseWandTresh && !Settings.Current.GroupAfflictionShadowboltWand, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shadow Bolt"), 17f ,(s,t) => Settings.Current.GroupAfflictionShadowboltWand, RotationCombatUtil.BotTargetFast)
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
