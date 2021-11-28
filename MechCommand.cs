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


        public sealed class MechBuy : MechCommand
        {
            public readonly string commodityName;
            public readonly int count;

            public MechBuy (string name, int count) {
                this.commodityName = name;
                this.count = count;
            }

            public override string show () {
                return $"buy {commodityName} {count}";
            }
        }

        public sealed class MechSell : MechCommand
        {
            public readonly string commodityName;
            public readonly int count;

            public MechSell (string name, int count) {
                this.commodityName = name;
                this.count = count;
            }

            public override string show () {
                return $"sell {commodityName} {count}";
            }
        }


        public sealed class MechInventory : MechCommand
        {
            public MechInventory () { }

            public override string show () {
                return "inventory";
            }
        }

        public sealed class MechMarketPrices : MechCommand
        {
            public MechMarketPrices () { }

            public override string show () {
                return "marketPrices";
            }
        }

        public sealed class MechCityPrices : MechCommand
        {
            public MechCityPrices () { }

            public override string show () {
                return "cityPrices";
            }
        }

    }

}