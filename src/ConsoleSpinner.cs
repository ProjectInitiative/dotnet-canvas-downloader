using System;
using System.Threading;

public class ConsoleSpinner
{
    private bool isTaskDone = false;
    public ConsoleSpinner() {}
    public void Wait()
    {
        Console.CursorVisible = false;
        int counter = 0;
        isTaskDone = false;
        while(!isTaskDone)
        {
            switch (counter)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            if (counter < 3)
                counter++;
            else 
                counter = 0;
            Thread.Sleep(150);
        }
        Console.Write("");
        Console.CursorVisible = true;
    }
    public bool IsTaskDone { get => isTaskDone; set { isTaskDone = value; } }
}