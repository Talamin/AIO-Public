using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Warlock
{
    using Settings = WarlockLevelSettings;
    internal class Affliction : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell("Shoot"), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating("Shoot"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Drain Soul"), 2.5f, (s,t) => t.HealthPercent <= 25 && ItemsHelper.GetItemCount("Soul Shard") <= 3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Demonic Empowerment"), 3f, (s,t) => !Pet.HaveBuff("Demonic Empowerment") && Pet.IsAlive && Pet.IsMyPet, RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell("Life Tap"), 4f, (s,t) => Me.HealthPercent > 50 && Me.ManaPercentage < Settings.Current.Lifetap,RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Seed of Corruption"), 4.4f, (s,t) => 
            Settings.Current.UseSeedGroup && Me.IsInGroup &&  !t.HaveMyBuff("Seed of Corruption") && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.AOEInstance && Settings.Current.UseAOE, RotationCombatUtil.FindEnemyAttackingGroupAndMe),
            new RotationStep(new RotationSpell("Corruption"), 8f, (s,t) => 
            Settings.Current.UseCorruptionGroup && Me.IsInGroup &&  !t.HaveMyBuff("Corruption") && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.AOEInstance && Settings.Current.UseAOE, RotationCombatUtil.FindEnemyAttackingGroupAndMe),
            new RotationStep(new RotationSpell("Rain of Fire"), 4.5f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.AOEInstance && Settings.Current.UseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 5f, (s,t) => Me.HaveBuff("Shadow Trance"),RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Health Funnel"), 6f, (s,t) => !Pet.HaveBuff("Health Funnel") && Pet.HealthPercent < Settings.Current.HealthfunnelPet && Me.HealthPercent > Settings.Current.HealthfunnelMe && Pet.IsAlive && Pet.IsMyPet, RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell("Haunt"), 7.5f, (s,t) => !t.HaveMyBuff("Haunt"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Corruption"), 8.1f, (s,t) => !t.HaveMyBuff("Corruption") && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.AOEOutsideInstance && Settings.Current.UseAOEOutside, RotationCombatUtil.FindEnemyAttackingGroupAndMe),
            //Curses
            new RotationStep(new RotationSpell("Curse of Agony"), 10f, (s,t) => !t.HaveMyBuff("Curse of Agony") && Settings.Current.AfflCurse == "Agony", RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of Doom"), 10.1f, (s,t) => !t.HaveMyBuff("Curse of Doom") && Settings.Current.AfflCurse == "Doom", RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of the Elements"), 10.2f, (s,t) => !t.HaveMyBuff("Curse of the Elements") && Settings.Current.AfflCurse == "Elements", RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of Tongues"), 10.3f, (s,t) => !t.HaveMyBuff("Curse of Tongues") && Settings.Current.AfflCurse == "Tongues", RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of Weakness"), 10.4f, (s,t) => !t.HaveMyBuff("Curse of Weakness") && Settings.Current.AfflCurse == "Weakness", RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Curse of Exhaustion"), 10.4f, (s,t) => !t.HaveMyBuff("Curse of Exhaustion") && Settings.Current.AfflCurse == "Exhaustion", RotationCombatUtil.BotTarget),
            //
            new RotationStep(new RotationSpell("Corruption"), 11f, (s,t) => !t.HaveMyBuff("Corruption"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Drain Life"), 12f, (s,t) => !Me.IsInGroup && Me.HealthPercent < Settings.Current.Drainlife, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Unstable Affliction"), 13f, (s,t) => !t.HaveMyBuff("Unstable Affliction"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Immolate"), 14f, (s,t) => !t.HaveMyBuff("Immolate") && !SpellManager.KnowSpell("Unstable Affliction"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 16f ,(s,t) => t.HealthPercent > Settings.Current.UseWandTresh && !Settings.Current.ShadowboltWand, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Shadow Bolt"), 17f ,(s,t) => Settings.Current.ShadowboltWand, RotationCombatUtil.BotTarget)
        };
    }
}
