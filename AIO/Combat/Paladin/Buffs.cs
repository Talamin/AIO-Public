using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class Buffs : BaseRotation
    {
        private readonly BaseCombatClass CombatClass;
        private string Spec => CombatClass.Specialisation;

        internal Buffs(BaseCombatClass combatClass) : base(runInCombat: true, runOutsideCombat: true) => CombatClass = combatClass;

        private bool IsBoM(IRotationAction s, WoWUnit t)
        {
            switch (t.WowClass)
            {
                case WoWClass.Warrior:
                case WoWClass.Paladin:
                case WoWClass.Hunter:
                case WoWClass.Rogue:
                case WoWClass.DeathKnight:
                    return true;
                default:
                    return false;
            }
        }

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff("Seal of Command"), 1f, (s, t) => Spec == "Retribution" && Settings.Current.Sealret == "Seal of Command", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Vengeance"), 1.1f, (s, t) => Spec == "Retribution" && Settings.Current.Sealret == "Seal of Vengeance", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Righteousness"), 2f, (s, t) => Spec == "Retribution" && Settings.Current.Sealret == "Seal of Righteousness", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Justice"), 3f, (s, t) => Spec == "Retribution" && Settings.Current.Sealret == "Seal of Justice", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Righteous Fury"), 4f, (s, t) => Spec == "Protection", RotationCombatUtil.FindMe),

            new RotationStep(new RotationBuff("Seal of Vengeance"), 4.1f, (s, t) => Spec == "Protection" && Settings.Current.Sealprot == "Seal of Vengeance" && Me.ManaPercentage >= Settings.Current.ProtectionSoL, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Command"), 4.2f, (s, t) => Spec == "Protection" && Settings.Current.Sealprot == "Seal of Command" && Me.ManaPercentage >= Settings.Current.ProtectionSoL, RotationCombatUtil.FindMe),

            new RotationStep(new RotationBuff("Seal of Wisdom"), 5f, (s, t) => Spec == "Protection" && Me.ManaPercentage < Settings.Current.ProtectionSoW , RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Wisdom"), 5.1f, (s, t) => Spec == "Holy", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Light"), 6f, (s, t) => Spec == "Protection" && Me.ManaPercentage >= Settings.Current.ProtectionSoL && Settings.Current.Sealprot != "Seal of Vengeance" && Settings.Current.Sealprot != "Seal of Command", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Seal of Righteousness"), 7f, (s, t) => Spec == "Protection" && !SpellManager.KnowSpell("Seal of Light"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Blessing of Might"), 7.1f, (s, t) => !Me.IsInGroup && Spec == "Retribution" && !Me.HaveBuff("Battle Shout"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Blessing of Might"), 7.2f, (s,t) => !Me.IsInGroup && Spec == "Protection" && !SpellManager.KnowSpell("Blessing of Sanctuary"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Crusader Aura"), 7.3f, (s,t) => !Me.IsOnTaxi && Me.IsMounted && Settings.Current.Crusader, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Retribution Aura"), 8f, (s,t) => Settings.Current.Aura =="Retribution Aura", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Devotion Aura"), 9f, (s,t) => Settings.Current.Aura =="Devotion Aura", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Concentration Aura"), 10f, (s,t) => Settings.Current.Aura =="Concentration Aura", RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff("Blessing of Sanctuary"), 12f, (s, t) => Spec == "Protection", RotationCombatUtil.FindMe, Exclusive.PaladinBlessing),
            new RotationStep(new RotationBuff("Blessing of Wisdom"), 13f, (s,t) => t.WowClass != WoWClass.Shaman, RotationCombatUtil.FindHeal, Exclusive.PaladinBlessing),
            new RotationStep(new RotationBuff("Blessing of Might"), 14f, IsBoM,
                s => RotationFramework.PartyMembers.Count(o=> o.WowClass == WoWClass.Warrior) == 0, RotationCombatUtil.FindPartyMember, Exclusive.PaladinBlessing),
            new RotationStep(new RotationBuff("Blessing of Kings"), 15f, RotationCombatUtil.Always, RotationCombatUtil.FindPartyMember, Exclusive.PaladinBlessing),
            new RotationStep(new RotationBuff("Blessing of Wisdom"), 16f, RotationCombatUtil.Always, RotationCombatUtil.FindPartyMember, Exclusive.PaladinBlessing),
            new RotationStep(new RotationBuff("Blessing of Might"), 17f, RotationCombatUtil.Always, RotationCombatUtil.FindPartyMember, Exclusive.PaladinBlessing),
        };
    }
}
