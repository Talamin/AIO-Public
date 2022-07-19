using System;

namespace AIO.Combat.Common
{
    internal class ConditionalCycleable : ICycleable
    {
        private readonly Func<bool> Predicate;
        private readonly ICycleable Cycleable;

        public ConditionalCycleable(Func<bool> predicate, ICycleable cycleable)
        {
            Predicate = predicate;
            Cycleable = cycleable;
        }

        public void Dispose()
        {
            if (Predicate()) { Cycleable.Dispose(); }
        }

        public void Initialize()
        {
            if (Predicate()) { Cycleable.Initialize(); }
        }
    }
}
