using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Priest
{
    using Settings = PriestLevelSettings;
    internal class GroupDiscipline : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        private List<WoWPlayer> _hurtPartyMembers = new List<WoWPlayer>(0);

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations", ignoresGlobal: true), 0.0f,(action, unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell("Power Word: Shield"), 2f, (action,tank)  => !tank.CHaveBuff("Power Word: Shield") && tank.InCombat, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell("Power Word: Shield"), 2.1f, (action,me)  => !me.CHaveBuff("Power Word: Shield") && me.CHealthPercent() < 100, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell("Power Word: Shield"), 2.2f, (s,t) => t.CHaveBuff("Power Word: Shield") && t.CHealthPercent() <= 80, RotationCombatUtil.FindPartyMember, checkLoS: true),
            //Heal Over Time
            new RotationStep(new RotationSpell("Renew"), 3f, (action,tank)  => !tank.CHaveMyBuff("Renew") && tank.CHealthPercent() <= 99, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell("Renew"), 3.1f, (action,me)  => !me.CHaveMyBuff("Renew") && me.CHealthPercent() <= 99, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell("Renew"), 3.2f, (s,t) => t.CHaveMyBuff("Renew") && t.CHealthPercent() <= 99, RotationCombatUtil.FindPartyMember, checkLoS: true),
            //Prayer of Mending
            new RotationStep(new RotationSpell("Prayer of Mending"), 3.5f, (action,tank) => 
            tank.CHealthPercent() <= 80 && tank.CHaveMyBuff("Prayer of Mending"), RotationCombatUtil.FindTank, checkLoS: true),
            //Oh Shit Heals
            new RotationStep(new RotationSpell("Inner Focus"), 3.5f, (s, t) => _hurtPartyMembers.ContainsAtLeast(p=> p.CHealthPercent() <= 60, 1),RotationCombatUtil.FindMe, checkLoS: false), 
            new RotationStep(new RotationSpell("Divine Hymn"), 3.6f, (s,t) => Me.CHaveBuff("Inner Focus") && _hurtPartyMembers.ContainsAtLeast(p=> p.CHealthPercent() <= 60, 2), RotationCombatUtil.FindMe, checkLoS: false),
            new RotationStep(new RotationSpell("Penance"), 4f, (action, tank)  => tank.CHealthPercent() <= 60, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell("Penance"), 4.1f, (s, t)  => t.CHealthPercent() <= 60, FindLowestPartyMember, checkLoS: true),            
            //Heals
            new RotationStep(new RotationSpell("Binding Heal"), 5f, (action, tank)  => tank.CHealthPercent() <= 80 && Me.CHealthPercent() <= 90, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell("Binding Heal"), 5.1f, (s, t)  => t.CHealthPercent() <= 80 && Me.CHealthPercent() <= 90, RotationCombatUtil.FindPartyMember, checkLoS: true),
            new RotationStep(new RotationSpell("Flash Heal"), 6f, (action, tank)  => tank.CHealthPercent() <= 80, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell("Flash Heal"), 6.1f, (s, t)  => t.CHealthPercent() <= 80, RotationCombatUtil.FindPartyMember, checkLoS: true),

        };

        private bool DoPreCalculations()
        {
            if (LimitExecutionSpeed(100))
            {
                return true;
            }
            Cache.Reset();
            EnemiesAttackingGroup = RotationFramework.Enemies.Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember())
                .ToArray();
            ClearLists();
            BuildLists();
            return false;
        }

        private void BuildLists()
        {
            for (int i = 0; i < RotationFramework.PartyMembers.Count(); i++)
            {
                WoWPlayer Partymember = RotationFramework.PartyMembers[i];
                if (Partymember.CHealthPercent() < 99)
                {
                    _hurtPartyMembers.Add(Partymember);
                }
            }
        }

        //clear prebuilded Lists
        private void ClearLists()
        {
            _hurtPartyMembers.Clear();
        }
        private bool LimitExecutionSpeed(int delay)
        {
            if (watch.ElapsedMilliseconds > delay)
            {
                watch.Restart();
                return false;
            }
            return true;
        }

        public WoWUnit FindEnemyAttackingGroup(Func<WoWUnit, bool> predicate) => EnemiesAttackingGroup.FirstOrDefault(predicate);

        public WoWUnit FindLowestPartyMember(Func<WoWUnit, bool> predicate) => _hurtPartyMembers.FirstOrDefault(predicate);
    }
}
