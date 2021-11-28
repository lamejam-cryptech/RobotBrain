using System;

namespace RobotBrain
{
    class Program
    {
        static void Main(string[] args)
        {
            Brain brain = new Brain ();

            while (true)
            {
                string input = Console.ReadLine ();
                input += '\n';

                brain.processLine (input);

                while (brain.hasErrors ())
                    Console.WriteLine (brain.nextError ());

                while (brain.hasStdOut ())
                    Console.WriteLine (brain.nextStdOut ());

                while (brain.hasCommands ())
                    Console.WriteLine (brain.nextCommand ().show ());
            }
        }
    }
}
