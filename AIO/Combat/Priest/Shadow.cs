using System;
using System.Collections.Generic;
using System.Linq;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using robotManager.Helpful;
using wManager.Wow;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using wManager.Wow.Patchables;
using static AIO.Constants;
using Math = System.Math;

namespace AIO.Combat.Priest {
    using Settings = PriestLevelSettings;

    internal class Shadow : BaseRotation {
        private static readonly HashSet<ulong> PartyGuids = new HashSet<ulong>();
        private static CancelableSpell _healSpell;
        private static WoWUnit _tank;
        private static WoWUnit _target;
        private static bool _haveShadowWeaving;
        private static bool _targetAttackable;

        public Shadow() : base(completelySynthetic: Settings.Current.CompletelySynthetic) {
            _healSpell = FindCorrectHealSpell();
            _haveShadowWeaving = TalentsManager.HaveTalent(3, 12);

            if (Me.Level < 20) {
                Rotation.Add(new RotationStep(new RotationSpell("Smite"), 18.1f, (s, t) => true,
                    CQuickBotTarget, checkLoS: true));
            }
            
        }

        protected sealed override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations", ignoresGlobal: true), 0f,
                (action, unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            
            new RotationStep(new RotationSpell("Power Word: Shield"), 1f,
                (s, t) => (Settings.Current.ShadowUseHeaInGrp || !Me.CIsInGroup()) &&
                          Me.CHealthPercent() <= Settings.Current.ShadowUseShieldTresh &&
                          !Me.CHaveBuff("Power Word: Shield") && !Me.CHaveBuff("Weakened Soul"),
                RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            
            // new RotationStep(new RotationSpell("Fade"), 1.1f, RotationCombatUtil.Always,
            //     action => Me.CIsInGroup() && Me.GetCachedThreatSituation() > 1, RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            
            new RotationStep(_healSpell, 2f,
                (s, t) => (Settings.Current.ShadowUseHeaInGrp || !Me.CIsInGroup()) &&
                          Me.CHealthPercent() < Settings.Current.ShadowUseHealTresh, RotationCombatUtil.FindMe, checkRange: false),
            
            new RotationStep(new CancelableSpell("Flash Heal", unit => unit.CHealthPercent() > Settings.Current.ShadowUseFlashTresh + 10), 3f,
                (s, t) => (Settings.Current.ShadowUseHeaInGrp || !Me.CIsInGroup()) &&
                          Me.CHealthPercent() < Settings.Current.ShadowUseFlashTresh ||
                          Me.CHealthPercent() < Math.Min(Settings.Current.ShadowUseFlashTresh + 25, 99) && !Me.CHaveBuff("Shadowform"),
                RotationCombatUtil.FindMe, checkRange: false),
            
            new RotationStep(new RotationSpell("Renew"), 4f,
                (s, t) => (Settings.Current.ShadowUseHeaInGrp || !Me.CIsInGroup()) && 
                          !Me.CHaveBuff("Shadowform") && Me.CHealthPercent() < Settings.Current.ShadowUseRenewTresh && 
                          Me.CManaPercentage() > 40 && t.CBuffTimeLeft("Renew") < 1000, RotationCombatUtil.FindMe, checkRange: false),
            
            new RotationStep(new RotationSpell("Psychic Scream"), 5f,
                (s, t) => !Me.CIsInGroup() && Me.CHealthPercent() < 80 &&
                          RotationFramework.Enemies.Count(o => o.Target == Me.Guid && o.CGetDistance() <= 6) >= 2,
                RotationCombatUtil.FindMe, checkRange: false),
            
            new RotationStep(new RotationSpell("Power Word: Shield"), 6f,
                (s, t) => (Settings.Current.ShadowUseHeaInGrp || !Me.CIsInGroup()) && 
                          Settings.Current.ShadowUseShieldParty && t.CHealthPercent() < Settings.Current.ShadowUseShieldTresh &&
                          t.GetCachedThreatSituation() > 1 && !t.CHaveBuff("Power Word: Shield") &&
                          !t.CHaveBuff("Weakened Soul"), pred => RotationFramework.PartyMembers.CFindInRange(unit => unit.CIsAlive() && pred(unit), 40, 5), checkLoS: true, checkRange: false),
            
            // Cast Shadowfiend on tank's combat partner to gain some mana
            new RotationStep(new RotationSpell("Shadowfiend"), 7f,
                (action, target) => target.IsEnemy() && target.IsAttackable && target.Target == _tank.Guid,
                action => _tank != null && Me.CManaPercentage() < Settings.Current.ShadowShadowfiend + 10 && _tank.CInCombat(),
                predicate => predicate(_tank?.TargetObject) ? _tank?.TargetObject : null, checkLoS: true),
            
            // Cast Shadowfiend on high HP combat partner to gain some Mana
            new RotationStep(new RotationSpell("Shadowfiend"), 8f,
                (action, target) => target.IsEnemy() && target.IsAttackable,
                action => Me.CManaPercentage() < Settings.Current.ShadowShadowfiend + 10,
                RotationCombatUtil.CGetHighestHpPartyMemberTarget, checkLoS: true),
            
            // Cast Shadowfiend on basically anything targeting me to gain some Mana
            new RotationStep(new RotationSpell("Shadowfiend"), 9f,
                (action, target) => target.Target == Me.Guid && target.IsAttackable,
                action => Me.CManaPercentage() < Settings.Current.ShadowShadowfiend && Me.CInCombat(),
                predicate => RotationFramework.Enemies.FirstOrDefault(predicate), checkLoS: true),
            
            new RotationStep(new RotationSpell("Dispersion"), 10f,
                (s, t) => Me.CManaPercentage() <= Settings.Current.ShadowDispersion && !Me.CHaveBuff("Dispersion") &&
                (Pet?.CreatedBySpell ?? 0) != 34433 /*Shadowfiend*/,
                RotationCombatUtil.FindMe, checkRange: false),
            
            new RotationStep(new RotationSpell("Shadowform"), 11f, (s, t) => !Me.CHaveBuff("Shadowform"),
                RotationCombatUtil.FindMe, checkRange: false),
            
            new RotationStep(new RotationSpell("Shoot"), 12f,
                (s, t) => Settings.Current.UseWand &&
                          (t.CHealthPercent() <= Settings.Current.UseWandTresh || Me.CManaPercentage() < 5) &&
                          Extension.HaveRangedWeaponEquipped && !RotationCombatUtil.IsAutoRepeating("Shoot"), CQuickBotTarget,
                checkLoS: true),

            new RotationStep(new RotationSpell("Mind Sear"), 13f,
                (action, target) => {
                    ushort count = 0;
                    Vector3 targetPosition = target.PositionWithoutType;
                    int length = Math.Min(10, RotationFramework.Enemies.Length);
                    for (var i = 0; i < length; i++) {
                        WoWUnit enemy = RotationFramework.Enemies[i];
                        if (target.GetBaseAddress != enemy.GetBaseAddress && targetPosition.DistanceTo(enemy.PositionWithoutType) <= 11)
                            count++;
                        if (count >= 2) return true;
                    }

                    return false;
                }, action => Me.CManaPercentage() > 65 && !Me.GetMove,
                CQuickBotTarget, checkLoS: true),
            
            new RotationStep(new RotationSpell("Vampiric Touch"), 14f,
                (s, t) => t.CMyBuffTimeLeft("Vampiric Touch") < 1300, CQuickBotTarget, checkLoS: true),
            
            new RotationStep(new RotationSpell("Devouring Plague"), 15f,
                (s, t) => ((t.CHealthPercent() > 40 || t.IsBoss && t.CHealthPercent() > 15) &&
                          t.CMyBuffTimeLeft("Devouring Plague") < 2590) && Settings.Current.ShadowDPUse, CQuickBotTarget,
                checkLoS: true),
            
            new RotationStep(new RotationSpell("Mind Blast"), 16f, (s, t) => true,
                CQuickBotTarget, checkLoS: true),
            
            new RotationStep(new RotationSpell("Shadow Word: Pain"), 17f,
                (action, target) => (!_haveShadowWeaving || Me.CBuffStack("Shadow Weaving") >= 5) && target.CMyBuffTimeLeft("Shadow Word: Pain") < 2800,
            CQuickBotTarget, checkLoS: true),

            // new RotationStep(new RotationSpell("Shadow Word: Pain"), 17.1f,
            //     (s, t) => PartyGuids.Contains(t.Target) && !t.CHaveMyBuff("Shadow Word: Pain"),
            //     action => Settings.Current.ShadowDotOff, 
            //     predicate => RotationFramework.Enemies.CFindInRange(predicate, 36f, 5), checkLoS: true),
            
            new RotationStep(new RotationSpell("Mind Flay"), 18f,
                (s, t) => Settings.Current.ShadowUseMindflay && (t.CHaveMyBuff("Shadow Word: Pain") ||
                          t.CHaveMyBuff("Devouring Plague")), action => !Me.GetMove, CQuickBotTarget, checkLoS: true),

            new RotationStep(new RotationSpell("Vampiric Embrace"), 19f,
                (s, t) => t.CBuffTimeLeft("Vampiric Embrace") < 1000 * 60 * 5, RotationCombatUtil.FindMe,
                checkRange: false),
        };

        private static bool DoPreCalculations() {
            Cache.Reset();
            PartyGuids.Clear();
            foreach (WoWPlayer partyMember in RotationFramework.PartyMembers) PartyGuids.Add(partyMember.Guid);

            _tank = RotationCombatUtil.CFindTank(unit => true);

            const bool lazyTarget = false;
            
            _target = ObjectManager.Target;
            if(lazyTarget && (_tank?.IsValid ?? false) && !(_target?.IsValid ?? false) && _tank.CInCombat()) {
                WoWUnit tmpTarget = null;
                long tmpHealth = 0;
                ulong tankGuid = _tank.Guid;
                foreach (WoWUnit enemy in RotationFramework.Enemies) {
                    if (enemy.Target == tankGuid) {
                        long veryTmpHealth = enemy.Health;
                        if (veryTmpHealth > tmpHealth) {
                            tmpTarget = enemy;
                            tmpHealth = veryTmpHealth;
                        }
                    }
                }

                if (tmpTarget != null) {
                    Me.Target = tmpTarget.Guid;
                }
                
                _target = ObjectManager.Target;
            }
            
            _targetAttackable = _target?.IsAttackable ?? false;
            return false;
        }

        private static CancelableSpell FindCorrectHealSpell() {
            if (SpellManager.KnowSpell("Greater Heal"))
                return new CancelableSpell("Greater Heal",
                    unit => unit.CHealthPercent() > Settings.Current.ShadowUseHealTresh + 10);
            return SpellManager.KnowSpell("Heal")
                ? new CancelableSpell("Heal",
                    unit => unit.CHealthPercent() > Settings.Current.ShadowUseHealTresh + 10)
                : new CancelableSpell("Lesser Heal",
                    unit => unit.CHealthPercent() > Settings.Current.ShadowUseHealTresh + 10);
        }

        private static WoWUnit CQuickBotTarget(Func<WoWUnit, bool> predicate) {
            return _target != null && _targetAttackable && predicate(_target) ? _target : null;
        }
    }
}
