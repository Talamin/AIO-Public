using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.DeathKnight
{
    using Settings = DeathKnightLevelSettings;
    internal class SoloUnholy : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            //new RotationStep(new RotationSpell("Raise Dead"), 2f, (s,t) => !Pet.IsAlive && Me.RunicPower > 80 , RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Mind Freeze"), 3.1f, (s,t) => t.IsCasting(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Strangulate"), 4f, (s,t) => t.IsCasting() && t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell("Death and Decay"), 5f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance < 15) >= Settings.Current.SoloUnholyDnD, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Icy Touch"), 6f, (s,t) => !t.HaveMyBuff("Frost Fever"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Plague Strike"), 7f, (s,t) => !t.HaveMyBuff("Blood Plague"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Summon Gargoyle"), 4.0f, (s,t) => BossList.MyTargetIsBoss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Pestilence"), 8f, (s,t) => t.HaveMyBuff("Blood Plague", "Frost Fever") && RotationFramework.Enemies.Count(o => o.GetDistance < 15 && !o.HaveMyBuff("Blood Plague", "Frost Fever")) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blood Strike"), 9f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) == Settings.Current.SoloUnholyBloodStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Heart Strike"), 10f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) >= Settings.Current.SoloUnholyHearthStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blood Boil"), 11f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) > Settings.Current.SoloUnholyBloodBoil, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Death Strike"), 12f, (s,t) => Me.HealthPercent < 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Scourge Strike"), 13f, (s,t) => t.HaveMyBuff("Blood Plague", "Frost Fever"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Death Coil"), 14f, (s,t) => Me.RunicPower > 80, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Blood Strike"), 15f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget)
        };
    }
}
