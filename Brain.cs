using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RobotBrain {

    public class Brain
    {
        private Dictionary<string, SyntaxTree> variables;

        private bool endGame;

        private Queue<string> errorQueue;
        private Queue<MechCommand> commandQueue;

        public Brain () {
            this.variables = new Dictionary<string, SyntaxTree> ();

            this.endGame = false;

            this.errorQueue = new Queue<string> ();
            this.commandQueue = new Queue<MechCommand> ();
        }


        public void processTree (SyntaxTree tree) {
            XList commands = Backend.convert(tree);

            foreach (var cmd in commands.toList ())
                processBrainCommand (cmd);
        }

        public void processBrainCommand (BrainCommand cmd) {
            switch (cmd) {
                case BrainCommand.BrainError errCmd:
                    errorQueue.Enqueue (errCmd.show ());
                    break;

                case BrainCommand.BrainSyntaxError syntaxErrCmd:
                    errorQueue.Enqueue (syntaxErrCmd.show ());
                    break;

                case BrainCommand.BrainQuit quitCmd:
                    this.endGame = true;
                    break;

                case BrainCommand.BrainStop stopCmd:
                    commandQueue.Clear ();
                    commandQueue.Enqueue (new MechCommand.MechStop ());
                    break;

                case BrainCommand.BrainLet letCmd:
                    variables[letCmd.lhs.name] = letCmd.rhs;
                    break;

                case BrainCommand.BrainEval evalCmd:
                    SyntaxTree evalTree = variables[evalCmd.ident.name];
                    processTree(evalTree);
                    break;

                case BrainCommand.BrainRotate rotCmd:
                    if (evalExpr(rotCmd.angleExpr) is int rotAngle) {
                        MechCommand mechRotCmd = new MechCommand.MechRotate
                            (rotAngle);
                        commandQueue.Enqueue(mechRotCmd);
                    }
                    break;

                case BrainCommand.BrainMove movCmd:
                    if (evalExpr(movCmd.distanceExpr) is int movDist) {
                        MechCommand mechMovCmd = new MechCommand.MechMove
                            (movDist);
                        commandQueue.Enqueue(mechMovCmd);
                    }
                    break;
            }
        }

        // Evaluate a syntax tree to a literal.
        //
        public int? evalExpr (SyntaxTree expr) {
            switch (expr) {
                case SyntaxTree.IdentifierExpr idExpr:
                    return evalExpr (variables[idExpr.ident.name]);

                case SyntaxTree.IntLiteralExpr litExpr:
                    return litExpr.val;

                default:
                    errorQueue.Enqueue
                        ( "error: "
                        + $"could not evaluate {expr.show ()} to literal" );
                    return null;
            }
        }

        public void processLine (string cmdLine) {
            SyntaxTree tree = Parser.parseLine (cmdLine);
            processTree (tree);
        }


        public bool hasErrors () {
            return errorQueue.Count > 0;
        }

        public string nextError () {
            return errorQueue.Dequeue ();
        }

        public string peekError () {
            return errorQueue.Peek ();
        }


        public bool shouldEndGame () {
            return endGame;
        }


        public bool hasCommands () {
            return commandQueue.Count > 0;
        }

        public MechCommand nextCommand () {
            return commandQueue.Dequeue ();
        }

        public MechCommand peekCommand () {
            return commandQueue.Peek ();
        }
    }

}
