using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Class;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    using Settings = WarriorLevelSettings;
    internal class GroupProtection : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        private readonly Spell _battleStanceSpell = new Spell("Battle Stance");
        private readonly Spell _defensiveStanceSpell = new Spell("Defensive Stance");

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f,
                (action, unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true, ignoreGCD: true),
            new RotationStep(new RotationAction("Check stance", CheckStance), 0f, 5000),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Last Stand"), 1.1f, RotationCombatUtil.Always, _ => Me.CHealthPercent() < 15, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Shield Wall"), 2.11f, RotationCombatUtil.Always, _ => Me.CInCombat() && Me.CHealthPercent() < 65, RotationCombatUtil.FindMe, checkRange: false),
            // new RotationStep(new RotationSpell("Shield Wall"), 2.12f, (s,t) => BossList.MyTargetIsBoss || RotationFramework.Enemies.Count(o => o.CGetDistance() <=10) >=3, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Shield Block"), 2.15f, (s,t) => (t.CHealthPercent() > 70 || t.IsElite || BossList.MyTargetIsBoss) && EnemiesAttackingGroup.ContainsAtLeast(o => o.CGetDistance() <=10 && o.CIsTargetingMe(), Settings.Current.GroupProtectionShieldBlock), _ => Me.CHealthPercent() < 90, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Enraged Regeneration"), 2.151f, RotationCombatUtil.Always, _ => Me.CHealthPercent() <= Settings.Current.GroupProtectionEnragedRegeneration && Me.CHaveBuff("Bloodrage") && Me.CBuffTimeLeft("Bloodrage") <= 2000, RotationCombatUtil.FindMe, checkRange: false),

            new RotationStep(new RotationSpell("Intercept"), 2.16f, (s,t) => Settings.Current.GroupProtectionIntercept && t.CGetDistance() > 8
                && RotationFramework.PartyMembers.Any(m => m.Position.DistanceTo(t.Position) < 7) && t.CIsTargetingMeOrMyPetOrPartyMember() && !t.CIsTargetingMe(), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell("Charge"), 2.17f, (s,t) => Settings.Current.GroupProtectionIntercept && t.CGetDistance() > 8
                && RotationFramework.PartyMembers.Any(m => m.Position.DistanceTo(t.Position) < 7)&& t.CIsTargetingMeOrMyPetOrPartyMember() && !t.CIsTargetingMe(), RotationCombatUtil.BotTargetFast, checkLoS: true),

            new RotationStep(new RotationSpell("Thunder Clap"), 2.18f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.Any(unit => unit.CGetDistance() < 8 && !unit.CHaveMyBuff("Thunder Clap")), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Shield Bash"), 2.19f, (s,t) => t.CCanInterruptCasting(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Spell Reflection"), 2.2f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.Any(unit => unit.CIsTargetingMe() && unit.CIsCast()), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Challenging Shout"), 3.1f, RotationCombatUtil.Always, _ => Me.CIsInGroup() && EnemiesAttackingGroup.ContainsAtLeast(o => o.CGetDistance() <= 10 && !o.CIsTargetingMe(), 2), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Heroic Strike"), 3.2f, RotationCombatUtil.Always, _ => Me.CHaveBuff("Glyph of Revenge") && !RotationCombatUtil.IsCurrentSpell("Heroic Strike"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Mocking Blow"), 3.21f, (s,t) => Me.CRage() >= 10 && !t.CIsTargetingMe() && t.CGetDistance() <= 7, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Taunt"), 4f, (s,t) => Settings.Current.GroupProtectionTauntGroup, TauntUnitPrio),
            new RotationStep(new RotationSpell("Taunt"), 4.1f, (s,t) => !t.CIsTargetingMe(), _ => Settings.Current.GroupProtectionTauntGroup, FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell("Mocking Blow"), 4.15f, (s,t) => !t.CIsTargetingMe() && t.CGetDistance() <= 8, FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell("Shield Slam"), 5.1f, RotationCombatUtil.Always, _ => Me.CHaveBuff("Sword and Board"), RotationCombatUtil.BotTargetFast),
            // new RotationStep(new RotationSpell("Piercing Howl"), 6f, RotationCombatUtil.Always, _ => Me.CHealthPercent() < 40 && EnemiesAttackingGroup.ContainsAtLeast(o => o.CGetDistance() <=10 && !o.CHaveBuff("Piercing Howl"), 3), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Berserker Rage"), 7f, (s,t) => !Me.CHaveBuff("Enraged Regeneration") && Me.CRage() < 50 && t.TargetObject.GetDistance < 8, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Shockwave"), 8f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.ContainsAtLeast(o => o.CGetDistance() < 10, 2), RotationCombatUtil.FindMe, checkRange: false),          
            // new RotationStep(new RotationSpell("Thunder Clap"), 10.1f, RotationCombatUtil.Always, _ => Me.CIsInGroup() && EnemiesAttackingGroup.Any(unit => unit.CGetDistance() < 8 && !unit.CHaveMyBuff("Thunder Clap")), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Revenge"), 10.2f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shield Slam"), 10.3f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell("Devastate"), 10.4f, (s,t) => (Me.CRage() > 70 || t.CMyBuffStack("Sunder Armor") < 5) && BossList.MyTargetIsBoss, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Devastate"), 10.5f, (s,t) => Me.CRage() > 70 || !t.CHaveMyBuff("Sunder Armor"), RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell("Bloodrage"), 11f, (s,t) => Me.CRage() < 70, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Cleave"), 13f, RotationCombatUtil.Always, _ => Me.CRage() > Settings.Current.GroupProtectionCleaveRageCount && !Me.CHaveBuff("Glyph of Revenge") && EnemiesAttackingGroup.ContainsAtLeast(o => o.CGetDistance() < 10, Settings.Current.GroupProtectionCleaveCount), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Concussion Blow"), 15f, (s,t) => t.CHealthPercent()  > 40, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Demoralizing Shout"), 19f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.ContainsAtLeast(unit => unit.CGetDistance() < 15 && unit.CHealthPercent() > 65 && !unit.CHaveBuff("Demoralizing Shout") && !unit.CHaveBuff("Demoralizing Roar"), Settings.Current.GroupProtectionDemoralizingCount), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell("Rend"), 20f, (s,t) => t.CHealthPercent() >50 && !t.CHaveMyBuff("Rend") && !t.IsCreatureType("Elemental"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Victory Rush"), 21f, RotationCombatUtil.Always, _ => !Me.CHaveBuff("Defensive Stance") && (Me.CHaveBuff("Battle Stance") || Me.CHaveBuff("Berserker Stance")), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Heroic Strike"), 22f, RotationCombatUtil.Always, _ => Me.CRage() >= 40 && Me.Level < 40 && !RotationCombatUtil.IsCurrentSpell("Heroic Strike"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Heroic Strike"), 23f, RotationCombatUtil.Always, _ => Me.CRage() >= 80 && Me.Level >= 40 && !RotationCombatUtil.IsCurrentSpell("Heroic Strike"), RotationCombatUtil.BotTargetFast),
        };

        private bool CheckStance()
        {
            if (!_defensiveStanceSpell.KnownSpell && RotationCombatUtil.GetLUAActiveShapeshiftName() != "Battle Stance")
                _battleStanceSpell.Launch();
            if (_defensiveStanceSpell.KnownSpell && RotationCombatUtil.GetLUAActiveShapeshiftName() != "Defensive Stance")
                _defensiveStanceSpell.Launch();
            return false;
        }


        private bool DoPreCalculations()
        {
            if (LimitExecutionSpeed(100)) return true;
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

        public WoWUnit FindEnemyAttackingGroup(Func<WoWUnit, bool> predicate)
            => EnemiesAttackingGroup.FirstOrDefault(predicate);


        public WoWUnit TauntUnitPrio(Func<WoWUnit, bool> predicate)
        {
            List<WoWUnit> enemiesToTaunt = new List<WoWUnit>();
            foreach (WoWUnit unit in RotationFramework.PartyMembers)
            {
                foreach (WoWUnit attacker in EnemiesAttackingGroup)
                {
                    if (!attacker.CIsTargetingMe() && !enemiesToTaunt.Contains(attacker) && unit.CGetPosition().DistanceTo(attacker.CGetPosition()) > unit.CGetPosition().DistanceTo(Me.CGetPosition()))
                    {
                        enemiesToTaunt.AddSorted(attacker, a => unit.CGetPosition().DistanceTo(attacker.CGetPosition()));
                    }
                }
            }
            return enemiesToTaunt.FirstOrDefault();
        }
    }
}
