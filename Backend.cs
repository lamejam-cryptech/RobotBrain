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
    }


    public class Backend
    {
        public static XList walkTreeConvert
            ( SyntaxTree tree, XList con1 )
        {
            switch (tree) {
                case SyntaxTree.ContinueExpr expr:
                    XList con2 = walkTreeConvert (expr.cexpr, con1);
                    XList con3 = walkTreeConvert (expr.lexpr, con2);
                    return con3;

                case SyntaxTree.LetExpr expr: {
                    BrainCommand cmd = new BrainCommand.BrainLet
                        (expr.lhs, expr.rhs);
                    return new XList.Cons (cmd, con1);
                }

                case SyntaxTree.IdentifierExpr expr: {
                    BrainCommand cmd = new BrainCommand.BrainEval
                        (expr.ident);
                    return new XList.Cons (cmd, con1);
                }

                case SyntaxTree.HelpExpr expr:
                    return new XList.Cons
                        ( new BrainCommand.BrainHelp (), con1 );

                case SyntaxTree.RotateExpr expr: {
                    BrainCommand cmd = new BrainCommand.BrainRotate
                        (expr.angleExpr);
                    return new XList.Cons (cmd, con1);
                }

                case SyntaxTree.MoveExpr expr: {
                    BrainCommand cmd = new BrainCommand.BrainMove
                        (expr.distanceExpr);
                    return new XList.Cons (cmd, con1);
                }


                default:
                    return null;
            }
        }

        public static XList convert (SyntaxTree tree) {
            return walkTreeConvert (tree, new XList.Nil ());
        }
    }

}
