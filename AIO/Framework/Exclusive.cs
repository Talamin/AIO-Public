namespace AIO.Framework
{
    /* 
     * Exclusive is a token attached to an IRotationAction
     * 
     * During one rotation run, only one spell associated with a specific token will be executed, per target.
     */
    public class Exclusive
    {
        internal string Name;

        private Exclusive(string Name) => this.Name = Name;

        public static readonly Exclusive HunterAspect = new Exclusive("HunterAspect");
        public static readonly Exclusive WarlockSkin = new Exclusive("WarlockSkin");
        public static readonly Exclusive MageArmor = new Exclusive("MageArmor");
        public static readonly Exclusive ShamanShield = new Exclusive("ShamanShield");
        public static readonly Exclusive PaladinBlessing = new Exclusive("PaladinBlessing");
    }
}
