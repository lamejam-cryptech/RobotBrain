﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RobotBrain {

    public class Brain
    {
        public const string helpMessage
            = "help\n"
            + " -> list available commands\n"
            + "let name = expr\n"
            + " -> save an expression\n"
            + "move (name|number)\n"
            + " -> move a certain distance\n"
            + "rotate (name|number)\n"
            + " -> rotate by a certain angle\n";

        private Dictionary<string, SyntaxTree> variables;

        private bool endGame;

        public bool shouldEndGame() {
            return endGame;
        }


        private Queue<string>       errorQueue;
        
        public bool hasErrors() {
            return errorQueue.Count > 0;
        }

        public string nextError() {
            return errorQueue.Dequeue();
        }

        public string peekError() {
            return errorQueue.Peek();
        }


        private Queue<string>       stdOut;

        public bool hasStdOut() {
            return stdOut.Count > 0;
        }

        public string nextStdOut() {
            return stdOut.Dequeue();
        }

        public string peekStdOut() {
            return stdOut.Peek();
        }


        private Queue<MechCommand>  commandQueue;

        public bool hasCommands(){
            return commandQueue.Count > 0;
        }

        public MechCommand nextCommand(){
            return commandQueue.Dequeue();
        }

        public MechCommand peekCommand(){
            return commandQueue.Peek();
        }


        public Brain () {
            this.variables = new Dictionary<string, SyntaxTree> ();

            this.endGame = false;

            this.errorQueue     = new Queue<string> ();
            this.stdOut         = new Queue<string> ();
            this.commandQueue   = new Queue<MechCommand> ();
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

                case BrainCommand.BrainHelp helpCmd:
                    stdOut.Enqueue (helpMessage);
                    break;

                case BrainCommand.BrainEcho echoCmd:
                    stdOut.Enqueue (variables[echoCmd.ident.name].show ());
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
        
    }

}
