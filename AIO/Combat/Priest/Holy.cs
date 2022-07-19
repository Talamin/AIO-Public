using System;
using System.Collections.Generic;
using System.Linq;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Lists;
using AIO.Settings;
using robotManager.Helpful;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Priest {
    using Settings = PriestLevelSettings;

    internal class Holy : BaseRotation {
        private const bool IsDebug = true;
        private static readonly LinkedList<WoWUnit> CastingOnMeOrGroup = new LinkedList<WoWUnit>();
        private static readonly LinkedList<WoWUnit> EnemiesTargetingMe = new LinkedList<WoWUnit>();
        private static CancelableSpell _slowHealSpell;
        private static CancelableSpell _fastHealSpell;
        private static RotationSpell _diseaseSpell;
        private static bool _castDivineHymn;
        private static bool _groupInCombat;
        private static bool _ignoreManaManagementOoc;
        private static bool _shouldCastOffTank;
        private static bool _isSpirit;
        private static WoWUnit _tank;

        // private const float Default = 1f;


        public Holy() : base(IsDebug, IsDebug,
            Settings.Current.UseSyntheticCombatEvents) {
            Logging.Write("Loading FlXWare's Heal Rotation ...");
            _slowHealSpell = FindCorrectSlowHealSpell();
            _fastHealSpell = FindCorrectFastHealSpell();
            _diseaseSpell = FindCorrectRemoveDiseaseSpell();
        }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            /*
             * Pre Calculations
             */

            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f, (action, me) => DoPreCalculations(),
                RotationCombatUtil.FindMe),

            /*
             * Critical section
             */

            // Cast Guardian Spirit on tank if he's about to die (Interrupts)
            new RotationStep(new RotationSpell("Guardian Spirit"), 1.0f,
                (action, tank) => tank.InCombat && tank.HealthPercent < Settings.Current.HolyGuardianSpiritTresh &&
                                  !tank.CHaveBuff("Guardian Spirit"),
                FindTank, forceCast: true),

            // Cast Guardian Spirit on me if I'm about to die (Interrupts)
            new RotationStep(new RotationSpell("Guardian Spirit"), 1.1f, RotationCombatUtil.Always,
                action => !_isSpirit && Me.InCombat && Me.HealthPercent < Settings.Current.HolyGuardianSpiritTresh &&
                          !Me.CHaveBuff("Guardian Spirit"),
                RotationCombatUtil.FindMe,
                forceCast: true, checkRange: false),

            // Cast Fear Ward on myself if enemy is casting a fear inducing spell on me (Interrupts)
            new RotationStep(new RotationSpell("Fear Ward"), 1.2f, RotationCombatUtil.Always,
                action => !_isSpirit && Settings.Current.HolyProtectAgainstFear &&
                          Me.BuffTimeLeft("Fear Ward") < 3000 &&
                          anyoneCastingFearSpellOnMe(CastingOnMeOrGroup),
                RotationCombatUtil.FindMe, null, true, false),

            // Abort
            new RotationStep(new DebugSpell("Abort"), 1.21f, (action, me) => HandleAbort(),
                RotationCombatUtil.FindMe),

            // Cast Inner Focus
            new RotationStep(new RotationSpell("Inner Focus"), 1.3f, RotationCombatUtil.Always,
                action => !Me.CHaveBuff("Inner Focus") && _castDivineHymn,
                RotationCombatUtil.FindMe),

            // Cast Divine Hymn if we've got Inner Focus or the situation becomes critical (Burst if dead)
            new RotationStep(new RotationSpell("Divine Hymn"), 1.4f, RotationCombatUtil.Always,
                action => Me.CHaveBuff("Spirit of Redemption") || Me.CHaveBuff("Inner Focus") || _castDivineHymn,
                RotationCombatUtil.FindMe),

            // Cast Fear Ward on tank if enemy is casting fear inducing spell on him
            new RotationStep(new RotationSpell("Fear Ward"), 1.5f,
                (action, tank) => tank.BuffTimeLeft("Fear Ward") < 3000 &&
                                  anyoneCastingFearSpellOnUnit(tank, CastingOnMeOrGroup),
                action => Settings.Current.HolyProtectAgainstFear,
                FindTank),

            // Do nothing if the NOP condition is met
            new RotationStep(new DebugSpell("Do-NOP"), 1.6f, (action, unit) => DoNop(), RotationCombatUtil.FindMe),

            // Cast Power Word: Shield on me if possible and looks like I'm taking damage (helps to keep casting)
            new RotationStep(new RotationBuff("Power Word: Shield"), 1.7f,
                (action, me) => me.HealthPercent < 70 &&
                                !(me.CHaveBuff("Weakened Soul") || me.CHaveBuff("Power Word: Shield")),
                action => !_isSpirit && Me.InCombat && (Me.CManaPercentage() > 50 || Me.HealthPercent < 40),
                RotationCombatUtil.FindMe),

            /*
             * Escape section
             */

            // Cast Fade on me if enemies are targeting me and there are party members who can take the aggro
            new RotationStep(new RotationSpell("Fade"), 2.0f, RotationCombatUtil.Always,
                action => !_isSpirit && EnemiesTargetingMe.Any() && !Me.CHaveBuff("Fade") &&
                          RotationCombatUtil.CCountAlivePartyMembers(partyMember =>
                              true) > 0, RotationCombatUtil.FindMe),

            // Cast Mind Soothe if we're too close to an enemy and could potentially pull him
            new RotationStep(new RotationSpell("Mind Soothe"), 2.1f,
                (action, enemy) => !enemy.InCombat && !enemy.IsMyTarget && !enemy.PlayerControlled &&
                                   enemy.IsAttackable && enemy.IsCreatureType("Humanoid") &&
                                   enemy.BuffTimeLeft("Mind Soothe") < 1000 && enemy.GetDistance - enemy.AggroDistance <
                                   Settings.Current.HolyMindSootheDistance,
                action => !_isSpirit && _shouldCastOffTank && Me.CManaPercentage() > 50 &&
                          Settings.Current.HolyUseMindSoothe,
                RotationCombatUtil.FindEnemy),

            // Cast Psychic Scream if we're still being attacked by more than 2 enemies which are in range
            new RotationStep(new RotationSpell("Psychic Scream"), 2.2f, RotationCombatUtil.Always,
                action => !_isSpirit && RotationCombatUtil.CountEnemies(enemy =>
                    enemy.IsTargetingMe && enemy.GetDistance < 8 && !enemy.CHaveBuff("Psychic Scream")) >= 2,
                RotationCombatUtil.FindMe),

            // Cast Shackle Undead to freeze undead enemies targeting us but out of combat reach to get some time
            new RotationStep(new RotationSpell("Shackle Undead"), 2.3f,
                (action, enemy) => enemy.IsCreatureType("Undead") && enemy.GetDistance > enemy.CombatReach,
                action => !_isSpirit && Me.CManaPercentage() > 50 && _shouldCastOffTank &&
                          Settings.Current.HolyShackleUndead &&
                          CanShackleNew(),
                predicate => EnemiesTargetingMe.FirstOrDefault(predicate)),

            /*
             * Heal section
             */

            // Cast Prayer of Healing if it's cheaper than healing them individually
            new RotationStep(new CancelableSpell("Prayer of Healing", me => !ShouldCastPrayerOfHealing()), 2.99f,
                RotationCombatUtil.Always,
                action => (_isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank) && ShouldCastPrayerOfHealing(),
                RotationCombatUtil.FindMe),

            // Cast Big Single Target Heal on Tank
            new RotationStep(_slowHealSpell, 3.00f,
                (action, tank) => tank.HealthPercent < Settings.Current.HolyBigSingleTargetHeal &&
                                  (_groupInCombat && Me.BuffStack("Serendipity") >= 2 || _isSpirit &&
                                      Me.BuffStack("Serendipity") >= 3 || !_groupInCombat),
                FindTank),

            // Cast Power Word: Shield on me if possible and looks like I'm taking damage (helps to keep casting)
            new RotationStep(new RotationBuff("Power Word: Shield"), 1.7f,
                (action, tank) => tank.HealthPercent < 70 && tank.InCombat &&
                                  !(tank.CHaveBuff("Weakened Soul") || tank.CHaveBuff("Power Word: Shield")),
                action => Me.CManaPercentage() > 40,
                RotationCombatUtil.FindTank),

            // Cast Flash Heal on Tank
            new RotationStep(_fastHealSpell, 3.01f,
                (action, tank) => tank.HealthPercent < 80,
                FindTank),

            // Cast Binding Heal on me and tank
            new RotationStep(new RotationSpell("Binding Heal"), 3.03f,
                (action, tank) => tank.HealthPercent < Settings.Current.HolyBindingHealTresh,
                action => !_isSpirit && Me.HealthPercent < Settings.Current.HolyBindingHealTresh,
                FindTank),

            // Cast renew on tank
            new RotationStep(new RotationSpell("Renew"), 3.02f,
                (action, tank) => tank.InCombat && tank.HealthPercent < 95 && !tank.CHaveBuff("Renew"),
                FindTank),

            // Cast Big Single Target Heal on Me
            new RotationStep(_slowHealSpell, 3.04f,
                (action, me) => !_isSpirit && me.HealthPercent < Settings.Current.HolyBigSingleTargetHeal - 5
                                           && (_groupInCombat && Me.BuffStack("Serendipity") >= 2 || !_groupInCombat),
                RotationCombatUtil.FindMe),

            // Cast Flash Heal on Me
            new RotationStep(_fastHealSpell, 3.05f,
                (action, me) => !_isSpirit && me.HealthPercent < 75,
                RotationCombatUtil.FindMe),

            // Cast Binding Heal on me and party member
            new RotationStep(new RotationSpell("Binding Heal"), 3.06f,
                (action, partyMember) => partyMember.HealthPercent < Settings.Current.HolyBindingHealTresh,
                action => !_isSpirit && (_ignoreManaManagementOoc || _shouldCastOffTank) &&
                          Me.HealthPercent < Settings.Current.HolyBindingHealTresh,
                RotationCombatUtil.FindExplicitPartyMember),

            // Cast renew on me
            new RotationStep(new RotationSpell("Renew"), 3.07f,
                (action, me) => !_isSpirit && me.InCombat && me.HealthPercent < 95 && !Me.CHaveBuff("Renew"),
                RotationCombatUtil.FindMe),

            // TODO: Implement better Chain-Clustering instead of the current AoE clustering
            // Cast Prayer of Mending on party members
            new RotationStep(new RotationSpell("Prayer of Mending"), 3.09f,
                (action, partyMember) => partyMember.InCombat &&
                                         partyMember.HealthPercent < Settings.Current.HolyPrayerOfMendingTresh &&
                                         RotationCombatUtil.CCountAlivePartyMembers(player =>
                                             player.HealthPercent < Settings.Current.HolyPrayerOfMendingTresh &&
                                             partyMember.Position.DistanceTo(player.Position) < 20) >= 2,
                action => _shouldCastOffTank &&
                          !RotationFramework.PartyMembers.Any(player => player.HaveMyBuff("Prayer of Mending")),
                RotationCombatUtil.FindPartyMember),

            // Cast Circle of Healing if it's cheaper than healing them individually and Prayer of Healing wasn't enough
            new RotationStep(new RotationSpell("Circle of Healing"), 3.10f, RotationCombatUtil.Always,
                action => (_isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank) &&
                          RotationCombatUtil.CCountAlivePartyMembers(partyMember =>
                              partyMember.HealthPercent < Settings.Current.HolyCircleOfHealingTresh &&
                              partyMember.GetDistance < 18) > 2,
                RotationCombatUtil.FindMe),

            // Cast Big Single Target Heal on Party Member
            new RotationStep(_slowHealSpell, 3.11f,
                (action, partyMember) => partyMember.HealthPercent < Settings.Current.HolyBigSingleTargetHeal - 15,
                action => _isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank ||
                          Me.BuffStack("Serendipity") >= 2,
                RotationCombatUtil.FindExplicitPartyMember),

            // Cast Flash Heal on party members
            new RotationStep(_fastHealSpell, 3.12f,
                (action, partyMember) => partyMember.HealthPercent < 70
                                         || Me.CHaveBuff("Surge of Light") && partyMember.HealthPercent < 99,
                action => _isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank,
                RotationCombatUtil.FindExplicitPartyMember),

            // Cast renew on party member
            new RotationStep(new RotationSpell("Renew"), 3.13f,
                (action, partyMember) => partyMember.InCombat && partyMember.HealthPercent < 90 &&
                                         !partyMember.CHaveBuff("Renew"),
                action => _isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank,
                RotationCombatUtil.FindExplicitPartyMember),

            /*
             * Mana section
             */

            // Cast Shadowfiend on tank's combat partner to gain some mana
            new RotationStep(new RotationSpell("Shadowfiend"), 4.0f,
                (action, target) => target.IsEnemy() && target.IsAttackable && target.Target == _tank.Guid,
                action => _groupInCombat && _tank != null && _tank.IsAlive && Me.CManaPercentage() < 40,
                predicate => predicate(_tank?.TargetObject) ? _tank?.TargetObject : null),

            // Cast Shadowfiend on high HP combat partner to gain some Mana
            new RotationStep(new RotationSpell("Shadowfiend"), 4.1f,
                (action, target) => target.IsEnemy() && target.IsAttackable,
                action => _groupInCombat && Me.CManaPercentage() < 40,
                RotationCombatUtil.CGetHighestHpPartyMemberTarget),

            // Cast Shadowfiend on basically anything targeting me to gain some Mana
            new RotationStep(new RotationSpell("Shadowfiend"), 4.2f,
                (action, target) => target.IsEnemy() && target.IsAttackable,
                action => Me.InCombat && Me.CManaPercentage() < 30,
                predicate => EnemiesTargetingMe.FirstOrDefault()),

            // Cast Hymn of Hope on me and/or party members
            new RotationStep(new RotationSpell("Hymn of Hope"), 4.3f, RotationCombatUtil.Always,
                action => Me.InCombat && (Me.CManaPercentage() < 20 || RotationCombatUtil.CCountAlivePartyMembers(
                    partyMember =>
                        partyMember.ManaPercentage < 30 && partyMember.GetDistance < 40 &&
                        partyMember.HasMana()) > 2),
                RotationCombatUtil.FindMe),

            /*
             * Protect section
             */

            // Cast Fear Ward on party member if enemy is casting fear inducing spell on him and no tank
            new RotationStep(new RotationSpell("Fear Ward"), 5.0f,
                (action, partyMember) => partyMember.BuffTimeLeft("Fear Ward") < 3000 &&
                                         anyoneCastingFearSpellOnUnit(partyMember, CastingOnMeOrGroup),
                action => _tank == null && Settings.Current.HolyProtectAgainstFear, // low mana usage - off tank is ok
                RotationCombatUtil.FindExplicitPartyMember),

            /*
             * Buff section
             */

            // Cast Prayer of Fortitude if it's cheaper than buffing everyone
            new RotationStep(new RotationSpell("Prayer of Fortitude"), 6.0f, RotationCombatUtil.Always,
                action =>  Settings.Current.UseAutoBuffInt && (_isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank) &&
                          ItemsManager.HasItemById(44615) &&
                          RotationCombatUtil.CCountAlivePartyMembers(CanGiveFortitude) > 2,
                RotationCombatUtil.FindMe),

            // Cast Power Word Fortitude if it's cheaper to buff individually
            new RotationStep(new RotationSpell("Power Word: Fortitude"), 6.1f,
                (action, partyMember) => Settings.Current.UseAutoBuffInt && CanGiveFortitude(partyMember),
                action => _isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank,
                RotationCombatUtil.FindExplicitPartyMember),

            // Cast Prayer of Spirit if it's cheaper than buffing everyone
            new RotationStep(new RotationSpell("Prayer of Spirit"), 6.2f, RotationCombatUtil.Always,
                action => (_isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank) &&
                          ItemsManager.HasItemById(44615) &&
                          RotationCombatUtil.CCountAlivePartyMembers(CanGiveSpirit) > 2 &&  Settings.Current.UseAutoBuffInt,
                RotationCombatUtil.FindMe),

            // Cast Divine Spirit if it's cheaper to buff individually
            new RotationStep(new RotationSpell("Divine Spirit"), 6.3f,
                (action, partyMember) =>  Settings.Current.UseAutoBuffInt && CanGiveSpirit(partyMember),
                action => _isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank,
                RotationCombatUtil.FindExplicitPartyMember),

            // Cast Prayer of Shadow Protection if it's cheaper than buffing everyone
            new RotationStep(new RotationSpell("Prayer of Shadow Protection"), 6.4f, RotationCombatUtil.Always,
                action => (_isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank) &&
                          ItemsManager.HasItemById(44615) &&
                          RotationCombatUtil.CCountAlivePartyMembers(CanGiveShadowProtection) > 2 && Settings.Current.UseAutoBuffInt,
                RotationCombatUtil.FindMe),

            // Cast Shadow Protection if it's cheaper to buff individually
            new RotationStep(new RotationSpell("Shadow Protection"), 6.5f,
                (action, partyMember) =>  Settings.Current.UseAutoBuffInt && CanGiveShadowProtection(partyMember),
                action => _isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank,
                RotationCombatUtil.FindExplicitPartyMember),

            /*
             * De-Buff section
             */

            // Cast Mass Dispel if we find an AoE position which is cheaper than dispelling individually
            new RotationStep(new RotationSpell("Mass Dispel"), 7.0f,
                RotationCombatUtil.Always,
                action => Settings.Current.HolyDeDeBuff &&
                          (_isSpirit || _ignoreManaManagementOoc || _shouldCastOffTank) &&
                          RotationFramework.PartyMembers.Count(partyMember =>
                              partyMember.HasDebuffType("Magic")) >= 3,
                predicate => RotationCombatUtil.GetBestAoETarget(predicate, 30, 15,
                    RotationFramework.PartyMembers.Where(partyMember => partyMember.HasDebuffType("Magic")), 3)),

            // Dispel Magic from me
            new RotationStep(new RotationSpell("Dispel Magic"), 7.1f,
                (action, me) => me.HasDebuffType("Magic"),
                action => Settings.Current.HolyDeDeBuff && (_ignoreManaManagementOoc || Me.CManaPercentage() > 30),
                RotationCombatUtil.FindMe),

            // Remove Disease from me
            new RotationStep(_diseaseSpell, 7.2f,
                (action, me) => !Me.CHaveBuff("Abolish Disease") && me.HasDebuffType("Disease"),
                action => Settings.Current.HolyDeDeBuff && (_ignoreManaManagementOoc || Me.CManaPercentage() > 30),
                RotationCombatUtil.FindMe),

            // Dispel Magic from the tank
            new RotationStep(new RotationSpell("Dispel Magic"), 7.3f,
                (action, tank) => tank.HasDebuffType("Magic"),
                action => Settings.Current.HolyDeDeBuff &&
                          (_isSpirit || _ignoreManaManagementOoc || Me.CManaPercentage() > 40), FindTank),

            // Remove Disease from tank
            new RotationStep(_diseaseSpell, 7.4f,
                (action, tank) => !tank.CHaveBuff("Abolish Disease") && tank.HasDebuffType("Disease"),
                action => Settings.Current.HolyDeDeBuff &&
                          (_isSpirit || _ignoreManaManagementOoc || Me.CManaPercentage() > 40), FindTank),

            // Dispel Magic from party members
            new RotationStep(new RotationSpell("Dispel Magic"), 7.5f,
                (action, partyMember) => partyMember.HasDebuffType("Magic"),
                action => Settings.Current.HolyDeDeBuff && (_isSpirit || _ignoreManaManagementOoc ||
                                                            _shouldCastOffTank && Me.CManaPercentage() > 40),
                RotationCombatUtil.FindExplicitPartyMember),

            // Remove Disease from party members
            new RotationStep(_diseaseSpell, 7.6f,
                (action, partyMember) =>
                    !partyMember.CHaveBuff("Abolish Disease") && partyMember.HasDebuffType("Disease"),
                action => Settings.Current.HolyDeDeBuff && (_isSpirit || _ignoreManaManagementOoc ||
                                                            _shouldCastOffTank && Me.CManaPercentage() > 40),
                RotationCombatUtil.FindExplicitPartyMember)
        };

        private static WoWUnit FindTank(Func<WoWUnit, bool> predicate) =>
            _tank != null && predicate(_tank) ? _tank : null;

        private static bool DoPreCalculations() {
            Reset();
            for (var i = 0; i < RotationFramework.Enemies.Length; i++) {
                WoWUnit enemy = RotationFramework.Enemies[i];
                if (enemy.IsTargetingMe) EnemiesTargetingMe.AddLast(enemy);
                if (enemy.IsCast && enemy.IsTargetingMeOrMyPetOrPartyMember) CastingOnMeOrGroup.AddLast(enemy);
            }

            _tank = FindExplicitPartyMemberByName(Settings.Current.HolyCustomTank) ??
                    RotationCombatUtil.FindTank(unit => true);
            _shouldCastOffTank = _tank == null || _tank.IsDead || TraceLine.TraceLineGo(_tank.Position) ||
                                 Me.CManaPercentage() > Settings.Current.HolyOffTankCastingMana;
            _groupInCombat = RotationFramework.PartyMembers.Any(partyMember => partyMember.InCombat);
            _ignoreManaManagementOoc = !_groupInCombat && Settings.Current.HolyIgnoreManaManagementOOC;
            _isSpirit = Me.CHaveBuff("Spirit of Redemption");
            _castDivineHymn = ShouldCastDivineHymn();
            return false;
        }

        /*private static bool HandleAbort()
        {
            if (!Me.IsCast) return false;
            if (Me.CastingSpell.Name.Equals(_healSpell.Name) && // Heal
                Me.TargetObject?.HealthPercent > Settings.Current.HolyBigSingleTargetHeal + 10
                || // Prayer of Healing
                Me.CastingSpell.Name.Equals("Prayer of Healing") && !ShouldCastPrayerOfHealing())
                RotationCombatUtil.StopCasting();
            return true;
        }*/

        private static bool HandleAbort() => CancelableSpell.Check();

        private static bool ShouldCastPrayerOfHealing() {
            return RotationCombatUtil.CCountAlivePartyMembers(partyMember =>
                partyMember.HealthPercent < Settings.Current.HolyPrayerOfHealingTresh &&
                partyMember.GetDistance < 36) > 2;
        }

        private static void Reset() {
            Cache.Reset();
            CastingOnMeOrGroup.Clear();
            EnemiesTargetingMe.Clear();
        }

        private static WoWUnit FindExplicitPartyMemberByName(string name) =>
            RotationFramework.PartyMembers.FirstOrDefault(partyMember =>
                partyMember.Name.ToLower().Equals(name.ToLower()));

        private static bool DoNop() => Me.IsResting();

        private static bool anyoneCastingFearSpellOnMe(IEnumerable<WoWUnit> castingUnits) =>
            castingUnits.Any(enemy => enemy.IsTargetingMe &&
                                      SpecialSpells.FearInducingWithCast.Contains(enemy.CastingSpell.Name));

        private static bool anyoneCastingFearSpellOnUnit(WoWObject unit, IEnumerable<WoWUnit> castingUnits) =>
            castingUnits.Any(enemy =>
                enemy.Target == unit.Guid && SpecialSpells.FearInducingWithCast.Contains(enemy.CastingSpell.Name));

        private static bool CanGiveFortitude(WoWUnit unit) =>
            !unit.CHaveBuff("Power Word: Fortitude")
            && !unit.CHaveBuff("Prayer of Fortitude")
            && !unit.CHaveBuff("Holy Word: Fortitude");

        private static bool CanGiveSpirit(WoWUnit unit) =>
            !unit.CHaveBuff("Divine Spirit")
            && !unit.CHaveBuff("Prayer of Spirit");

        private static bool CanGiveShadowProtection(WoWUnit unit) =>
            !unit.CHaveBuff("Shadow Protection")
            && !unit.CHaveBuff("Prayer of Shadow Protection");

        private static bool CanShackleNew() => EnemiesTargetingMe.Any(enemy =>
            enemy.GetDistance > enemy.CombatReach && enemy.HaveMyBuff("Shackle Undead"));

        private static bool ShouldCastDivineHymn() =>
            RotationCombatUtil.CCountAlivePartyMembers(partyMember =>
                partyMember.GetDistance < 48 &&
                partyMember.HealthPercent < Settings.Current.HolyDivineHymnTresh) > 2;

        private static CancelableSpell FindCorrectSlowHealSpell() {
            if (SpellManager.KnowSpell("Greater Heal"))
                return new CancelableSpell("Greater Heal",
                    unit => unit.HealthPercent > Settings.Current.HolyBigSingleTargetHeal + 15);
            return SpellManager.KnowSpell("Heal")
                ? new CancelableSpell("Heal",
                    unit => unit.HealthPercent > Settings.Current.HolyBigSingleTargetHeal + 15)
                : new CancelableSpell("Lesser Heal",
                    unit => unit.HealthPercent > Settings.Current.HolyBigSingleTargetHeal + 15);
        }

        private static CancelableSpell FindCorrectFastHealSpell() =>
            SpellManager.KnowSpell("Flash Heal")
                ? new CancelableSpell("Flash Heal", unit => unit.HealthPercent > 90)
                : new CancelableSpell("Lesser Heal", unit => unit.HealthPercent > 90);

        private static RotationSpell FindCorrectRemoveDiseaseSpell() =>
            SpellManager.KnowSpell("Abolish Disease")
                ? new RotationSpell("Abolish Disease")
                : new RotationSpell("Cure Disease");
    }
}