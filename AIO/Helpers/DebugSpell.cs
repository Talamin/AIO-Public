using AIO.Framework;
using wManager.Wow.ObjectManager;

namespace AIO.Helpers
{
    public class DebugSpell : IRotationAction
    {
        private readonly string Name;

        public DebugSpell(string name, float maxRange = int.MaxValue, bool ignoresGlobal = false)
        {
            Name = name;
            MaxRange = maxRange;
            IgnoresGlobal = ignoresGlobal;
        }

        public bool Execute(WoWUnit target, bool force = false)
        {
            // RotationLogger.Trace($"[Debug] Executing {Name}.");
            return true;
        }

        public float MaxRange { get; }
        public bool IgnoresGlobal { get; }

        public (bool, bool) Should(WoWUnit target)
        {
            return (true, true);
        }
    }
}