using wManager.Wow.ObjectManager;

namespace AIO.Framework
{
    public interface IRotationAction
    {
        bool Execute(WoWUnit target, bool force = false);

        float MaxRange { get; }

        bool IgnoresGlobal { get; }

        (bool, bool) Should(WoWUnit target);
    }
}