using wManager.Wow.ObjectManager;

namespace AIO
{
    internal static class Constants
    {
        internal static WoWUnit Target => ObjectManager.Target;
        internal static WoWLocalPlayer Me => ObjectManager.Me;
        internal static WoWUnit Pet => ObjectManager.Pet;
    }
}
