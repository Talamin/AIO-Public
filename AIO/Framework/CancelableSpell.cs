using System;
using System.Collections.Generic;
using System.Windows.Forms;
using robotManager.Helpful;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.ObjectManager;

namespace AIO.Framework {
    public class CancelableSpell : RotationSpell {
        private static readonly object Lock = new object();
        private static CancelableSpell _current;
        private readonly Func<WoWUnit, bool> CancelPred;
        private WoWUnit Target;

        public CancelableSpell(string name, Func<WoWUnit, bool> cancelPred, bool ignoresGlobal = false) : base(name,
            ignoresGlobal) {
            CancelPred = cancelPred;
        }
        
        public CancelableSpell(Spell spell, Func<WoWUnit, bool> cancelPred, bool ignoresGlobal = false) : base(spell,
            ignoresGlobal) {
            CancelPred = cancelPred;
        }

        public static bool Check() {
            var cancelled = false;

            lock (Lock) {
                if (_current != null) {
                    cancelled = _current.CheckCancel();
                }
            }
            
            return cancelled;
        }

        private bool CheckCancel() {
            var cancelled = false;
            
            lock(Lock) {
                if (_current == this && CancelPred(Target)) {
                    _current = null;
                    cancelled = true;
                    RotationCombatUtil.StopCasting();
                    Logging.WriteFight($"Canceled {Name}");
                }
            }
            
            return cancelled;
        }

        public static void CastStopHandler(string id, List<string> args) {
            lock (Lock) {
                if (_current != null &&
                    (id == "UNIT_SPELLCAST_FAILED" ||
                     id == "UNIT_SPELLCAST_FAILED_QUIET" ||
                     id == "UNIT_SPELLCAST_INTERRUPTED" ||
                     id == "UNIT_SPELLCAST_STOP" ||
                     id == "UNIT_SPELLCAST_SUCCEEDED")
                    && args.Count >= 1 && args[0].Equals("player")) {
                    _current = null;
                }
            }
        }

        public override bool Execute(WoWUnit target, bool force = false) {
            Target = target;
            bool castSuccessful = RotationCombatUtil.CastSpell(this, target, force);
            if(castSuccessful) {
                lock (Lock) {
                    _current = this;
                }
            }

            return castSuccessful;
        }
    }
}