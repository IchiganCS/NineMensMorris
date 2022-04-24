using System.Collections.Generic;
using System.Linq;

public class Game
{
    public static void Main(string[] args)
    {
        State current = new();
        while (current.DetermineWinner() == Color.None)
        {
            Console.WriteLine(current);
            //bot       
            if (current.BlackToMove)
            {         
                current = new StateTree(current, 5).BuildAndEvaluate();
                continue;
            }
            //player
            while (current.WhiteToMove)
            {
                State copy = current.Clone();
                List<State> valids = current.GenerateNextStates();
                try
                {
                    int start = 0, end = 0;
                    if (current.Phase == Phase.Place)
                    {
                        Console.Write("Place piece at: ");
                        end = Convert.ToInt32(Console.ReadLine());
                        copy.Pieces[end] = Color.White;
                        copy.WhiteToMove = false;
                        copy.WhitePiecesToPlace--;
                    }
                    else if (current.Phase == Phase.Play) 
                    {
                        Console.Write("Move piece from: ");
                        start = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Set piece to: ");
                        end = Convert.ToInt32(Console.ReadLine());

                        copy.Pieces[start] = Color.None;
                        copy.Pieces[end] = Color.White;
                        copy.WhiteToMove = false;
                    }
                    if (copy.CheckMill(end)) {
                        Console.Write("Take piece from: ");
                        int pos = Convert.ToInt32(Console.ReadLine());
                        copy.Pieces[pos] = Color.None;
                    }
                    if (!valids.Contains(copy))
                    {
                        copy = current.Clone();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR!");
                    Console.WriteLine(e.StackTrace);
                }
                current = copy;
            }
            
        }
        Console.WriteLine(current);
        Console.WriteLine($"The winner is {current.DetermineWinner()}");
    }
}