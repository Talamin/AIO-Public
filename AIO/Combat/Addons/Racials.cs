using AIO.Framework;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using static AIO.Constants;

namespace AIO.Combat.Addons
{
    internal class Racials : IAddon
    {
        private readonly Spell Cannibalize = new Spell("Cannibalize");
        private readonly Spell ArcaneTorrent = new Spell("Arcane Torrent");
        private readonly Spell WarStomp = new Spell("War Stomp");
        private readonly Spell BloodFury = new Spell("Blood Fury");
        private readonly Spell Berserking = new Spell("Berserking");
        private readonly Spell WilloftheForsaken = new Spell("Will of the Forsaken");
        private readonly Spell Everyman = new Spell("Every Man for Himself");
        private readonly Spell StoneForm = new Spell("Stoneform");
        private readonly Spell EscapeArtist = new Spell("Escape Artist");
        private readonly Spell GiftoftheNaruu = new Spell("Gift of the Naaru");
        private readonly Spell Shadowmeld = new Spell("Shadowmeld");

        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;

        private List<RotationStep> _rotation = new List<RotationStep>();

        public List<RotationStep> Rotation => _rotation;

        public void Initialize()
        {
            int berserkerBloodFRange = Me.WowClass == WoWClass.Warrior || Me.WowClass == WoWClass.Rogue || Me.WowClass == WoWClass.DeathKnight ? 7 : 30;
            switch (Me.WowRace)
            {
                case WoWRace.Orc:
                    _rotation.Add(new RotationStep(new RotationSpell(BloodFury), 1f, (s, t) => RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.GetDistance <= berserkerBloodFRange) >= 2 || BossList.MyTargetIsBoss, RotationCombatUtil.FindMe));
                    break;
                case WoWRace.Troll:
                    _rotation.Add(new RotationStep(new RotationSpell(Berserking), 1f, (s, t) => RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.GetDistance <= berserkerBloodFRange) >= 2 || BossList.MyTargetIsBoss, RotationCombatUtil.FindMe));
                    break;
                case WoWRace.Undead:
                    _rotation.Add(new RotationStep(new RotationSpell(WilloftheForsaken), 1f, (s, t) => Me.HaveBuff("Fear") || Me.HaveBuff("Charm") || Me.HaveBuff("Sleep"), RotationCombatUtil.FindMe));
                    _rotation.Add(new RotationStep(new RotationSpell(Cannibalize), 1f, (s, t) => !Me.HaveBuff("Drink") && !Me.HaveBuff("Food") && RotationFramework.AllUnits.Where(u => u.GetDistance <= 8 && u.IsDead && (u.CreatureTypeTarget == "Humanoid" || u.CreatureTypeTarget == "Undead")).Count() > 0, RotationCombatUtil.FindMe));
                    break;
                case WoWRace.Human:
                    _rotation.Add(new RotationStep(new RotationSpell(Everyman), 1f, (s, t) => Me.HaveBuff("Fear") || Me.HaveBuff("Charm") || Me.HaveBuff("Sleep"), RotationCombatUtil.FindMe));
                    break;
                case WoWRace.Tauren:
                    _rotation.Add(new RotationStep(new RotationSpell(WarStomp), 1f, (s, t) => !Me.HaveBuff("Cat Form") && !Me.HaveBuff("Bear Form") && !Me.HaveBuff("Dire Bear Form") && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.GetDistance <= 7) >= 2, RotationCombatUtil.FindMe));
                    break;
                case WoWRace.Gnome:
                    _rotation.Add(new RotationStep(new RotationSpell(EscapeArtist), 1f, (s, t) => Me.Rooted || Me.HaveBuff("Frostnova"), RotationCombatUtil.FindMe));
                    break;
                case WoWRace.Dwarf:
                    _rotation.Add(new RotationStep(new RotationSpell(StoneForm), 1f, (s, t) => Extension.HasPoisonDebuff() || Extension.HasDiseaseDebuff(), RotationCombatUtil.FindMe));
                    break;
                case WoWRace.BloodElf:
                    _rotation.Add(new RotationStep(new RotationSpell(ArcaneTorrent), 1f, (s, t) => Target.GetDistance <= 8 && Target.IsCast, RotationCombatUtil.FindMe));
                    break;
                case WoWRace.Draenei:
                    _rotation.Add(new RotationStep(new RotationSpell(GiftoftheNaruu), 1f, (s, t) => RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.GetDistance <= 7) >= 2, RotationCombatUtil.FindMe));
                    break;
                case WoWRace.NightElf:
                    _rotation.Add(new RotationStep(new RotationSpell(Shadowmeld), 1f, (s, t) => Me.HealthPercent < 5, RotationCombatUtil.FindMe));
                    break;
            }
        }

        public void Dispose() { }
    }
}
