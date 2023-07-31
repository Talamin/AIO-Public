using AIO.Framework;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using wManager.Events;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Addons
{
    internal class PetAutoTarget : IAddon
    {
        private readonly string tauntSpellName;
        public bool RunOutsideCombat => false;
        public bool RunInCombat => true;

        public List<RotationStep> Rotation => new List<RotationStep>();

        public PetAutoTarget(string taunt)
        {
            tauntSpellName = taunt;
        }

        public void Initialize() => FightEvents.OnFightLoop += OnFightLoop;

        public void Dispose() => FightEvents.OnFightLoop -= OnFightLoop;

        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (!Pet.IsAlive || !Pet.IsValid || Me.IsInGroup)
            {
                return;
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
                return;
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
                return;
            }

            PetManager.PetSpellCast(tauntSpellName);
        }
    }
}
