using System.Collections.Generic;
using System.Linq;

public class Game
{
    public static void Main(string[] args)
    {

        State current = new();
        Console.WriteLine(current.ToString());
        List<State> lower = current.GenerateNextStates();
        for (int i = 0; i < 4; i++) {
            List<State> newLs = new();
            lower.ForEach(x => newLs.AddRange(x.GenerateNextStates()));
            lower = newLs;
        }
        lower.ForEach(Console.WriteLine);
        Console.WriteLine($"The winner is {current.DetermineWinner()}");
    }
}