using System;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;

namespace TextAdventureInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            AdventureCommandCatalog commandCatalog = new AdventureCommandCatalog();

            bool running = true;
            int runCount = 0;
            Console.WriteLine("Tick...");
            Pause pause2 = new Pause(1, "tock...");
            Pause pause3 = new Pause(1, "tick...");
            Pause pause4 = new Pause(1, "tock.");
            Pause pause5 = new Pause(1);
            Console.WriteLine("You appear in a locked room. You can see a knife, window, chest, and door.");
            Console.Write("Command:>");
                    
            while (running == true)
            {
                runCount++;

                if (runCount > 1)
                {
                    Console.WriteLine("");
                    Console.WriteLine("You are still in the room what would you like to do?");
                    Console.Write("Command:>");
                }
                string myCommand = Console.ReadLine().ToLower();

                Console.WriteLine(commandCatalog.executeCommand(myCommand, ref running));
                Console.Write("");

                if (runCount > 10)
                {
                    commandCatalog.Dispose(ref running);
                }
            }
        }
    }
}
