using System;

namespace AIO.Combat.Common
{
    internal class DeferredCycleable : ICycleable
    {
        private readonly Func<ICycleable> Cycleable;

        public DeferredCycleable(Func<ICycleable> cycleable) => Cycleable = cycleable;

        public void Dispose() => Cycleable()?.Dispose();

        public void Initialize() => Cycleable()?.Initialize();
    }
}
