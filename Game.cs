using System;

public class Game
{
    public static void Main(string[] args)
    {

        State current = new();
        Console.WriteLine(current.ToString());
        for (int i = 0; i < 18; i++)
        {
            current.ExecuteAction();
            Console.WriteLine(current.ToString());
        }

    }
}