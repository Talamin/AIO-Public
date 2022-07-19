namespace AIO.Helpers.Caching
{
    public enum Entry : byte
    {
        ManaPercentage = 0,
        GetAuras = 1,
        InCombat = 2,
        Health = 3,
        IsAlive = 4,
        PositionWithoutType = 5,
        Rage = 6,
        IsTargetingMe = 7,
        CastingSpellId = 8,
        IsTargetingMeOrMyPetOrPartyMember = 9,
        MaxHealth = 10,
        Name = 11
    }
}