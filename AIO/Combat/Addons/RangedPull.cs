using AIO.Framework;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using wManager.Events;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using Timer = robotManager.Helpful.Timer;

namespace AIO.Combat.Addons
{
    internal class RangedPull : IAddon
    {
        private Timer _timeout = new Timer(6 * 1000);
        private PullCondition _pullCondition;
        private Action SetDefaultRange;
        private Action<float> SetRange;
        private bool _pullSuccesful;
        private List<RotationSpell> _knownPullSpells = new List<RotationSpell>();
        private readonly RotationSpell _throwSpell = new RotationSpell("Throw");
        private readonly RotationSpell _shootSpell = new RotationSpell("Shoot");

        public bool RunOutsideCombat => false;
        public bool RunInCombat => true;
        public List<RotationStep> Rotation => new List<RotationStep>()
        {
            new RotationStep(new RotationAction("Pull", Pull), -1f)
        };

        public enum PullCondition
        {
            ALWAYS,
            ENEMIES_AROUND
        }

        private List<RotationSpell> _allPullSpells = new List<RotationSpell>()
        {
            new RotationSpell("Avenger's Shield"),
            new RotationSpell("Exorcism"),
            new RotationSpell("Hand of Reckoning"),
            new RotationSpell("Faerie Fire (Feral)")
        };

        public RangedPull(Action setDefaultRange, Action<float> setRange, PullCondition pullCondition)
        {
            SetDefaultRange = setDefaultRange;
            SetRange = setRange;
            _pullCondition = pullCondition;
            _timeout.ForceReady();
        }

        public void Initialize()
        {
            FightEvents.OnFightStart += OnFightStart;
            FightEvents.OnFightEnd += OnFightEnd;
        }

        public void Dispose()
        {
            FightEvents.OnFightStart -= OnFightStart;
            FightEvents.OnFightEnd += OnFightEnd;
        }

        private void OnFightEnd(ulong guid)
        {
            _pullSuccesful = false;
            SetDefaultRange();
        }

        private void OnFightStart(WoWUnit unit, CancelEventArgs cancelable)
        {
            _knownPullSpells = _allPullSpells.Where(spell => spell.KnownSpell).ToList();
            _pullSuccesful = false;
            _timeout.Reset();

            if (_throwSpell.KnownSpell
                && EquippedItems.GetEquippedItems().Exists(item => item.GetItemInfo.ItemSubType == "Thrown"))
                _knownPullSpells.Add(_throwSpell);

            if (_shootSpell.KnownSpell
                && EquippedItems.GetEquippedItems().Exists(item => item.GetItemInfo.ItemSubType == "Bows" || item.GetItemInfo.ItemSubType == "Guns" || item.GetItemInfo.ItemSubType == "Crossbows"))
                _knownPullSpells.Add(_shootSpell);
        }

        private bool Pull()
        {
            if (_pullSuccesful || !Me.HasTarget)
            {
                SetDefaultRange();
                return false;
            }

            WoWUnit target = new WoWUnit(Target.GetBaseAddress);

            if (_timeout.IsReady
                || target.IsCast
                || target.HasTarget && target.Target != Me.Guid && target.IsTargetingMeOrMyPetOrPartyMember
                || RotationFramework.PartyMembers.Any(p => p.Guid != Me.Guid && RotationFramework.Enemies.Any(e => e.Position.DistanceTo(p.Position) < 13)))
            {
                _pullSuccesful = true;
                SetDefaultRange();
                return false;
            }

            // Pull done, wait for the enemy to come (or timeout)
            if (target.Target == Me.Guid)
            {
                return target.GetDistance > 8;
            }

            RotationSpell pullSpell = _knownPullSpells.FirstOrDefault(spell => spell.IsSpellUsable);
            // No pull spell available
            if (pullSpell == null)
            {
                SetDefaultRange();
                return false;
            }

            // Check pull condition
            bool pullCOnditionMet = _pullCondition == PullCondition.ALWAYS
                || _pullCondition == PullCondition.ENEMIES_AROUND && HasNearbyEnemies(target, 25f);
            if (!pullCOnditionMet)
            {
                SetDefaultRange();
                return false;
            }

            bool inRealRange = Lua.LuaDoString<bool>($@"
                local inRange = 0;
                if UnitExists('target') and UnitIsVisible('target') then
                   inRange = IsSpellInRange(""{pullSpell.Name}"", 'target');
                end
                return inRange == 1;
            ");
            bool inLoS = !TraceLine.TraceLineGo(target.Position);

            if (target.GetDistance > 40)
                _timeout.Reset();

            if (target != null
                && inRealRange
                && inLoS)
            {
                SetRange(50);
                MovementManager.StopMove();
                RotationCombatUtil.CastSpell(pullSpell, target, true);
                Thread.Sleep(500);
                if (!pullSpell.IsSpellUsable)
                {
                    Timer timer = new Timer(2000);
                    while (!target.InCombat
                        && !target.IsCast
                        && !timer.IsReady
                        && Conditions.InGameAndConnectedAndAlive)
                    {
                        Thread.Sleep(100);
                    }
                }
                return true;

            }

            return false;
        }

        public static bool HasNearbyEnemies(WoWUnit target, float distance)
        {
            WoWUnit[] surroundingEnemies = RotationFramework.Enemies.Where(u =>
                !u.IsTapDenied &&
                !u.IsTaggedByOther &&
                !u.PlayerControlled &&
                u.IsAttackable &&
                u.Guid != target.Guid).ToArray();

            foreach (WoWUnit unit in surroundingEnemies)
            {
                if (unit.Position.DistanceTo(target.Position) < distance)
                    return true;
            }

            return false;
        }
    }
}
