﻿using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;

    internal class GroupHolyHeal : HealerRotation
    {
        public GroupHolyHeal() : base(useCombatSynthetics: Settings.Current.UseSyntheticCombatEvents)
        {
        }
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            //Pre Calculations
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f, (action, me) => DoPreCalculations(), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Divine Plea"), 3f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Hand of Freedom"), 4f, (s, t) => Me.Rooted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Lay on Hands"), 4.1f, (s,t) => Settings.Current.HolyLoH && t.HealthPercent < Settings.Current.HolyLoHTresh && t.InCombat, GetTank),
            new RotationStep(new RotationSpell("Purify"), 5f, (s,t) => Me.IsInGroup && (t.HasDebuffType("Disease") || t.HasDebuffType("Poison")) && Settings.Current.HolyPurify, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Beacon of Light"), 6f, (s,t) => Me.IsInGroup && t.InCombat && !t.HaveMyBuff("Beacon of Light"), GetTank),
            new RotationStep(new RotationSpell("Sacred Shield"), 7f, (s,t) => Me.IsInGroup && t.HealthPercent <= 99 && !t.HaveMyBuff("Sacred Shield"), GetTank),
            new RotationStep(new RotationSpell("Holy Shock"), 8f, (s,t) => Me.IsInGroup && t.HealthPercent <= Settings.Current.HolyHS, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Holy Light"), 9f, (s,t) => Me.IsInGroup && t.HealthPercent <= Settings.Current.HolyHL, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Holy Light"), 9.1f, (s,t) => Me.IsInGroup && t.HealthPercent <= Settings.Current.HolyHL, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Flash of Light"), 10f, (s,t) => Me.IsInGroup && t.HealthPercent <= Settings.Current.HolyFL, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell("Flash of Light"), 10.1f, (s,t) => Me.IsInGroup && t.HealthPercent <= Settings.Current.HolyFL, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell("Judgement of Light"), 11f, (s,t) => Me.IsInGroup && !t.HaveMyBuff("Judgement of Light"), RotationCombatUtil.BotTarget)
        };


    }
}