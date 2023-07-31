using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;

namespace AIO.Combat.Addons
{
    internal interface IAddon : ICycleable
    {
        bool RunOutsideCombat { get; }
        bool RunInCombat { get; }
        List<RotationStep> Rotation { get; }
    }
}
