using AIO.Helpers.Caching;
using AIO.Lists;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager;
using wManager.Wow;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using Math = System.Math;

namespace AIO.Framework
{
    public static class RotationCombatUtil
    {
        private static Dictionary<DebuffType, List<WoWUnit>> _cachedDebuffedPlayers = new Dictionary<DebuffType, List<WoWUnit>>();
        private static List<WoWUnit> _enemiesToInterrupt = new List<WoWUnit>();
        public static bool CacheLUADebuffedPartyMembersStep()
        {
            _cachedDebuffedPlayers = LuaCache.GetLUADebuffedPartyMembers();
            return false;
        }

        // Remember to call CacheLUADebuffedPartyMembersStep() before this method in your rotation
        public static bool IHaveCachedDebuff(DebuffType debuffType)
        {
            if (_cachedDebuffedPlayers.TryGetValue(debuffType, out List<WoWUnit> players))
                return players.Exists(p => p.Guid == Me.Guid);
            return false;
        }

        // Remember to call CacheLUADebuffedPartyMembersStep() before this method in your rotation
        public static List<WoWUnit> GetPartyMembersWithCachedDebuff(DebuffType debuffType, bool checkLos, float maxDistance = float.MaxValue)
        {
            if (_cachedDebuffedPlayers.TryGetValue(debuffType, out List<WoWUnit> players))
            {
                return players
                    .Where(m => m.GetDistance < maxDistance && (!checkLos || !TraceLine.TraceLineGo(m.Position)))
                    .ToList();
            }
            return new List<WoWUnit>();
        }

        private static readonly List<string> AreaSpells = new List<string> {
            "Death and Decay",
            "Mass Dispel",
            "Blizzard",
            "Rain of Fire",
            "Freeze",
            "Volley",
            "Flare",
            "Hurricane",
            "Flamestrike",
            "Distract",
            "Force of Nature",
            "Shadowfury"
        };

        public static bool freeMove = false;

        public static WoWUnit GetBestAoETarget(Func<WoWUnit, bool> predicate, float range, float size,
            IEnumerable<WoWUnit> units, int minimum = 0)
        {
            // As this is still a C language, we can use fixed memory addresses with direct memory access -> Performance
            WoWUnit[] unitArray = units as WoWUnit[] ?? units.ToArray();
            return unitArray.Where(target => target.GetDistance < range)
                .Select(unit => new KeyValuePair<WoWUnit, int>(unit,
                    unitArray.Count(otherUnit =>
                        unit.Position.DistanceTo(otherUnit.Position) < size && predicate(otherUnit))))
                .OrderByDescending(pair => pair.Value).FirstOrDefault(pair => pair.Value >= minimum).Key;
        }

        public static WoWUnit CGetHighestHpPartyMemberTarget(Func<WoWUnit, bool> predicate) =>
            RotationFramework.PartyMembers
                .Where(partyMember => partyMember.CInCombat() && partyMember.TargetObject?.Target == partyMember.Guid &&
                                      predicate(partyMember.TargetObject))
                .OrderByDescending(partyMember => partyMember.Health).FirstOrDefault()?.TargetObject;

        public static bool IsCurrentSpell(this Spell spell) => IsCurrentSpell(spell.Name);

        public static bool IsCurrentSpell(string spellName) =>
            Lua.LuaDoString<bool>(
                $"if IsCurrentSpell('{spellName}') == 1 then return true else return false end");

        public static int CCountAlivePartyMembers(Func<WoWUnit, bool> predicate)
        {
            var count = 0;

            for (var i = 0; i < RotationFramework.PartyMembers.Length; i++)
            {
                WoWPlayer partyMember = RotationFramework.PartyMembers[i];
                if (partyMember.CIsAlive() && predicate(partyMember))
                    count++;
            }

            return count;
        }

        public static bool CHurtPartyMembersAtLeast(Func<WoWPlayer, bool> predicate, byte amount)
        {
            var count = 0;

            for (byte i = 0; i < RotationFramework.PartyMembers.Length; i++)
            {
                WoWPlayer partyMember = RotationFramework.PartyMembers[i];
                if (partyMember.CHealthPercent() > 99) break;
                if (partyMember.CIsAlive() && predicate(partyMember))
                    count++;
                if (count >= amount) return true;
            }

            return false;
        }

        public static ushort CCountHurtPartyMembers(Func<WoWPlayer, bool> predicate)
        {
            ushort count = 0;

            for (ushort i = 0; i < RotationFramework.PartyMembers.Length; i++)
            {
                WoWPlayer partyMember = RotationFramework.PartyMembers[i];
                if (partyMember.CHealthPercent() > 99) break;
                if (partyMember.CIsAlive() && predicate(partyMember))
                    count++;
            }

            return count;
        }

        public static bool CAnyPartyMemberAlive()
        {
            for (var i = 0; i < RotationFramework.PartyMembers.Length; i++)
                if (RotationFramework.PartyMembers[i].CIsAlive())
                    return true;
            return false;
        }

        public static int CountAliveGroupMembers(Func<WoWUnit, bool> predicate)
        {
            var count = 0;

            List<WoWPlayer> party = Party.GetParty();

            for (var i = 0; i < party.Count; i++)
            {
                WoWPlayer partyMember = party[i];
                if (partyMember.IsAlive && predicate(partyMember))
                    count++;
            }

            return count;
        }

        public static int CCountHurtGroupMembers(Func<WoWUnit, bool> predicate)
        {
            var count = 0;

            List<WoWPlayer> party = Party.GetParty();

            for (var i = 0; i < party.Count; i++)
            {
                WoWPlayer partyMember = party[i];
                if (partyMember.CHealthPercent() > 99) break;
                if (partyMember.CIsAlive() && predicate(partyMember))
                    count++;
            }

            return count;
        }

        public static int CountEnemies(Func<WoWUnit, bool> predicate) =>
            RotationFramework.Enemies.Count(unit =>
                unit.IsAlive && unit.IsEnemy() && predicate(unit));

        public static WoWUnit FindFriendlyPlayer(Func<WoWUnit, bool> predicate) =>
            RotationFramework.PlayerUnits.Where(u => u.Reaction == Reaction.Friendly && u.IsAlive && predicate(u))
                .OrderBy(u => u.HealthPercent).FirstOrDefault();

        public static WoWUnit FindExplicitPartyMember(Func<WoWUnit, bool> predicate) =>
            FindPartyMember(u => !u.IsLocalPlayer && predicate(u));

        public static WoWUnit CFindExplicitHurtPartyMember(Func<WoWUnit, bool> predicate) =>
            CFindHurtPartyMember(partyMember => !partyMember.IsLocalPlayer && predicate(partyMember));

        public static WoWUnit CFindHurtPartyMember(Func<WoWUnit, bool> predicate)
        {
            for (ushort i = 0; i < RotationFramework.PartyMembers.Length; i++)
            {
                WoWPlayer partyMember = RotationFramework.PartyMembers[i];
                if (partyMember.CHealthPercent() > 99) break;
                if (predicate(partyMember)) return partyMember;
            }

            return null;
        }

        public static WoWUnit FindPartyMember(Func<WoWUnit, bool> predicate) =>
            RotationFramework.PartyMembers.FirstOrDefault(partyMember =>
                partyMember.IsAlive && predicate(partyMember));

        public static WoWUnit CFindPartyMember(Func<WoWUnit, bool> predicate)
        {
            for (var i = 0; i < RotationFramework.PartyMembers.Length; i++)
            {
                WoWPlayer partyMember = RotationFramework.PartyMembers[i];
                if (partyMember.CIsAlive() && predicate(partyMember)) return partyMember;
            }

            return null;
        }

        public static WoWUnit CFindPartyMemberWithoutMe(Func<WoWUnit, bool> predicate)
        {
            for (var i = 0; i < RotationFramework.PartyMembers.Length; i++)
            {
                WoWPlayer partyMember = RotationFramework.PartyMembers[i];
                if (partyMember.CIsAlive() && partyMember != ObjectManager.Me && predicate(partyMember)) return partyMember;
            }

            return null;
        }

        public static WoWUnit FindTank(Func<WoWUnit, bool> predicate) =>
            FindPartyMember(u => u.Name == RotationFramework.TankName && predicate(u));

        public static WoWUnit CFindTank(Func<WoWUnit, bool> predicate) =>
           CFindPartyMember(u => u.Name == RotationFramework.TankName && predicate(u));

        public static WoWUnit FindHeal(Func<WoWUnit, bool> predicate) =>
            FindPartyMember(u => u.Name == RotationFramework.HealName && predicate(u));

        public static WoWUnit CFindHeal(Func<WoWUnit, bool> predicate) =>
            CFindPartyMember(u => u.Name == RotationFramework.HealName && predicate(u));

        public static WoWUnit FindEnemy(Func<WoWUnit, bool> predicate) =>
            RotationFramework.Enemies.FirstOrDefault(predicate);

        public static WoWUnit FindEnemyPlayer(Func<WoWUnit, bool> predicate) =>
            FindEnemy(RotationFramework.PlayerUnits, predicate);

        public static WoWUnit FindEnemyCasting(Func<WoWUnit, bool> predicate) =>
            FindUnitCasting(RotationFramework.Enemies, predicate);

        public static WoWUnit FindEnemyCastingWithLoS(Func<WoWUnit, bool> predicate) =>
            FindUnitCastingWithLoS(RotationFramework.Enemies, predicate);

        public static WoWUnit FindPlayerCasting(Func<WoWUnit, bool> predicate) =>
            FindUnitCasting(RotationFramework.PlayerUnits, predicate);

        public static WoWUnit FindEnemyCastingOnMe(Func<WoWUnit, bool> predicate) =>
            FindUnitCastingOnMe(RotationFramework.Enemies, predicate);

        public static WoWUnit FindEnemyCastingOnGroup(Func<WoWUnit, bool> predicate) =>
            FindUnitCastingOnGroup(RotationFramework.Enemies, predicate);

        public static WoWUnit FindPlayerCastingOnMe(Func<WoWUnit, bool> predicate) =>
            FindUnitCastingOnMe(RotationFramework.PlayerUnits, predicate);

        public static WoWUnit FindEnemyTargetingMe(Func<WoWUnit, bool> predicate) =>
            FindEnemyTargetingMe(RotationFramework.Enemies, predicate);

        public static WoWUnit CFindInRange(this WoWUnit[] units, Func<WoWUnit, bool> predicate, float distance, int count = 0)
        {
            int length = count == 0 ? units.Length : Math.Min(count, units.Length);
            for (var i = 0; i < length; i++)
            {
                WoWUnit unit = units[i];
                if (unit.CGetDistance() > distance) return null;
                if (predicate(unit)) return unit;
            }

            return null;
        }

        public static WoWUnit FindEnemyAttackingGroup(Func<WoWUnit, bool> predicate) => RotationFramework.Enemies
            .FirstOrDefault(u => u.IsAttackable &&
                                 !u.IsTargetingMe &&
                                 u.IsTargetingPartyMember &&
                                 predicate(u));

        public static WoWUnit FindEnemyAttackingGroupAndMe(Func<WoWUnit, bool> predicate) => RotationFramework.Enemies
            .FirstOrDefault(u => u.IsAttackable &&
                                 u.IsTargetingMeOrMyPetOrPartyMember &&
                                 predicate(u));

        private static WoWUnit FindUnitCasting(IEnumerable<WoWUnit> units, Func<WoWUnit, bool> predicate) =>
            FindEnemy(units, u => predicate(u) && u.IsCast);
        private static WoWUnit FindUnitCastingWithLoS(IEnumerable<WoWUnit> units, Func<WoWUnit, bool> predicate) =>
            FindEnemy(units, u => predicate(u) && u.IsCast && !TraceLine.TraceLineGo(u.Position));

        private static WoWUnit FindUnitCastingOnMe(IEnumerable<WoWUnit> units, Func<WoWUnit, bool> predicate) =>
            FindUnitCasting(units, u => predicate(u) && u.IsTargetingMe);

        private static WoWUnit FindUnitCastingOnGroup(IEnumerable<WoWUnit> units, Func<WoWUnit, bool> predicate) =>
            FindUnitCasting(units, u => u.IsTargetingPartyMember && predicate(u));

        private static WoWUnit FindEnemyTargetingMe(IEnumerable<WoWUnit> units, Func<WoWUnit, bool> predicate) =>
            FindEnemy(units, u => predicate(u) && u.IsTargetingMe);

        private static WoWUnit FindEnemy(IEnumerable<WoWUnit> units, Func<WoWUnit, bool> predicate) =>
            units.Where(u =>
                    u.IsAlive && u.IsEnemy() && predicate(u))
                .OrderBy(u => u.GetDistance).FirstOrDefault();

        public static WoWUnit BotTarget(Func<WoWUnit, bool> predicate) =>
            !TraceLine.TraceLineGo(Target?.Position) && predicate(Target) ? Target : null;

        public static WoWUnit BotTargetFast(Func<WoWUnit, bool> predicate) => predicate(Target) ? Target : null;

        public static WoWUnit PetTarget(Func<WoWUnit, bool> predicate) =>
            !TraceLine.TraceLineGo(Pet?.TargetObject?.Position) && predicate(Pet?.TargetObject)
                ? Pet?.TargetObject
                : null;

        public static WoWUnit FindPet(Func<WoWUnit, bool> predicate) =>
            !TraceLine.TraceLineGo(Pet?.Position) && predicate(Pet) ? Pet : null;

        public static WoWUnit FindMe(Func<WoWUnit, bool> predicate) => predicate(Me) ? Me : null;

        public static bool Always(IRotationAction action, WoWUnit target) => true;

        public static int EnemyAttackingCount() =>
            RotationFramework.Enemies.Count(u => u.IsTargetingMeOrMyPetOrPartyMember && u.IsAttackable);

        public static int EnemyAttackingCountCluster(int Range) => RotationFramework.Enemies.Count(u =>
            u.IsTargetingMeOrMyPetOrPartyMember && u.IsAttackable && u.Position.DistanceTo(Target.Position) <= Range);

        public static uint GetThreatStatus(this WoWUnit unit) =>
            Lua.LuaDoString<uint>($"return UnitThreatSituation(\"{unit.Name}\");");

        public static int GetThreatStatusMemory(this WoWUnit unit)
        {
            const uint ClntObjMgrGetActivePlayerObj = 0x004038F0 - 0x400000;
            const uint UnitThreatSituationFunc = 0x0052A1A0 - 0x400000;
            const uint objGuid = 0x30;
            uint baseAddress = unit.GetBaseAddress;

            try
            {
                if (!Conditions.InGameAndConnected)
                    return -1;

                if (baseAddress > 0)
                {
                    uint resultCodecave = Memory.WowMemory.AllocData.Get(0x4);

                    if (resultCodecave <= 0)
                        return -1;

                    string[] asm = new[] {
                        Memory.WowMemory.CallWrapperCode(
                            Memory.WowMemory.Memory.RebaseAddress(ClntObjMgrGetActivePlayerObj)),
                        "test eax, eax",
                        "je @out",
                        "mov ecx, " + (baseAddress + objGuid),
                        Memory.WowMemory.CallWrapperCodeRebaseEsp(
                            Memory.WowMemory.Memory.RebaseAddress(UnitThreatSituationFunc), 4, "ecx"),
                        "movzx ecx, al",
                        "sub ecx, 1",
                        "mov [" + resultCodecave + "], ecx",
                        "@out:",
                        Memory.WowMemory.RetnToHookCode
                    };

                    Memory.WowMemory.Memory.WriteInt32(resultCodecave, 0);

                    Memory.WowMemory.InjectAndExecute(asm);
                    int result = Memory.WowMemory.Memory.ReadInt32(resultCodecave);

                    Memory.WowMemory.AllocData.Free(resultCodecave);

                    return Math.Max(0, result);
                }
            }
            catch
            {
                Logging.WriteError("Failed to get ThreatStatus from memory.");
            }

            return -1;
        }

        public static int GetAggroDifferenceFor(WoWUnit unit)
        {
            uint myThreat = GetThreatStatus(unit);
            uint highestParty = (from p in RotationFramework.PartyMembers
                                 let tVal = GetThreatStatus(p)
                                 orderby tVal descending
                                 select tVal).FirstOrDefault();

            int result = (int)myThreat - (int)highestParty;
            return result;
        }

        public static bool IsAutoRepeating(string name) =>
            Lua.LuaDoString<bool>($"return IsAutoRepeatSpell(\"{name}\")");

        public static bool IsAutoAttacking() =>
            Lua.LuaDoString<bool>("return IsCurrentSpell('Auto Attack') == 1 or IsCurrentSpell('Auto Attack') == true");

        public static bool CastSpell(RotationSpell spell, WoWUnit unit, bool force)
        {
            if (unit == null ||
                !spell.KnownSpell ||
                !spell.IsSpellUsable ||
                !unit.IsValid ||
                unit.IsDead ||
                wManagerSetting.CurrentSetting.IgnoreFightGoundMount && Me.IsMounted && !LaggyTransports.Entries.Contains(ObjectManager.GetObjectByGuid(Me.TransportGuid).Entry))
                return false;

            //Lua.LuaDoString("if IsMounted() then Dismount() end");
            if (spell.CastTime > 0.0 && Me.GetMove)
            {
                if (freeMove) return false;
            }

            if (!force && Me.IsCast) return false;

            if (force) StopCasting();

            RotationLogger.Fight($"Casting {spell.Name} on {unit.Name} [{unit.Guid}]");
            SpellManager.CastSpellByNameOn(spell.Name, GetLuaId(unit));
            if (AreaSpells.Contains(spell.Name)) ClickOnTerrain.Pulse(unit.Position);

            // Usefuls.WaitIsCasting();
            //TimeSinceLastCast.Restart();
            return true;
        }

        //public static readonly Stopwatch TimeSinceLastCast = Stopwatch.StartNew();

        public static void StopCasting()
        {
            Lua.LuaDoString("SpellStopCasting();");
        }

        public static string GetLuaId(WoWUnit unit)
        {
            if (unit.Guid == Me.Guid)
                return "player";
            if (unit.Guid == Target.Guid)
                return "target";

            Me.FocusGuid = unit.Guid;
            return "focus";
        }

        public static T ExecuteActionOnUnit<T>(WoWUnit unit, Func<string, T> action) =>
            ExecuteActionOnTarget(unit.Guid, action);

        public static T ExecuteActionOnTarget<T>(ulong target, Func<string, T> action)
        {
            if (target == Me.Guid) return action("player");
            if (target == Target.Guid) return action("target");
            SetMouseoverGuid(target);
            return action("mouseover");
        }

        public static T ExecuteActionOnFocus<T>(ulong target, Func<string, T> action)
        {
            SetFocusGuid(target);
            return action("focus");
        }

        public static void SetFocusGuid(ulong guid)
        {
            Me.FocusGuid = guid;
        }

        private static void SetMouseoverGuid(ulong guid)
        {
            Me.MouseOverGuid = guid;
        }
    }
}