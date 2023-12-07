using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers.Caching;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Priest
{
    using Settings = PriestLevelSettings;

    internal class GroupShadow : BaseRotation
    {
        private bool _haveShadowWeavingTalent = TalentsManager.HaveTalent(3, 12, 3);
        private List<WoWUnit> _enemiesAroundMe = new List<WoWUnit>();
        private List<WoWUnit> _enemiesWithoutMySWP = new List<WoWUnit>();
        private List<WoWUnit> _enemiesWithoutMyVT = new List<WoWUnit>();
        private Spell _mindBlastSpell = new Spell("Mind Blast");
        private bool _knowAbolishDisease = new Spell("Abolish Disease").KnownSpell;
        private bool _knowMindFlay = new Spell("Mind Flay").KnownSpell;
        private bool _mindBlastIsUsable;

        protected sealed override List<RotationStep> Rotation => new List<RotationStep>
        {
            new RotationStep(new RotationAction("PreCalculations", PreCalculations), 0f, 200),
            new RotationStep(new RotationAction("Cache dotables and debuffed", CacheDotablesAndDebuffed), 0f, 1000),
            new RotationStep(new RotationAction("Skip if Dispersion", SkipIfDispersion), 0.1f),

            // Utility
            new RotationStep(new RotationSpell("Shadowfiend"), 1f, (s,t) => Me.CManaPercentage() <= Settings.Current.GroupShadowShadowfiend && t.IsElite && t.CHealthPercent() > 50, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Dispersion"), 2f, (s, t) => Me.CManaPercentage() <= Settings.Current.GroupShadowDispersion && !Pet.IsAlive && !Me.CHaveBuff("Dispersion"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Power Word: Shield"), 3f, (s, t) => Me.CHealthPercent() <= Settings.Current.GroupShadowUseShieldTresh && !Me.CHaveBuff("Weakened Soul"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Inner Focus"), 4f, (s,t) => BossList.MyTargetIsBoss && _mindBlastIsUsable, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Mind Blast"), 5f, (s,t) => Me.CHaveBuff("Inner Focus"), RotationCombatUtil.BotTarget),

            // AOE Rotation
            new RotationStep(new RotationSpell("Shadow Word: Pain"), 6f, (s,t) => Settings.Current.GroupShadowSpreadSWPain && _enemiesAroundMe.Count >= 2, _enemiesWithoutMySWP.FirstOrDefault),
            new RotationStep(new RotationSpell("Vampiric Touch"), 7f, (s,t) => Settings.Current.GroupShadowSpreadVT && _enemiesAroundMe.Count >= 2, _enemiesWithoutMyVT.FirstOrDefault, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Mind Sear"), 8f, (s,t) => RotationFramework.Enemies.Count(e => e.Guid != t.Guid && e.Position.DistanceTo(t.Position) < 10) >= 2, RotationCombatUtil.BotTargetFast),

            // Single target Rotation
            new RotationStep(new RotationSpell("Shadow Word: Pain"), 9f, (s,t) => !_haveShadowWeavingTalent && !t.CHaveMyBuff("Shadow Word: Pain"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shadow Word: Pain"), 10f, (s,t) => _haveShadowWeavingTalent && !t.CHaveMyBuff("Shadow Word: Pain") && Me.CBuffStack("Shadow Weaving") >= 5, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Devouring Plague"), 11f, (s,t) => !t.CHaveBuff("Devouring Plague"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Shadow Word: Death"), 12f, (s,t) => Me.GetMove, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell("Vampiric Touch"), 12.5f, (s,t) => !t.CHaveBuff("Vampiric Touch"), RotationCombatUtil.BotTargetFast, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Mind Blast"), 13f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),

            // Cure Debuffs
            new RotationStep(new RotationSpell("Cure Disease"), 14f, (s,t) => Settings.Current.GroupShadowCureDisease && !_knowAbolishDisease, p => RotationCombatUtil.GetPartyMemberWithCachedDebuff(p, DebuffType.Disease, true, 30)),
            new RotationStep(new RotationSpell("Abolish Disease"), 15f, (s,t) => Settings.Current.GroupShadowCureDisease, p => RotationCombatUtil.GetPartyMembersWithCachedDebuff(p, DebuffType.Disease, true, 30).FirstOrDefault(t => !t.CHaveMyBuff("Abolish Disease"))),
            new RotationStep(new RotationSpell("Dispel Magic"), 16f, (s,t) => Settings.Current.GroupShadowDispelMagic, p => RotationCombatUtil.GetPartyMemberWithCachedDebuff(p, DebuffType.Magic, true, 30)),

            // Fillers
            new RotationStep(new RotationSpell("Mind Flay"), 17f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Smite"), 18f, (s,t) => !_knowMindFlay, RotationCombatUtil.BotTarget),

            new RotationStep(new RotationSpell("Shoot"), 25f, (s,t) => Me.CManaPercentage() < 5 && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTargetFast, checkLoS: true),
        };

        private bool SkipIfDispersion()
        {
            if (Me.CHaveBuff("Dispersion"))
                return true;

            return false;
        }

        private bool CacheDotablesAndDebuffed()
        {
            Cache.Reset();
            RotationCombatUtil.CacheLUADebuffedPartyMembersStep();
            _enemiesWithoutMySWP.Clear();
            _enemiesWithoutMyVT.Clear();
            foreach (WoWUnit unit in _enemiesAroundMe)
            {
                if (Settings.Current.GroupShadowSpreadSWPain
                    && !unit.CHaveMyBuff("Shadow Word: Pain")
                    && unit.IsElite
                    && unit.Guid != Target.Guid
                    && !TraceLine.TraceLineGo(unit.PositionWithoutType))
                    _enemiesWithoutMySWP.Add(unit);
                if (Settings.Current.GroupShadowSpreadVT
                    && unit.IsElite
                    && !unit.CHaveMyBuff("Vampiric Touch")
                    && !TraceLine.TraceLineGo(unit.PositionWithoutType))
                    _enemiesWithoutMyVT.Add(unit);
            }
            return false;
        }

        private bool PreCalculations()
        {
            Cache.Reset();
            _mindBlastIsUsable = _mindBlastSpell.IsSpellUsable;
            _enemiesAroundMe = RotationFramework.Enemies
                .Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember() && unit.CGetDistance() < 30)
                .ToList();
            if (Pet.IsAlive)
                Lua.LuaDoString("PetAttack('target')");

            return false;
        }
    }
}
