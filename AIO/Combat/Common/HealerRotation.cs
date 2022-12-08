using AIO.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wManager.Wow.ObjectManager;
using AIO.Helpers.Caching;
using robotManager.Helpful;

namespace AIO.Combat.Common
{
    internal abstract class HealerRotation : BaseRotation
    {

        public WoWUnit Tank { get; set; }
        public WoWUnit Healer { get; set; }


        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];

        private readonly int MAX_CACHE_AGE = 100;

        private Stopwatch watch = Stopwatch.StartNew();

        public HealerRotation(bool runInCombat = true, bool runOutsideCombat = false, bool useCombatSynthetics = false, bool completelySynthetic = false) : base(runInCombat, runOutsideCombat, useCombatSynthetics, completelySynthetic) { }

        public WoWUnit GetTank(Func<WoWUnit, bool> predicate)
        {
            if (Tank == null)
            {
                Tank = RotationCombatUtil.FindPartyMember(u => u.Name == RotationFramework.TankName && predicate(u));
            }
            return Tank;
        }

        protected bool DoPreCalculations()
        {
            if (CacheIsValid(MAX_CACHE_AGE)) return true;
            //Logging.Write("Flushing cache");
            Cache.Reset();
            EnemiesAttackingGroup = RotationFramework.Enemies.Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember()).ToArray();
            return false;
        }

        private bool CacheIsValid(int age)
        {
            if (watch.ElapsedMilliseconds < age)
            {
                return true;
            }
            else
            {
                watch.Restart();
                return false;
            }
        }
        protected WoWUnit FindExplicitPartyMemberByName(string name) => RotationFramework.PartyMembers.FirstOrDefault(partyMember => partyMember.Name.ToLower().Equals(name.ToLower()));
    }
}