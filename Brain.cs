using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RobotBrain {

    public class Brain
    {
        private Hashtable variables;

        private Queue<BrainCommand> commandQueue;

        public Brain () {
            this.variables = new Hashtable ();
            this.commandQueue = new Queue<BrainCommand> ();
        }


        public void processTree (SyntaxTree tree) {
            XList commands = Backend.convert(tree);

            foreach (var cmd in commands.toList())
            {
                switch (cmd)
                {
                    case BrainCommand.BrainLet letCmd:
                        variables[letCmd.lhs.name] = letCmd.rhs;
                        break;

                    case BrainCommand.BrainEval evalCmd:
                        SyntaxTree evalTree =
                            (SyntaxTree) variables[evalCmd.ident.name];
                        processTree (evalTree);
                        break;

                    default:
                        commandQueue.Enqueue (cmd);
                        break;
                }
            }
        }

        public void processLine (string cmdLine) {
            SyntaxTree tree = Parser.parseLine (cmdLine);
            processTree (tree);
        }


        public bool hasCommands () {
            return commandQueue.Count > 0;
        }

        public BrainCommand nextCommand () {
            return commandQueue.Dequeue ();
        }
    }

}
