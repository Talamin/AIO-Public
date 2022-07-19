using System.Collections.Generic;
using wManager.Wow.ObjectManager;

namespace AIO.Framework
{
    public class Exclusives
    {
        private readonly Dictionary<ulong, HashSet<Exclusive>> Tokens = new Dictionary<ulong, HashSet<Exclusive>>();

        public bool Add(WoWUnit unit, Exclusive exclusive)
        {
            /* Null tokens are ignored */
            if (exclusive == null)
            {
                return false;
            }

            /* Create the set if it doesn't already exist */
            if (!Tokens.TryGetValue(unit.Guid, out HashSet<Exclusive> set))
            {
                set = new HashSet<Exclusive>();
                Tokens[unit.Guid] = set;
            }

            /* Add the token to the (new) set */
            return set.Add(exclusive);
        }

        public bool Remove(WoWUnit unit, Exclusive exclusive)
        {
            /* Null tokens are ignored */
            if (exclusive == null)
            {
                return false;
            }

            /* There is nothing to remove if the set is empty */
            if (!Tokens.TryGetValue(unit.Guid, out HashSet<Exclusive> set))
            {
                return false;
            }

            /* Remove the token from the set */
            return set.Remove(exclusive);
        }

        public bool Contains(WoWUnit unit, Exclusive exclusive)
        {
            /* Null tokens are ignored */
            if (exclusive == null)
            {
                return false;
            }

            /* If the set does not exist, it is automatically empty */
            if (!Tokens.TryGetValue(unit.Guid, out HashSet<Exclusive> set))
            {
                return false;
            }

            /* Check if the set contains the token */
            return set.Contains(exclusive);
        }
    }
}
