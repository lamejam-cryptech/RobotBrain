
using System;

namespace RobotBrain {
    
    public abstract class SyntaxTree {
        
        private SyntaxTree () { }

        public abstract string show ();


        public sealed class SyntaxError : SyntaxTree
        {
            //public readonly int position;
            //public readonly Token cause;
            public readonly string message;

            public SyntaxError () {
                this.message = "oops";
            }

            public SyntaxError (string msg) {
                this.message = msg;
            }

            public SyntaxError (int pos, Token tok, string msg) {
                //this.position = pos;
                //this.cause = tok;
                this.message = msg;
            }

            public override string show () {
                return $"(Error: {message})";
            }
        }


        public sealed class ContinueExpr : SyntaxTree
        {
            public readonly SyntaxTree lexpr;
            public readonly SyntaxTree cexpr;

            public ContinueExpr(SyntaxTree lexpr, SyntaxTree cexpr)
            {
                this.lexpr = lexpr;
                this.cexpr = cexpr;
            }

            public override string show()
            {
                return $"(Continue {lexpr.show()} {cexpr.show()})";
            }
        }


        public sealed class LetExpr : SyntaxTree
        {
            public readonly Identifier lhs;
            public readonly SyntaxTree rhs;

            public LetExpr(Identifier lhs, SyntaxTree rhs)
            {
                this.lhs = lhs;
                this.rhs = rhs;
            }

            public override string show()
            {
                return $"(Let {lhs.name} {rhs.show()})";
            }
        }


        public sealed class IdentifierExpr : SyntaxTree
        {
            public readonly Identifier ident;

            public IdentifierExpr (Identifier ident) {
                this.ident = ident;
            }

            public override string show () {
                return $"{ident.name}";
            }
        }


        public sealed class IntLiteralExpr : SyntaxTree
        {
            public readonly int val;

            public IntLiteralExpr (int val) {
                this.val = val;
            }

            public override string show () {
                return $"{val}";
            }
        }


        public sealed class QuitExpr : SyntaxTree
        {
            public QuitExpr () { }

            public override string show () {
                return "Quit";
            }
        }


        public sealed class StopExpr : SyntaxTree
        {
            public StopExpr() { }

            public override string show()
            {
                return "Stop";
            }
        }


        public sealed class HelpExpr : SyntaxTree {
            public HelpExpr () { }

            public override string show () {
                return "Help";
            }
        }


        public sealed class EchoExpr : SyntaxTree
        {
            public readonly Identifier ident;

            public EchoExpr (Identifier ident) {
                this.ident = ident;
            }

            public override string show () {
                return $"(Echo {ident.name})";
            }
        }


        public sealed class LandmarkExpr : SyntaxTree
        {
            public LandmarkExpr () { }

            public override string show () {
                return "Landmark";
            }
        }



        public sealed class RotateExpr : SyntaxTree
        {
            public readonly SyntaxTree angleExpr;

            public RotateExpr (SyntaxTree expr) {
                this.angleExpr = expr;
            }

            public override string show () {
                return $"(Rotate {angleExpr.show ()})";
            }
        }


        public sealed class MoveExpr : SyntaxTree
        {
            public readonly SyntaxTree distanceExpr;

            public MoveExpr(SyntaxTree expr)
            {
                this.distanceExpr = expr;
            }

            public override string show()
            {
                return $"(Move {distanceExpr.show()})";
            }
        }


        public sealed class BuyExpr : SyntaxTree
        {
            public readonly SyntaxTree commodityId;
            public readonly SyntaxTree countExpr;

            public BuyExpr (SyntaxTree num, SyntaxTree count) {
                this.commodityId = num;
                this.countExpr = count;
            }

            public override string show () {
                return $"(Buy {commodityId.show ()} {countExpr.show ()})";
            }
        }

        public sealed class SellExpr : SyntaxTree
        {
            public readonly SyntaxTree commodityId;
            public readonly SyntaxTree countExpr;

            public SellExpr(SyntaxTree num, SyntaxTree count)
            {
                this.commodityId = num;
                this.countExpr = count;
            }

            public override string show()
            {
                return $"(Sell {commodityId.show ()} {countExpr.show()})";
            }
        }

        public sealed class InventoryExpr : SyntaxTree
        {
            public InventoryExpr () { }

            public override string show () {
                return "Inventory";
            }
        }

        public sealed class MarketPricesExpr : SyntaxTree
        {
            public MarketPricesExpr() { }

            public override string show() {
                return "MarketPrices";
            }
        }

        public sealed class CityPricesExpr : SyntaxTree
        {
            public CityPricesExpr() { }

            public override string show() {
                return "CityPrices";
            }
        }

    }
    
}
