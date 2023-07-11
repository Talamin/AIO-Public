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
        internal Buffs() : base(runInCombat: Settings.Current.BuffIC, runOutsideCombat: true) { }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Mark of the Wild"), 1f, (s,t) => !Me.IsMounted && !t.HaveBuff("Gift of the Wild"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Mark of the Wild"), 2f, (s,t) =>!Me.IsMounted && !t.HaveBuff("Gift of the Wild"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Thorns"), 3f,(s,t) => !Me.IsMounted && !t.HaveBuff("Thorns"), RotationCombatUtil.FindTank),
            new RotationStep(new RotationBuff("Thorns"), 4f,(s,t) => !Me.IsInGroup && !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Tree of Life"), 5f,(s,t) => !Me.IsMounted, RotationCombatUtil.FindMe),
        };
    }
}
