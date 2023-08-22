using AIO.Framework;
using robotManager.Helpful;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Addons
{
    internal class PetAutoTarget : IAddon
    {
        private readonly string _tauntSpellName;
        private readonly bool _IAmHunter = Me.WowClass == wManager.Wow.Enums.WoWClass.Hunter;

        public bool RunOutsideCombat => false;
        public bool RunInCombat => true;

        public PetAutoTarget(string taunt)
        {
            _tauntSpellName = taunt;
        }

        public List<RotationStep> Rotation => new List<RotationStep>
        {
            new RotationStep(new RotationAction("Auto Pet Target", AutoTarget), 0f, 500),
        };

        public void Initialize() { }

        public void Dispose() { }

        private bool AutoTarget()
        {
            if (!Pet.IsAlive || !Pet.IsValid || Me.IsInGroup)
            {
                return false;
            }

            var validTargets = RotationFramework.Enemies.OrderBy(uu => uu.HealthPercent);

            var unitsAttackMe = validTargets.Where(u =>
                u.IsTargetingMe).ToList();
            var unitsAttackPet = validTargets.Where(u =>
                u.IsTargetingMyPet).ToList();

            var targets = unitsAttackMe;
            if (unitsAttackMe.Count == 0)
            {
                targets = unitsAttackPet;
            }

            var petTarget = targets.FirstOrDefault();
            if (petTarget == null)
            {
                return false;
            }

            if (!petTarget.IsMyPetTarget)
            {
                Me.FocusGuid = petTarget.Guid;
                Lua.RunMacroText("/petattack [@focus]");
                Lua.LuaDoString("ClearFocus();");

                Logging.WriteFight($"Changing pet target to {petTarget.Name} [{petTarget.Guid}]");
            }

            if (Me.IsInGroup)
            {
                return false;
            }

            if (_IAmHunter && Pet.Focus < 20) return false;
            PetManager.CastPetSpellIfReady(_tauntSpellName);
            return false;
        }
    }
}
