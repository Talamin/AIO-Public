using System;
using wManager.Wow.ObjectManager;

namespace AIO.Framework
{
    public class RotationAction : IRotationAction
    {
        private readonly string _name;
        private readonly Func<bool> _action;
        public string Name => _name;
        public float MaxRange => int.MaxValue;

        public RotationAction(string name, Func<bool> action)
        {
            _action = action;
            _name = name;
        }

        public virtual bool Execute(WoWUnit target, bool force = false)
        {
            return _action();
        }

        public virtual (bool, bool) Should(WoWUnit target) => (true, true);

        public bool IgnoresGlobal { get; }
    }
}