using AIO.Framework;
using wManager.Wow.ObjectManager;

namespace AIO.Helpers
{
    public class DebugSpell : IRotationAction
    {
        private readonly string Name;

        public DebugSpell(string name, float maxRange = int.MaxValue)
        {
            Name = name;
            MaxRange = maxRange;
        }

        public bool Execute(WoWUnit target, bool force = false)
        {
            // RotationLogger.Trace($"[Debug] Executing {Name}.");
            return true;
        }

        public float MaxRange { get; }

        public (bool, bool) Should(WoWUnit target)
        {
            return (true, true);
        }
    }
}