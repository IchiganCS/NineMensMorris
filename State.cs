using System.Linq;

public enum Color
{
    None, White, Black
}

public enum Phase
{
    Place, Move, Jump
}

public class State
{
    public Color[] Pieces { get; init; } = new Color[15];
    public Phase Phase { get; set; }

    public bool WhiteToMove { get; set; }
    public bool BlackToMove {
        get => !WhiteToMove;
        set => WhiteToMove = !value;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>None if no winner has been found yet</returns>
    public Color DetermineWinner() {
        if (Pieces.Count(x => x == Color.White) < 3)
            return Color.Black;
        else if (Pieces.Count(x => x == Color.Black) < 3)
            return Color.White;

        return Color.None;
    }

    public State[] GenerateNextStates() {
        
    }
}