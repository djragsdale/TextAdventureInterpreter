using System;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;

namespace TextAdventureInterpreter
{
    class AdventureRoom
    {
        public string[,] roomObjects = new string[4, 2];

        public AdventureRoom()
        {
            this.roomObjects[0, 0] = "knife";
            this.roomObjects[0, 1] = "That knife looks pretty sharp.";
            this.roomObjects[1, 0] = "window";
            this.roomObjects[1, 1] = "The window has an okay view, but no foxes in the fern.";
            this.roomObjects[2, 0] = "chest";
            this.roomObjects[2, 1] = "It could be full of gold. Or dust.";
            this.roomObjects[3, 0] = "door";
            this.roomObjects[3, 1] = "The door appears to be locked...apparently...";
        }

        public List<string> listContents()
        {
            List<string> contents = new List<string>();

            for (int i = 0; i < this.roomObjects.GetLength(0); i++)
            {
                contents.Add(this.roomObjects[i, 0]);
            }

            return contents;
        }
    }

    static class AdventureCommand
    {
        public static int add(int int1, int int2)
        {
            Console.WriteLine("Add {0} to {1}", int1, int2);
            return (int1 + int2);
        }

        public static int subtract(int int1, int int2)
        {
            Console.WriteLine("Subtract {0} from {1}", int2, int1);
            return (int1 - int2);
        }

        public static string examine(string objectName)
        {
            AdventureRoom newRoom = new AdventureRoom();
            string result = objectName + " is not found. Please reformat your request and try again.";
            for (int i = 0; i < newRoom.listContents().Count; i++)
            {
                if (newRoom.roomObjects[i, 0].Equals(objectName, StringComparison.Ordinal))
                {
                    result = newRoom.roomObjects[i, 1];
                }
            }
            return result;
        }


    }

    class Pause
    {
        Timer pauseTimer = new Timer();
        public bool asleep = false;
        private string messageString;
        private double secondsDouble;

        //private delegate void PauseHandler(object sender, EventArgs e);                           //class-level variable

        public Pause(double seconds, string message)
        {
            asleep = true;
            //PauseArgs myPauseArgs = new PauseArgs("testing");                                     //lambda expression
            EventArgs e = new EventArgs();                                                          //delegates
            secondsDouble = seconds;
            double inMilliseconds = secondsDouble * 1000;
            messageString = message;                                                                //class-level variable
            pauseTimer.Interval = inMilliseconds;
            //PauseHandler pauseHandler = new PauseHandler(pauseTimer_Tick);                        //class-level variable
            //pauseTimer.Elapsed += pauseHandler;                                                   //class-level variable
            //pauseTimer.Elapsed += (sender, args) => pauseTimer_Tick(pauseTimer, myPauseArgs);     //lambda expression
            pauseTimer.Elapsed += delegate { pauseTimer_Tick(pauseTimer, e, message); };            //delegates
            pauseTimer.Start();
            while (asleep == true)
            {
            }
        }

        public Pause(Pause anotherPause)
        {
            Pause newPause = new Pause(anotherPause.getSeconds(), anotherPause.getMessage());
        }

        private void pauseTimer_Tick(Timer sender, EventArgs e, string message)                     //delegates
        {
            Console.WriteLine(message);
            //pauseTimer.Stop();
            sender.Stop();
            asleep = false;
        }

        //private void pauseTimer_Tick(Timer sender, PauseArgs e)                                   //lambda expression
        //{
        //    Console.WriteLine(e.messageString);
        //    //pauseTimer.Stop();
        //    sender.Stop();
        //    asleep = false;
        //}

        //private class PauseArgs : EventArgs                                                       //lambda expression
        //{
        //    public string messageString;

        //    public PauseArgs(string message)
        //    {
        //        messageString = message;
        //    }
        //}

        public string getMessage()
        {
            return messageString;
        }

        public double getSeconds()
        {
            return secondsDouble;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            int runCount = 0;
            Console.WriteLine("Tick...");
            Pause pause2 = new Pause(1, "tock...");
            Pause pause3 = new Pause(1, "tick...");
            Pause pause4 = new Pause(1, "tock.");
            Pause pause5 = new Pause(1, "");
            Console.WriteLine("You appear in a locked room. You can see a knife, window, chest, and door.");

            while (running == true)
            {
                runCount++;
                if (runCount > 1)
                {
                    Console.WriteLine("You are still in the room what would you like to do?");
                }
                string myCommand = Console.ReadLine();
                object[] parameters = myCommand.Split(' ');
                if (parameters.Length == 3)
                {
                    Type type = typeof(AdventureCommand);
                    MethodInfo info = type.GetMethod(parameters[0].ToString());
                    Console.WriteLine("Thank you, your result is coming right up!");
                    try
                    {
                        Console.WriteLine("{0}", info.Invoke(null, new object[] { Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]) }));
                    }
                    catch
                    {
                        Console.WriteLine("Command not found. Please reformat your request and try again.");
                    }
                }
                else if (parameters.Length == 2)
                {
                    Type type = typeof(AdventureCommand);
                    MethodInfo info = type.GetMethod(parameters[0].ToString());
                    Console.WriteLine("Your request is being processed.");
                    try
                    {
                        Console.WriteLine("{0}", info.Invoke(null, new object[] { parameters[1].ToString() }));
                    }
                    catch
                    {
                        Console.WriteLine("I'm sorry, please reformat your request and try again.");
                    }
                }
                else if (parameters.Length == 1)
                {
                    if (myCommand.Equals("exit", StringComparison.Ordinal))
                    {
                        Console.WriteLine("Thank you for playing TextAdventure...Interpreter...");
                        running = false;
                    }
                    else
                    {
                        Console.WriteLine("That didn't work, please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Syntax error...system fault...stack overflow...WHAT DID YOU DO?????");
                }

                if (runCount > 10)
                {
                    Console.WriteLine("Thank you for playing TextAdventure...Interpreter...");
                    running = false;
                }
            }
        }
    }
}
