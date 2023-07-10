using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class Buffs : BaseRotation
    {
        private bool UseTranquility(IRotationAction s, WoWUnit t)
        {
            var nearbyFriendlies = RotationFramework.PartyMembers.Where(o => o.IsAlive && o.GetDistance <= 40).ToList();

            var under40 = nearbyFriendlies.Count(o => o.HealthPercent <= 40);
            var under55 = nearbyFriendlies.Count(o => o.HealthPercent <= 55);
            var under65 = nearbyFriendlies.Count(o => o.HealthPercent <= 65);

            return Me.IsInGroup && RotationFramework.PartyMembers.Count(u => u.IsAlive && u.IsTargetingMeOrMyPetOrPartyMember) >= 1 && (under40 >= 2 || under55 >= 3 || under65 >= 4);
        }

        internal Buffs() : base(runInCombat: Settings.Current.BuffIC, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Mark of the Wild"), 1f, (s,t) => !Me.IsMounted && !t.HaveBuff("Gift of the Wild"), RotationCombatUtil.FindPartyMember, ignoreMovement: true),
            new RotationStep(new RotationBuff("Mark of the Wild"), 2f, (s,t) =>!Me.IsMounted && !t.HaveBuff("Gift of the Wild"), RotationCombatUtil.FindMe, ignoreMovement: true),
            new RotationStep(new RotationBuff("Thorns"), 3f,(s,t) => !Me.IsMounted, RotationCombatUtil.FindTank),
            new RotationStep(new RotationBuff("Thorns"), 4f,(s,t) => !Me.IsInGroup && !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Tree of Life"), 5f,(s,t) => !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Tranquility"), 7f, UseTranquility, RotationCombatUtil.FindMe),
        };
    }
}
