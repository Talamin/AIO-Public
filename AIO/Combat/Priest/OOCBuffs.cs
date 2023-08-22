using AIO.Combat.Addons;
using AIO.Framework;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Priest
{
    internal class OOCBuffs : IAddon
    {
        private bool _hasCandle;
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationAction("Candle Check", CandleCheck), 1500),
            new RotationStep(new RotationBuff("Prayer of Fortitude"), 1f, (s,t) =>  !Me.IsMounted && _hasCandle && NeedsFort(t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Prayer of Spirit"), 2f, (s,t) =>  !Me.IsMounted && _hasCandle && NeedsSpirit(t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Prayer of Shadow Protection"), 3f, (s,t) =>  !Me.IsMounted && _hasCandle && NeedsShadow(t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Power Word: Fortitude"), 4f, (s,t) =>  !Me.IsMounted && NeedsFort(t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Divine Spirit"), 5f, (s,t) =>  !Me.IsMounted && NeedsSpirit(t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff("Shadow Protection"), 6f, (s,t) =>  !Me.IsMounted && NeedsShadow(t), RotationCombatUtil.FindPartyMember),
        };

        public void Initialize() { }
        public void Dispose() { }

        private bool CandleCheck()
        {
            _hasCandle = ItemsManager.HasItemById(17029) || ItemsManager.HasItemById(17028);
            return false;
        }

        private bool NeedsFort(WoWUnit target) => !target.HaveBuff("Power Word: Fortitude") && !target.HaveBuff("Prayer of Fortitude") && !target.HaveBuff("Holy Word: Fortitude");
        private bool NeedsSpirit(WoWUnit target) => !target.HaveBuff("Divine Spirit") && !target.HaveBuff("Prayer of Spirit");
        private bool NeedsShadow(WoWUnit target) => !target.HaveBuff("Shadow Protection") && !target.HaveBuff("Prayer of Shadow Protection");
    }
}