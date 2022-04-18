using System;

public class Game
{
    public static void Main(string[] args)
    {

        State current = new();
        Console.WriteLine(current.ToString());
        while (current.DetermineWinner() == Color.None)
        {
            current.ExecuteAction();
            Console.WriteLine(current.ToString());
        }
        Console.WriteLine($"The winner is {current.DetermineWinner()}");

    }
}