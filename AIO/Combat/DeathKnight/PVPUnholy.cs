using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.DeathKnight
{
    internal class PVPUnholy : BaseRotation
    {
        private static readonly LinkedList<WoWUnit> CastingOnMeOrGroup = new LinkedList<WoWUnit>();
        private static readonly LinkedList<WoWUnit> EnemiesTargetingMe = new LinkedList<WoWUnit>();
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            //Do Precalculations
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f, (action, me) => DoPreCalculations(),
                RotationCombatUtil.FindMe),
            //Section for Interrupts
            new RotationStep(new RotationSpell("Mind Freeze"), 1.0f, (s,t) => t.IsCast, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Strangulate"), 1.1f, (s,t) => t.IsCast && t.IsTargetingMe && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell("Death Grip"), 1.2f, (s,t) => t.IsCast && t.IsTargetingMe && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            //Section for Defense
            // Cast Lichborne on myself if enemy is casting a fear inducing spell on me (Interrupts)
            new RotationStep(new RotationSpell("Lichborne"), 2.0f, RotationCombatUtil.Always, action => anyoneCastingFearSpellOnMe(CastingOnMeOrGroup), RotationCombatUtil.FindMe, null, true, false),
            //Cast AntiMagicShell on Me
            new RotationStep(new RotationSpell("Anti-Magic Shell"), 2.5f, (s,t) => RotationFramework.Enemies.Count(o => o.IsCast && o.IsTargetingMe) >=1, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Anti-Magic Zone"), 2.6f, (s,t) => RotationFramework.Enemies.Count(o => o.IsCast && o.IsTargetingMe) >=1 && !RotationFramework.SpellReady(48707), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Icebound Fortitude"), 2.7f, (s,t) => Me.HealthPercent < 70, RotationCombatUtil.BotTarget),

            //Section for DPS
            //Raise Dead
            new RotationStep(new RotationSpell("Raise Dead"), 3.0f, (s,t) => !Pet.IsAlive && Me.RunicPower > 80, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Chains of Ice"), 3.1f, (s,t) => !t.HaveMyBuff("Frost Fever"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Plague Strike"), 3.2f, (s,t) => !t.HaveMyBuff("Blood Plague"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Death Strike"), 3.3f, (s,t) => Me.HealthPercent < 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Scourge Strike"), 3.4f, (s,t) => Me.HealthPercent >= 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Empower Rune Weapon"), 3.5f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            //Summon  Gargoyle for more DPS
            new RotationStep(new RotationSpell("Summon Gargoyle"), 4.0f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            //Diseases on  Target
            new RotationStep(new RotationSpell("Pestilence"), 5.0f, (s,t) => t.HaveMyBuff("Blood Plague", "Frost Fever") && RotationFramework.Enemies.Count(o => o.GetDistance <= 10 && !o.HaveMyBuff("Blood Plague", "Frost Fever")) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blood Strike"), 5.1f, (s,t) => t.HaveMyBuff("Blood Plague", "Frost Fever"), RotationCombatUtil.BotTarget),
            //Dump Runic Power
            new RotationStep(new RotationSpell("Death Coil"), 6.0f, (s,t) => Me.RunicPower > 80, RotationCombatUtil.BotTarget),
        };
        private static bool anyoneCastingFearSpellOnMe(IEnumerable<WoWUnit> castingUnits) =>
        castingUnits.Any(enemy => enemy.IsTargetingMe &&
                              SpecialSpells.FearInducingWithCast.Contains(enemy.CastingSpell.Name));
        private static bool DoPreCalculations()
        {
            Reset();
            for (var i = 0; i < RotationFramework.Enemies.Length; i++)
            {
                WoWUnit enemy = RotationFramework.Enemies[i];
                if (enemy.IsTargetingMe) EnemiesTargetingMe.AddLast(enemy);
                if (enemy.IsCast && enemy.IsTargetingMe) CastingOnMeOrGroup.AddLast(enemy);
            }
            return false;
        }
        private static void Reset()
        {
            Cache.Reset();
            CastingOnMeOrGroup.Clear();
            EnemiesTargetingMe.Clear();
        }
    }
}
