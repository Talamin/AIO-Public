using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Framework
{
    public class RotationBuff : RotationSpell
    {
        private readonly int MinimumStacks;
        private readonly int MinimumRefreshTimeLeft;

        public RotationBuff(string name, bool ignoresGlobal = false, int minimumStacks = 0, int minimumRefreshTimeLeft = 0) :
            base(name, ignoresGlobal)
        {
            MinimumStacks = minimumStacks;
            MinimumRefreshTimeLeft = minimumRefreshTimeLeft;
        }

        public override (bool, bool) Should(WoWUnit target)
        {
            var buffs = BuffManager.GetAuras(target.GetBaseAddress, Spell.Ids);
            var someBuff = buffs.FirstOrDefault();

            // If the target doesn't have the buff at all, we should cast it
            // and consume the token.
            if (someBuff == null)
            {
                return (true, true);
            }

            var myBuffs = buffs.Where(b => b.Owner == Me.Guid);
            var myBuff = myBuffs.FirstOrDefault();

            // We should cast this buff if the stacks from our own buff are not enough.
            var should = (myBuff?.Stack ?? 0) < MinimumStacks ||
                (myBuff?.TimeLeft ?? 0) < MinimumRefreshTimeLeft;

            // We should consume the token only if we casted this buff.
            var consume = myBuff != null;

            return (should, consume);
        }
    }
}
