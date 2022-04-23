using System.Collections.Generic;
using System.Linq;

public class Game
{
    public static void Main(string[] args)
    {
        State current = new();
        List<State> lower = new();
        lower.Add(current);
        for (int i = 0; i < 3; i++) {
            List<State> newLs = new();
            lower.ForEach(x => newLs.AddRange(x.GenerateNextStates()));
            lower = newLs.Distinct().ToList();
            Console.WriteLine($"Iteration {i}: {lower.Count} possibilities");
        }
        Console.WriteLine(lower.Where(x => x.BlackPiecesCount == 8).Count());
        Console.WriteLine(lower.Count(x => x.Evaluate() > 0));
        Console.WriteLine(lower.Count(x => x.Evaluate() < 0));
        lower.Where(x => x.Pieces[0] == Color.White && x.Pieces[1] == Color.Black).Distinct().ToList().ForEach(Console.WriteLine);
        Console.WriteLine($"The winner is {current.DetermineWinner()}");
    }
}