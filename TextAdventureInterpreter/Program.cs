using System;
using System.Collections.Generic;
using System.Reflection;

namespace TextAdventureInterpreter
{
    class AdventureRoom
    {
        public string[,] roomObjects = new string[4,2];

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

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please type your command.");
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
        }
    }
}
