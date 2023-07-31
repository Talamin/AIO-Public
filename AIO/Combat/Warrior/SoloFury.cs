using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    using Settings = WarriorLevelSettings;
    internal class SoloFury : BaseRotation
    {
        private static readonly string Intercept = "Intercept";
        private readonly bool KnowIntercept = SpellManager.KnowSpell(Intercept);
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Pummel"), 2f, (s,t) => t.IsCasting(), RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell("Hamstring"), 3f, (s,t) => !t.HaveBuff("Hamstring") && t.HealthPercent < 40 && t.CreatureTypeTarget=="Humanoid" && !BossList.MyTargetIsBoss && Settings.Current.Hamstring, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Piercing Howl"), 4f, (s,t) => t.HealthPercent < 40 && RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Bloodrage"), 5f, (s,t) => t.GetDistance < 7, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Slam"), 6f, (s,t) => Me.HaveBuff("Slam!"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Bloodthirst"), 7f, (s,t) => Me.Rage > 30 && Me.HealthPercent <= 80, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Death Wish"), 8f, (s,t) => Me.Rage> 10, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Execute"), 9f, (s1,t) => t.HealthPercent < 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Victory Rush"), 10f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Rend"), 11f, (s,t) => !t.HaveMyBuff("Rend") && !t.IsCreatureType("Elemental"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Intercept"), 12f, (s,t) => Settings.Current.SoloFuryIntercept && Me.Rage > 10 && t.GetDistance > 7 && t.GetDistance <= 24, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Charge"), 13f, (s,t) => !KnowIntercept && Settings.Current.SoloFuryIntercept && t.GetDistance > 7, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Thunder Clap"), 14f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Whirlwind"), 15f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Cleave"), 16f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Heroic Strike"), 17f, (s,t) => Me.Rage > 40, RotationCombatUtil.BotTarget),
        };
    }
}
