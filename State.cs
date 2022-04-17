public enum Piece
{
    None, White, Black
}

public enum Phase
{
    Place, Move, Jump
}

public class State
{
    public Piece[] Pieces { get; init; } = new Piece[15];
    
}