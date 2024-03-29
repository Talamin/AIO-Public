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
        private Stopwatch watch = Stopwatch.StartNew();
        private List<WoWUnit> _enemiesWithoutMyCOA = new List<WoWUnit>();
        private List<WoWUnit> _enemiesWithoutMyUA = new List<WoWUnit>();
        private List<WoWUnit> _enemiesWithoutMyCorr = new List<WoWUnit>();

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f,(action, unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange : false, forceCast : true, ignoreGCD : true),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Drain Soul"), 2.5f, (s,t) => !BossList.MyTargetIsBoss && t.HealthPercent <= 25 && ItemsHelper.GetItemCount("Soul Shard") <= 3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Demonic Empowerment"), 3f, (s,t) => !Pet.CHaveBuff("Demonic Empowerment") && Pet.IsAlive && Pet.IsMyPet, RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell("Life Tap"), 4f, (s,t) => !Me.CHaveBuff("Life Tap") && Settings.Current.GlyphLifeTap && Me.HealthPercent > 30,RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Life Tap"), 4.1f, (s,t) => Me.CHealthPercent() > 30 && Me.ManaPercentage < Settings.Current.GroupAfflictionLifetap,RotationCombatUtil.FindMe),

            //AOE
            new RotationStep(new RotationSpell("Seed of Corruption"), 4.4f, (s,t) =>  Settings.Current.GroupAfflictionUseSeedGroup
                &&  !t.CHaveMyBuff("Seed of Corruption")
                && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <= 15) >= Settings.Current.GroupAfflictionAOECount
                && Settings.Current.GroupAfflictionUseAOE, RotationCombatUtil.FindEnemyAttackingGroupAndMe),

            new RotationStep(new RotationSpell("Shadow Bolt"), 5f, (s,t) => Me.CHaveBuff("Shadow Trance"), FindShadowTranceTarget),
            new RotationStep(new RotationSpell("Health Funnel"), 6f, (s,t) => !Pet.CHaveBuff("Health Funnel") && Pet.CHealthPercent() < Settings.Current.GroupAfflictionHealthfunnelPet && Me.CHealthPercent() > Settings.Current.GroupAfflictionHealthfunnelMe && Pet.IsAlive && Pet.IsMyPet, RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell("Haunt"), 7.6f, (s,t) => BossList.MyTargetIsBoss && !t.CHaveMyBuff("Haunt"), RotationCombatUtil.BotTargetFast),

            // DoT spread
            new RotationStep(new RotationAction("Search for enemies to DoT", SearchForEnemiesToDoT), 7.9f),
            new RotationStep(new RotationSpell("Corruption"), 8f, (s,t) => Settings.Current.GroupAfflictionSpreadCorruption, _enemiesWithoutMyCorr.FirstOrDefault),
            new RotationStep(new RotationSpell("Unstable Affliction"), 8.5f, (s,t) => Settings.Current.GroupAfflictionSpreadUnstableAffliction, _enemiesWithoutMyUA.FirstOrDefault, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Curse of Agony"), 9f, (s,t) => Settings.Current.GroupAfflictionAfflCurse == "Agony" && Settings.Current.GroupAfflictionSpreadCurseOfAgony, _enemiesWithoutMyCOA.FirstOrDefault),

            new RotationStep(new RotationSpell("Corruption"), 9.1f, (s,t) => !t.CHaveMyBuff("Corruption"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Unstable Affliction"), 9.2f, (s,t) => !t.CHaveMyBuff("Unstable Affliction"), RotationCombatUtil.BotTarget, preventDoubleCast: true),

            //Curses
            new RotationStep(new RotationSpell("Curse of Agony"), 10f, (s,t) => !t.CHaveMyBuff("Curse of Agony") && Settings.Current.GroupAfflictionAfflCurse == "Agony", RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Curse of Doom"), 10.1f, (s,t) => !t.CHaveMyBuff("Curse of Doom") && Settings.Current.GroupAfflictionAfflCurse == "Doom", RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Curse of the Elements"), 10.2f, (s,t) => !t.CHaveMyBuff("Curse of the Elements") && Settings.Current.GroupAfflictionAfflCurse == "Elements", RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Curse of Tongues"), 10.3f, (s,t) => !t.CHaveMyBuff("Curse of Tongues") && Settings.Current.GroupAfflictionAfflCurse == "Tongues", RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Curse of Weakness"), 10.4f, (s,t) => !t.CHaveMyBuff("Curse of Weakness") && Settings.Current.GroupAfflictionAfflCurse == "Weakness", RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Curse of Exhaustion"), 10.4f, (s,t) => !t.CHaveMyBuff("Curse of Exhaustion") && Settings.Current.GroupAfflictionAfflCurse == "Exhaustion", RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell("Drain Life"), 12f, (s,t) => Me.HealthPercent < Settings.Current.GroupAfflictionDrainlife, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shadow Bolt"), 15f ,(s,t) => Settings.Current.GroupAfflictionShadowboltOverWand, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shadow Bolt"), 17f ,(s,t) => t.CHealthPercent() > Settings.Current.UseWandTresh && !Settings.Current.GroupAfflictionShadowboltOverWand, RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell("Shoot"), 25f, (s,t) => Me.CManaPercentage() < 5 && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTargetFast, checkLoS: true),
        };

        private WoWUnit FindShadowTranceTarget(Func<WoWUnit, bool> predicate) => RotationFramework.Enemies
            .OrderByDescending(u => u.HealthPercent)
            .FirstOrDefault(u => u.IsAttackable
                                && u.GetDistance < 29
                                && Me.IsFacing(u.Position, 3)
                                && u.IsTargetingMeOrMyPetOrPartyMember
                                && !TraceLine.TraceLineGo(u.Position)
                                && predicate(u));

        private bool SearchForEnemiesToDoT()
        {
            _enemiesWithoutMyCorr.Clear();
            _enemiesWithoutMyUA.Clear();
            _enemiesWithoutMyCOA.Clear();
            foreach (WoWUnit unit in RotationFramework.Enemies)
            {
                if (unit.IsAttackable
                    && unit.IsTargetingMeOrMyPetOrPartyMember
                    && unit.GetDistance < 29
                    //&& unit.IsElite
                    && !TraceLine.TraceLineGo(unit.PositionWithoutType))
                {
                    if (!unit.CHaveMyBuff("Corruption"))
                        _enemiesWithoutMyCorr.Add(unit);
                    if (!unit.CHaveMyBuff("Unstable Affliction"))
                        _enemiesWithoutMyUA.Add(unit);
                    if (!unit.CHaveMyBuff("Curse of Agony"))
                        _enemiesWithoutMyCOA.Add(unit);
                }
            }
            return false;
        }

        private bool DoPreCalculations()
        {
            if (LimitExecutionSpeed(100))
            {
                return true;
            }
            Cache.Reset();
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
    }
}
