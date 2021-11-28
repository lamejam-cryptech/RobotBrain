using System;

namespace RobotBrain {
    
    public abstract class MechCommand
    {
        private MechCommand () { }

        public abstract string show ();


        public sealed class MechStop : MechCommand
        {
            public MechStop () { }

            public override string show () {
                return "stop";
            }
        }


        public sealed class MechRotate : MechCommand
        {
            public readonly int angle;

            public MechRotate (int angle) {
                this.angle = angle;
            }

            public override string show () {
                return $"rotate {angle}";
            }
        }


        public sealed class MechMove : MechCommand
        {
            public readonly int distance;

            public MechMove (int distance) {
                this.distance = distance;
            }

            public override string show () {
                return $"move {distance}";
            }
        }

    }

}