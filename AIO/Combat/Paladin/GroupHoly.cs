using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;

    internal class GroupHoly : HealerRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            //Pre Calculations
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f, (action, me) => DoPreCalculations(), RotationCombatUtil.FindMe),
            new RotationStep(new RotationAction("Cache debuffed party members", RotationCombatUtil.CacheLUADebuffedPartyMembersStep), 0f, 1000),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Divine Plea"), 3f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hand of Freedom"), 4f, (s, t) => Me.Rooted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Lay on Hands"), 4.1f, (s,t) => Settings.Current.GroupHolyLoH && t.HealthPercent < Settings.Current.GroupHolyLoHTresh && t.InCombat, GetTank),
            new RotationStep(new RotationBuff("Beacon of Light"), 6f, (s,t) => t.InCombat && !t.HaveMyBuff("Beacon of Light"), RotationCombatUtil.FindTank),
            new RotationStep(new RotationSpell("Sacred Shield"), 7f, (s,t) => t.HealthPercent <= 99 && !t.HaveMyBuff("Sacred Shield"), GetTank),
            new RotationStep(new RotationSpell("Holy Shock"), 8f, (s,t) => t.HealthPercent <= Settings.Current.GroupHolyHS, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Holy Light"), 9f, (s,t) => t.HealthPercent <= Settings.Current.GroupHolyHL, RotationCombatUtil.FindPartyMember, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Holy Light"), 9.1f, (s,t) => t.HealthPercent <= Settings.Current.GroupHolyHL, RotationCombatUtil.FindMe, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Flash of Light"), 10f, (s,t) => t.HealthPercent <= Settings.Current.GroupHolyFL, RotationCombatUtil.FindPartyMember, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Flash of Light"), 10.1f, (s,t) => t.HealthPercent <= Settings.Current.GroupHolyFL, RotationCombatUtil.FindMe, preventDoubleCast: true),
            new RotationStep(new RotationSpell("Judgement of Light"), 11f, (s,t) => !t.HaveMyBuff("Judgement of Light"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Purify"), 12f, (s,t) => Settings.Current.GroupHolyPurify, p => RotationCombatUtil.GetPartyMemberWithCachedDebuff(p, new List<DebuffType>() { DebuffType.Disease, DebuffType.Poison } , true, 30)),
        };
    }
}