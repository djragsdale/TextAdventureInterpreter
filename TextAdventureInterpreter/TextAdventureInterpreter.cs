﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;


class GlobalState
{
    private AdventureRoom currentAdventureRoom;
    //private string currentRoom;
    private int currentRoomState;
    private string exitMessage;
    private string username;
    private string saveName;

    public GlobalState()
    {
    }

    public GlobalState(AdventureRoom room)
    {
        currentAdventureRoom = room;
    }

    public string CurrentRoom
    {
        get
        {
            return currentAdventureRoom.ToString();
        }
        set
        {
            try
            {
                currentAdventureRoom = new AdventureRoom(value, currentRoomState);
            }
            catch
            {
                currentAdventureRoom = new AdventureRoom(value, 0);
            }
        }
    }

    public int CurrentRoomState
    {
        get
        {
            return currentRoomState;
        }
        set
        {
            try
            {
                currentAdventureRoom.roomState = value;
            }
            catch
            {
                currentRoomState = value;
            }
        }
    }

    public string ExitMessage
    {
        get
        {
            return exitMessage;
        }
        set
        {
            exitMessage = value;
        }
    }

    public string Username
    {
        get
        {
            return username;
        }
        set
        {
            username = value;
        }
    }

    public string SaveName
    {
        get
        {
            return saveName;
        }
        set
        {
            saveName = value;
        }
    }

    public void setRoom(AdventureRoom room)
    {
        currentAdventureRoom = room;
    }
}

class AdventureRoom
{
    private string roomName;
    public int roomState;
    public string[] stateDescription;
    public string[,] roomObjects = new string[4, 2];

    public AdventureRoom(string name, int state)
    {
        roomName = name;
        roomState = state;

        loadStates(name);
        loadObjects(name);
    }

    public string getDescription(int state)
    {
        return stateDescription[state];
    }

    private void loadStates(string name)
    {
        //dummy data until model is working
        stateDescription = new string[1]; //This should do a 'select count' to instantiate
        stateDescription[0] = "You appear in a locked room. You can see a knife, window, chest, and door.";
    }

    private void loadObjects(string name)
    {
        roomObjects[0, 0] = "knife";
        roomObjects[0, 1] = "That knife looks pretty sharp.";
        roomObjects[1, 0] = "window";
        roomObjects[1, 1] = "The window has an okay view, but no foxes in the fern.";
        roomObjects[2, 0] = "chest";
        roomObjects[2, 1] = "It could be full of gold. Or dust.";
        roomObjects[3, 0] = "door";
        roomObjects[3, 1] = "The door appears to be locked...apparently...";
    }
    
    public List<string> listContents()
    {
        List<string> contents = new List<string>();

        for (int i = 0; i < roomObjects.GetLength(0); i++)
        {
            contents.Add(roomObjects[i, 0]);
        }

        return contents;
    }

    public void triggerStateChange(int state)
    {
        roomState = state;
    }

    public override string ToString()
    {
        return roomName;
    }
}

class AdventureCommandCatalog
{
    private GlobalState globalState = new GlobalState();
    private List<AdventureCommand> commandList = new List<AdventureCommand>();

    public AdventureCommandCatalog(string exitMessage)
    {
        globalState.ExitMessage = exitMessage;
        addDummyCommands();
    }

    public void addCommand(AdventureCommand command)
    {
        commandList.Add(command);
    }

    private bool commandExists(string command)
    {
        bool result = false;
        foreach (AdventureCommand advCmd in commandList)
        {
            if (String.Compare(advCmd.getName(), command) == 0)
            {
                result = true;
            }
        }
        return result;
    }

    private AdventureCommand getCommand(string command)
    {
        //search for command
        AdventureCommand tempCommand = new AdventureCommand(command);
        foreach (AdventureCommand advCmd in commandList)
        {
            if (String.Compare(advCmd.getName(), command) == 0)
            {
                tempCommand = advCmd;
            }
        }
        return tempCommand;
    }

    private void addDummyCommands()
    {
        AdventureCommand take = new AdventureCommand("take");
        take.addObject("knife", "Knife added to inventory.");
        take.addObject("chest", "Chest is too heavy to take.");
        AdventureCommand open = new AdventureCommand("open");
        open.addObject("chest", "Chest contains two gold coins.");
        open.addObject("door", "The door is locked.");
        open.addObject("window", "The window opens just big enough for a person to squeeze through."); //This should trigger a requirement change.
        AdventureCommand examine = new AdventureCommand("examine");
        examine.addObject("knife", "That knife looks pretty sharp.");
        examine.addObject("window", "The window has an okay view, but no foxes in the fern.");
        examine.addObject("chest", "It could be full of gold. Or dust.");
        examine.addObject("door", "The door appears to be locked...apparently...");
        AdventureCommand go = new AdventureCommand("go");
        go.addObject("door", "A door has to be open to be used.");
        AdventureCommand exit = new AdventureCommand("exit");
        exit.addObject("door", "A door has to be open to be used.");
        exit.addObject("window", "You squeeze through the window."); //only if requirement is met. a trigger should go off to enter the next AdventureRoom

        commandList.Add(take);
        commandList.Add(open);
        commandList.Add(examine);
        commandList.Add(go);
        commandList.Add(exit);
    }

    public string executeCommand(string command, ref bool falseToClose)
    {
        string result = "Command not found. Please reformat your request and try again.";

        //object[] parameters = command.Split(' ');   //old style
        string[] parameters = command.Split(' ');

        if (parameters.Length == 0)
        {
            result = "Syntax error...system fault...stack overflow...WHAT DID YOU DO?????";
        }
        else if (parameters.Length == 1)
        {
            //invoke a method instead of searching for commands
            Type type = typeof(AdventureCommand);
            MethodInfo info = type.GetMethod(parameters[0]);
            try
            {
                result = info.Invoke(null, new object[1] { globalState } ).ToString();
            }
            catch
            {
                if (String.Compare(command, "exit", StringComparison.Ordinal) == 0)
                {
                    Dispose(ref falseToClose);
                }
                else
                {
                    result = "That didn't work, please try again.";
                }
            }
            //The old style
            //Type type = typeof(AdventureCommand);
            //MethodInfo info = type.GetMethod(parameters[0].ToString());
            //try
            //{
            //    Console.WriteLine("{0}", info.Invoke(null, new object[] { parameters[1].ToString() }));
            //    Console.WriteLine("");
            //}
        }
        else
        {
            string[] commandArgs = new string[parameters.Length - 1];
            for (int i = 1; i < parameters.Length; i++)
            {
                commandArgs[i - 1] = parameters[i];
            }

            try
            {
                if (commandExists(parameters[0]))
                {
                    AdventureCommand tempCommand = getCommand(parameters[0]);
                    result = tempCommand.execute(commandArgs);
                }
            }
            catch
            {
                result = "Command not found. Please reformat your request and try again.";
            }
            //The old style
            //Type type = typeof(AdventureCommand);
            //MethodInfo info = type.GetMethod(parameters[0].ToString());
            //try
            //{
            //    Console.WriteLine("{0}", info.Invoke(null, new object[] { parameters[1].ToString() }));
            //    Console.WriteLine("");
            //}
            //catch
            //{
            //    Console.WriteLine("I'm sorry, please reformat your request and try again.");
            //    Console.WriteLine("");
            //}
        }

        return result;
    }

    public void Dispose(ref bool trueToFalse)
    {
        Console.WriteLine("");
        Console.WriteLine(globalState.ExitMessage);
        Pause pause = new Pause(3);
        trueToFalse = false;
    }
    //overload for custom message
    public void Dispose(ref bool trueToFalse, string exitMessage)
    {
        Console.WriteLine("");
        Console.WriteLine(exitMessage);
        Pause pause = new Pause(3);
        trueToFalse = false;
    }

    //NOT FINISHED YET!!!!!
    private bool RoomExists(string name)
    {
        bool result = true;
        return result;
    }
}

class AdventureCommand
{
    private List<string[]> actsOn = new List<string[]>();

    private string name;

    public AdventureCommand(string commandName)
    {
        name = commandName;
    }

    ////originally part of the parser test
    //public static int add(int int1, int int2)
    //{
    //    Console.WriteLine("Add {0} to {1}", int1, int2);
    //    return (int1 + int2);
    //}

    //public static int subtract(int int1, int int2)
    //{
    //    Console.WriteLine("Subtract {0} from {1}", int2, int1);
    //    return (int1 - int2);
    //}

    ////THIS IS DEPRECATED.
    //public string examine(string objectName)
    //{
    //    //how does the state get passed to here?
    //    AdventureRoom newRoom = new AdventureRoom("room", 0);
    //    string result = objectName + " is not found. Please reformat your request and try again.";
    //    for (int i = 0; i < newRoom.listContents().Count; i++)
    //    {
    //        if (String.Compare(newRoom.roomObjects[i, 0], objectName, StringComparison.Ordinal) == 0)
    //        {
    //            result = newRoom.roomObjects[i, 1];
    //        }
    //    }
    //    return result;
    //}

    //has not been added
    public string save(GlobalState globalState)
    {
        //Where do we put the name to save?
        string result = "Game saving is not configured yet."; //Later this should actually save the game
        return result;
    }

    public string look(GlobalState globalState)
    {
        string result = "Either nothing is visible or you are nowhere.";
        try
        {
            AdventureRoom newRoom = new AdventureRoom(globalState.CurrentRoom, globalState.CurrentRoomState); //[0] is name, [1] is state
            result = newRoom.getDescription(globalState.CurrentRoomState);
        }
        catch
        {

        }
        return result;
    }

    public void addObject(string objectToActOn, string message)
    {
        actsOn.Add(new string[2] { objectToActOn, message });
    }

    public string execute(string[] commandArgs)
    {
        //should use CommandArgs, a list of the other words in the command
        string result = "Command not prepared properly. Please reformat your request and try again.";
        foreach (string[] args in actsOn)
        {
            if (String.Compare(args[0], commandArgs[0]) == 0)
            {
                result = args[1];
            }
        }
        return result;
    }

    public string getName()
    {
        return name;
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

    public Pause(double seconds)
    {
        asleep = true;
        EventArgs e = new EventArgs();
        secondsDouble = seconds;
        double inMilliseconds = secondsDouble * 1000;
        messageString = "";
        pauseTimer.Interval = inMilliseconds;
        pauseTimer.Elapsed += delegate { pauseTimer_Tick(pauseTimer, e, messageString); };
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