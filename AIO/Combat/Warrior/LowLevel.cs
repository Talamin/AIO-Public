using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Warrior
{
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !ObjectManager.Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Intercept"), 2f, (s,t) => t.GetDistance > 7, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Charge"), 3f, (s,t) => t.GetDistance > 8, RotationCombatUtil.BotTarget, forcedTimerMS: 1000),
            new RotationStep(new RotationSpell("Rend"), 4f, (s,t) => !t.HaveMyBuff("Rend") && !t.IsCreatureType("Elemental"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Victory Rush"), 5f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Thunder Clap"), 6f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Heroic Strike"), 7f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
        };
    }
}