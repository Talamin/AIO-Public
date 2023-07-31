using AIO.Framework;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using wManager.Events;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using Timer = robotManager.Helpful.Timer;

namespace AIO.Combat.Addons
{
    internal class AutoBackpedal : IAddon
    {
        private readonly Func<bool> Should;
        private readonly IEnumerable<Vector3> Circle;
        private readonly Timer MoveTimer = new Timer();

        public bool RunOutsideCombat => false;
        public bool RunInCombat => true;

        public AutoBackpedal(Func<bool> should, float range)
        {
            Should = should;

            const int points = 8;
            Circle = Enumerable.Range(0, points).Select(x => x * (360.0 / points))
                .Select(angle => new Vector3()
                {
                    X = (float)(range * System.Math.Cos(angle * (System.Math.PI / 180.0))),
                    Y = (float)(range * System.Math.Sin(angle * (System.Math.PI / 180.0))),
                    Z = 0
                }).ToList();
        }

        public List<RotationStep> Rotation => new List<RotationStep>();
        public void Initialize() => FightEvents.OnFightLoop += OnFightLoop;
        public void Dispose() => FightEvents.OnFightLoop -= OnFightLoop;

        private bool MoveToClosest()
        {
            var closest = Circle
                /* Project the angles around the target */
                .Select(b => new Vector3()
                {
                    X = Target.Position.X + b.X,
                    Y = Target.Position.Y + b.Y,
                    Z = Target.Position.Z + b.Z,
                }).
                /* Ensure positions are reachable */
                Where(x => !TraceLine.TraceLineGo(Target.Position, x) && !TraceLine.TraceLineGo(x)).
                /* Order by distance to current position */
                OrderBy(x => x.DistanceTo2D(Me.Position)).
                /* Attempt to retrieve the first one */
                FirstOrDefault();

            if (closest == null)
            {
                return false;
            }
            MovementManager.MoveTo(closest);
            return true;
        }

        private void OnFightLoop(WoWUnit unit, CancelEventArgs cancelable)
        {
            if (!Should() || !MoveTimer.IsReady)
            {
                return;
            }

            switch (Me.WowClass)
            {
                case WoWClass.Mage:

                    if (MoveToClosest())
                    {
                        Logging.WriteFight("Mage Backup");
                        Thread.Sleep(2800);
                        MoveTimer.Reset(2500);
                    }
                    break;
                case WoWClass.Hunter:
                    if (!Pet.IsAlive)
                    {
                        return;
                    }

                    if (Me.IsInGroup)
                    {
                        if (MoveToClosest())
                        {
                            Logging.WriteFight("Hunter Backup");
                            Thread.Sleep(2800);
                            MoveTimer.Reset(2500);
                        }
                        break;
                    }
                    else
                    {
                        var types = Others.Random(0, 2);
                        switch (types)
                        {
                            case 0:
                                Logging.WriteFight("Default  Backup");
                                Move.Backward(Move.MoveAction.PressKey, 1200);
                                break;
                            case 1:
                                Logging.WriteFight("Default  Backup");
                                Move.StrafeLeft(Move.MoveAction.PressKey, 1200);
                                break;
                            case 2:
                                Logging.WriteFight("Default  Backup");
                                Move.StrafeRight(Move.MoveAction.PressKey, 1200);
                                break;
                        }
                        MoveTimer.Reset(1200);
                    }
                    break;
                default:
                    var type = Others.Random(0, 2);
                    switch (type)
                    {
                        case 0:
                            Logging.WriteFight("Default  Backup");
                            Move.Backward(Move.MoveAction.PressKey, 1200);
                            break;
                        case 1:
                            Logging.WriteFight("Default  Backup");
                            Move.StrafeLeft(Move.MoveAction.PressKey, 1200);
                            break;
                        case 2:
                            Logging.WriteFight("Default  Backup");
                            Move.StrafeRight(Move.MoveAction.PressKey, 1200);
                            break;
                    }
                    MoveTimer.Reset(1200);
                    return;
            }
        }
    }
}
