using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotBrain
{

    public abstract class XList
    {

        private XList () { }

        public abstract List<BrainCommand> toList ();

        public sealed class Nil : XList
        {
            public Nil () { }

            public override List<BrainCommand> toList () {
                return new List<BrainCommand> ();
            }
        }

        public sealed class Cons : XList
        {
            public BrainCommand data;
            public XList continuation;

            public Cons (BrainCommand cur, XList con)
            {
                this.data = cur;
                this.continuation = con;
            }

            public override List<BrainCommand> toList () {
                List<BrainCommand> list = new List<BrainCommand> ();
                list.Add (data);
                return list
                    .Concat (continuation.toList ())
                    .ToList ();
            }
        }

    }


    public abstract class BrainCommand
    {
        private BrainCommand () { }

        public abstract string show ();


        public sealed class BrainError : BrainCommand
        {
            public readonly string message;

            public BrainError (string msg) {
                this.message = msg;
            }

            public override string show () {
                return $"error: {message}";
            }
        }

        public sealed class BrainSyntaxError : BrainCommand
        {
            public readonly SyntaxTree.SyntaxError err;

            public BrainSyntaxError (SyntaxTree.SyntaxError err) {
                this.err = err;
            }

            public override string show () {
                return err.show ();
            }
        }

        public sealed class BrainQuit : BrainCommand
        {
            public BrainQuit () { }

            public override string show () {
                return "quit";
            }
        }

        public sealed class BrainStop : BrainCommand
        {
            public BrainStop () { }

            public override string show () {
                return "stop";
            }
        }

        public sealed class BrainLet : BrainCommand
        {
            public readonly Identifier lhs;
            public readonly SyntaxTree rhs;

            public BrainLet (Identifier lhs, SyntaxTree rhs) {
                this.lhs = lhs;
                this.rhs = rhs;
            }

            public override string show () {
                return $"let {lhs.name} = {rhs.show ()}";
            }
        }


        public sealed class BrainEval : BrainCommand
        {
            public readonly Identifier ident;

            public BrainEval (Identifier ident) {
                this.ident = ident;
            }

            public override string show () {
                return $"eval {ident.name}";
            }
        }


        public sealed class BrainHelp : BrainCommand
        {
            public BrainHelp () { }

            public override string show () {
                return $"help";
            }
        }


        public sealed class BrainEcho : BrainCommand
        {
            public readonly Identifier ident;

            public BrainEcho (Identifier ident) {
                this.ident = ident;
            }

            public override string show () {
                return $"echo {ident.name}";
            }
        }


        public sealed class BrainRotate : BrainCommand
        {
            public readonly SyntaxTree angleExpr;

            public BrainRotate (SyntaxTree expr) {
                this.angleExpr = expr;
            }

            public override string show () {
                return $"rotate {angleExpr.show ()}";
            }
        }


        public sealed class BrainMove : BrainCommand
        {
            public readonly SyntaxTree distanceExpr;

            public BrainMove(SyntaxTree expr)
            {
                this.distanceExpr = expr;
            }

            public override string show ()
            {
                return $"move {distanceExpr.show()}";
            }
        }


        public sealed class BrainBuy : BrainCommand
        {
            public readonly Identifier commodityName;
            public readonly SyntaxTree countExpr;

            public BrainBuy (Identifier name, SyntaxTree count) {
                this.commodityName = name;
                this.countExpr = count;
            }

            public override string show () {
                return $"buy {commodityName.name} {countExpr.show ()}";
            }
        }


        public sealed class BrainSell : BrainCommand
        {
            public readonly Identifier commodityName;
            public readonly SyntaxTree countExpr;

            public BrainSell(Identifier name, SyntaxTree count) {
                this.commodityName = name;
                this.countExpr = count;
            }

            public override string show () {
                return $"sell {commodityName.name} {countExpr.show()}";
            }
        }


        public sealed class BrainInventory : BrainCommand
        {
            public BrainInventory () { }

            public override string show () {
                return "inventory";
            }
        }

        public sealed class BrainMarketPrices : BrainCommand
        {
            public BrainMarketPrices () { }

            public override string show () {
                return "marketPrices";
            }
        }

        public sealed class BrainCityPrices : BrainCommand
        {
            public BrainCityPrices () { }

            public override string show () {
                return "cityPrices";
            }
        }

    }


    public class Backend
    {
        public static XList walkTreeConvert
            ( SyntaxTree tree, XList con1 )
        {
            switch (tree) {
                case SyntaxTree.SyntaxError errExpr:
                    BrainCommand syntaxError =
                        new BrainCommand.BrainSyntaxError (errExpr);
                    return new XList.Cons (syntaxError, con1);

                case SyntaxTree.QuitExpr quitExpr:
                    return new XList.Cons
                        (new BrainCommand.BrainQuit (), con1);

                case SyntaxTree.StopExpr stopExpr:
                    return new XList.Cons
                        (new BrainCommand.BrainStop(), con1);

                case SyntaxTree.ContinueExpr contExpr:
                    XList con2 = walkTreeConvert (contExpr.cexpr, con1);
                    XList con3 = walkTreeConvert (contExpr.lexpr, con2);
                    return con3;

                case SyntaxTree.LetExpr letExpr:
                    BrainCommand letCmd = new BrainCommand.BrainLet
                        (letExpr.lhs, letExpr.rhs);
                    return new XList.Cons (letCmd, con1);

                case SyntaxTree.IdentifierExpr identExpr:
                    BrainCommand evalCmd = new BrainCommand.BrainEval
                        (identExpr.ident);
                    return new XList.Cons (evalCmd, con1);

                case SyntaxTree.HelpExpr helpExpr:
                    return new XList.Cons
                        ( new BrainCommand.BrainHelp (), con1 );

                case SyntaxTree.EchoExpr echoExpr:
                    return new XList.Cons
                        ( new BrainCommand.BrainEcho (echoExpr.ident), con1 );

                case SyntaxTree.RotateExpr rotExpr:
                    BrainCommand rotCmd = new BrainCommand.BrainRotate
                        (rotExpr.angleExpr);
                    return new XList.Cons (rotCmd, con1);

                case SyntaxTree.MoveExpr movExpr:
                    BrainCommand movCmd = new BrainCommand.BrainMove
                        (movExpr.distanceExpr);
                    return new XList.Cons (movCmd, con1);

                case SyntaxTree.BuyExpr buyExpr:
                    BrainCommand buyCmd = new BrainCommand.BrainBuy
                        (buyExpr.commodityName, buyExpr.countExpr);
                    return new XList.Cons (buyCmd, con1);

                case SyntaxTree.SellExpr sellExpr:
                    BrainCommand sellCmd = new BrainCommand.BrainSell
                        (sellExpr.commodityName, sellExpr.countExpr);
                    return new XList.Cons (sellCmd, con1);

                case SyntaxTree.InventoryExpr invExpr:
                    return new XList.Cons
                        (new BrainCommand.BrainInventory (), con1);

                case SyntaxTree.MarketPricesExpr pricesExpr:
                    return new XList.Cons
                        (new BrainCommand.BrainMarketPrices (), con1);

                case SyntaxTree.CityPricesExpr pricesExpr:
                    return new XList.Cons
                        (new BrainCommand.BrainCityPrices (), con1);


                default:
                    BrainCommand err = new BrainCommand.BrainError
                        ("unrecognized command");
                    return new XList.Cons (err, con1);
            }
        }

        public static XList convert (SyntaxTree tree) {
            return walkTreeConvert (tree, new XList.Nil ());
        }
    }

}
