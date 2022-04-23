using System.Collections.Generic;
using System.Linq;

public class Game
{
    public static void Main(string[] args)
    {
        State current = new();
        List<State> lower = new();
        lower.Add(current);
        for (int i = 0; i < 5; i++) {
            List<State> newLs = new();
            lower.ForEach(x => newLs.AddRange(x.GenerateNextStates()));
            lower = newLs;
            Console.WriteLine($"Iteration {i}: {lower.Count} possibilities");
        }
        Console.WriteLine(lower.Where(x => x.BlackPiecesCount == 8).Count());
        Console.WriteLine(State.MILLS.Length);
        Console.WriteLine($"The winner is {current.DetermineWinner()}");
    }
}