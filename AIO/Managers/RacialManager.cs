using AIO.Combat.Common;
using AIO.Framework;
using robotManager.Helpful;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using wManager.Events;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

internal class RacialManager : ICycleable
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

    public static bool Enabled = true;

    private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
    {
        if (!Enabled)
        {
            return;
        }

        //Bloodfury
        if (BloodFury.KnownSpell && BloodFury.IsSpellUsable && Me.InCombat)
        {
            if (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.GetDistance <= 7) >= 2 || BossList.isboss)
            {
                RacialBloodFury();
            }
        }
        //Berserking
        if (Berserking.KnownSpell && Berserking.IsSpellUsable && Me.InCombat)
        {
            var range = Me.WowClass == WoWClass.Warrior || Me.WowClass == WoWClass.Rogue || Me.WowClass == WoWClass.DeathKnight ? 7 : 30;
            if (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.GetDistance <= range) >= 2 || BossList.isboss)
            {
                RacialBerserking();
            }
        }
        //WillofForsaken
        if (WilloftheForsaken.KnownSpell)
        {
            RacialWillForsaken();
        }
        //EverymanforHimself
        if (Everyman.KnownSpell)
        {
            RacialEveryManforHimself();
        }
        //Warstomp
        if (WarStomp.KnownSpell && WarStomp.IsSpellUsable && Me.InCombat && !Me.HaveBuff("Cat Form") && !Me.HaveBuff("Bear Form") && !Me.HaveBuff("Dire Bear Form"))
        {
            if (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.GetDistance <= 7) >= 2)
            {
                RacialWarStomp();
            }
        }
        //Escapeartist
        if (EscapeArtist.KnownSpell)
        {
            RacialEscapeArtist();
        }
        //Stoneform
        if (StoneForm.KnownSpell)
        {
            RacialStoneform();
        }
        if (ArcaneTorrent.KnownSpell && Target.GetDistance <= 8 && Target.IsCast)
        {
            RacialArcaneTorrent();
        }
        if (GiftoftheNaruu.KnownSpell && GiftoftheNaruu.IsSpellUsable && Me.InCombat)
        {
            if (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.GetDistance <= 7) >= 2)
            {
                RacialGiftofNaru();
            }
        }
        if (Shadowmeld.KnownSpell && Me.HealthPercent < 5)
        {
            RacialShadowmeld();
        }
    }

    private void OnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
    {
        if (!Enabled)
        {
            return;
        }

        if (!Me.InCombat && Me.HealthPercent < 50 && Me.IsAlive)
        {
            RacialCannibalize();
        }
    }

    private void RacialCannibalize()
    {
        if (RotationFramework.AllUnits.Where(u => u.GetDistance <= 8 && u.IsDead && (u.CreatureTypeTarget == "Humanoid" || u.CreatureTypeTarget == "Undead")).Count() > 0)
        {
            if (!Me.HaveBuff("Drink") && !Me.HaveBuff("Food") && Me.IsAlive)
            {
                if (Cannibalize.KnownSpell && Cannibalize.IsSpellUsable)
                {
                    Cannibalize.Launch();
                    Usefuls.WaitIsCasting();
                }
            }
        }
    }

    private void RacialShadowmeld()
    {
        if (Me.InCombat && Shadowmeld.KnownSpell && Shadowmeld.IsSpellUsable)
        {
            Shadowmeld.Launch();
            Thread.Sleep(8000);
            Usefuls.WaitIsCasting();
        }
    }

    private void RacialWillForsaken()
    {
        if (Me.InCombat && (Me.HaveBuff("Fear") || Me.HaveBuff("Charm") || Me.HaveBuff("Sleep")))
        {
            if (WilloftheForsaken.KnownSpell && WilloftheForsaken.IsSpellUsable)
            {
                WilloftheForsaken.Launch();
                Usefuls.WaitIsCasting();
            }
        }
    }

    private void RacialGiftofNaru()
    {
        if (Me.InCombat && GiftoftheNaruu.KnownSpell && GiftoftheNaruu.IsSpellUsable)
        {
            GiftoftheNaruu.Launch();
            Usefuls.WaitIsCasting();
        }
    }

    private void RacialEveryManforHimself()
    {
        if (Me.InCombat && (Me.HaveBuff("Fear") || Me.HaveBuff("Charm") || Me.HaveBuff("Sleep")))
        {
            if (Everyman.KnownSpell && Everyman.IsSpellUsable)
            {
                Everyman.Launch();
                Usefuls.WaitIsCasting();
            }
        }
    }

    private void RacialStoneform()
    {
        if (Me.InCombat && (Extension.HasPoisonDebuff() || Extension.HasDiseaseDebuff()))
        {
            if (StoneForm.KnownSpell && StoneForm.IsSpellUsable)
            {
                StoneForm.Launch();
                Usefuls.WaitIsCasting();
            }
        }
    }

    private void RacialEscapeArtist()
    {
        if (Me.InCombat && Me.Rooted || Me.HaveBuff("Frostnova"))
        {
            if (EscapeArtist.KnownSpell && EscapeArtist.IsSpellUsable)
            {
                EscapeArtist.Launch();
                Usefuls.WaitIsCasting();
            }
        }
    }

    private void RacialWarStomp()
    {
        if (Me.InCombat && Me.IsAlive && !Me.HaveBuff("Bear Form") && !Me.HaveBuff("Cat Form") && !Me.HaveBuff("Dire Bear Form"))
        {
            if (WarStomp.KnownSpell && WarStomp.IsSpellUsable)
            {
                WarStomp.Launch();
                Usefuls.WaitIsCasting();
            }
        }
    }

    private void RacialArcaneTorrent()
    {
        if (Me.InCombat && Me.IsAlive)
        {
            if (ArcaneTorrent.KnownSpell && ArcaneTorrent.IsSpellUsable)
            {
                ArcaneTorrent.Launch();
                Usefuls.WaitIsCasting();
            }
        }
    }

    private void RacialBloodFury()
    {
        if (Me.InCombat && Me.IsAlive)
        {
            if (BloodFury.KnownSpell && BloodFury.IsSpellUsable)
            {
                BloodFury.Launch();
                Usefuls.WaitIsCasting();
            }
        }
    }

    private void RacialBerserking()
    {
        if (Me.InCombat && Me.IsAlive)
        {
            if (Berserking.KnownSpell && Berserking.IsSpellUsable)
            {
                Berserking.Launch();
                Usefuls.WaitIsCasting();
            }
        }
    }

    public void Initialize()
    {
        FightEvents.OnFightLoop += OnFightLoop;
        MovementEvents.OnMovementPulse += OnMovementPulse;
    }

    public void Dispose()
    {
        FightEvents.OnFightLoop -= OnFightLoop;
        MovementEvents.OnMovementPulse -= OnMovementPulse;
    }
}

