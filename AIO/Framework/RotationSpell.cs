using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace AIO.Framework
{
    public class RotationSpell : IRotationAction
    {
        public readonly Spell Spell;

        public RotationSpell(string name, bool ignoresGlobal = false)
        {
            Spell = new Spell(name);
            IgnoresGlobal = ignoresGlobal;
        }
        
        public RotationSpell(Spell spell, bool ignoresGlobal = false)
        {
            Spell = spell;
            IgnoresGlobal = ignoresGlobal;
        }

        public string Name => Spell.Name;

        public bool IsSpellUsable => Spell.IsSpellUsable;

        public bool KnownSpell => Spell.KnownSpell;

        public float CastTime => Spell.CastTime;

        public float MaxRange => Spell.MaxRange;

        public virtual bool Execute(WoWUnit target, bool force = false) => RotationCombatUtil.CastSpell(this, target, force);

        public virtual (bool, bool) Should(WoWUnit target) => (true, true);

        public bool IgnoresGlobal { get; }

        //
        public static int GetSpellCost(string spellName)
        {
            return Lua.LuaDoString<int>("local name, rank, icon, cost, isFunnel, powerType, castTime, minRange, maxRange = GetSpellInfo('" + spellName + "'); return cost");
        }
    }
}