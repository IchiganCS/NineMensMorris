using System.Linq;
using System.Collections.Generic;
using System;

public sealed class State : IEquatable<State>
{
    /// <summary>
    /// from Left to Right, Top to Bottom
    /// 0  -   1   -  2
    ///    3   4   5
    ///      6 7 8
    /// 9 10 11  12 13 14
    ///      15 16 17
    ///   18    19  20
    /// 21     22       23
    /// </summary>
    /// <value></value>
    public Color[] Pieces { get; set; } = new Color[24];
    public static readonly int[][] MILLS = new int[][]{
        new int[]{0, 1, 2},
        new int[]{0, 9, 21},
        new int[]{2, 14, 23},
        new int[]{21, 22, 23},
        new int[]{1, 4, 7},
        new int[]{9, 10, 11},
        new int[]{12, 13, 14},
        new int[]{16, 19, 22},
        new int[]{3, 4, 5},
        new int[]{3, 10, 18},
        new int[]{5, 13, 20},
        new int[]{18, 19, 20},
        new int[]{6, 7, 8},
        new int[]{6, 11, 15},
        new int[]{15, 16, 17},
        new int[]{8, 12, 17}
    };
    public static readonly int[][] CONNECTIONS = new int[][]{
        new int[]{0, 1}, new int[]{1, 2}, new int[]{2, 14}, new int[]{14, 23}, new int[]{23, 22}, new int[]{22, 21}, new int[]{21, 9}, new int[]{9, 0},
        new int[]{1, 4}, new int[]{9, 10}, new int[]{13, 14}, new int[]{19, 22},
        new int[]{3, 4}, new int[]{4, 5}, new int[]{5, 13}, new int[]{13, 20}, new int[]{20, 19}, new int[]{19, 18}, new int[]{18, 10}, new int[]{10, 3},
        new int[]{4, 7}, new int[]{12, 13}, new int[]{16, 19}, new int[]{10, 11},
        new int[]{6, 7}, new int[]{7, 8}, new int[]{8, 12}, new int[]{12, 17}, new int[]{17, 16}, new int[]{16, 15}, new int[]{15, 11}, new int[]{11, 6}
    };

    public int WhitePiecesToPlace { get; set; } = 9;
    public int BlackPiecesToPlace { get; set; } = 9;

    public int PiecesToPlace(Color color)
    {
        return color == Color.White ? WhitePiecesToPlace : BlackPiecesToPlace;
    }

    public Phase Phase
    {
        get => (WhitePiecesToPlace > 0 || BlackPiecesToPlace > 0) ? Phase.Place : Phase.Play;
    }

    public bool WhiteToMove { get; set; } = true;
    public bool BlackToMove
    {
        get => !WhiteToMove;
        set => WhiteToMove = !value;
    }

    public bool WhiteCanJump
    {
        get => WhitePiecesCount < 4;
    }

    public bool BlackCanJump
    {
        get => BlackPiecesCount < 4;
    }
    public bool CanJump(Color color)
    {
        return color == Color.White ? WhiteCanJump : BlackCanJump;
    }
    public int WhitePiecesCount
    {
        get => Pieces.Count(x => x == Color.White) + WhitePiecesToPlace;
    }
    public int BlackPiecesCount
    {
        get => Pieces.Count(x => x == Color.Black) + BlackPiecesToPlace;
    }
    public Color CurrentColor
    {
        get => WhiteToMove ? Color.White : Color.Black;
    }
    public Color EnemyColor
    {
        get => WhiteToMove ? Color.Black : Color.White;
    }

    public int PiecesCount(Color color)
    {
        return color == Color.White ? WhitePiecesCount : BlackPiecesCount;
    }

    private IEnumerable<int> PiecePostions(Color color)
    {
        return Enumerable.Range(0, Pieces.Length).Where(x => Pieces[x] == color);
    }
    private IEnumerable<int> EmptyPositions()
    {
        return Enumerable.Range(0, Pieces.Length).Where(x => Pieces[x] == Color.None);
    }

    private void RemovePiece(int pos)
    {
        Pieces[pos] = Color.None;
    }
    private void SetPiece(int pos, Color col)
    {
        Pieces[pos] = col;
    }

    /// <returns>None if no winner has been found yet</returns>
    public Color DetermineWinner()
    {
        if (PiecesCount(Color.White) < 3)
            return Color.Black;
        else if (PiecesCount(Color.Black) < 3)
            return Color.White;

        return Color.None;
    }


    public State Clone()
    {
        return new()
        {
            Pieces = Pieces.ToArray(),
            BlackPiecesToPlace = BlackPiecesToPlace,
            WhitePiecesToPlace = WhitePiecesToPlace,
            WhiteToMove = WhiteToMove
        };
    }

    public override bool Equals(object? obj)
        => Equals(obj as State);
    public bool Equals(State? state)
    {
        if (state is null)
            return false;
        return state.WhiteToMove == WhiteToMove &&
            state.BlackPiecesToPlace == BlackPiecesToPlace &&
            state.WhitePiecesToPlace == WhitePiecesToPlace &&
            Enumerable.Range(0, Pieces.Length).All(x => state.Pieces[x] == Pieces[x]);
    }
    public override int GetHashCode()
    {
        return Pieces.Sum(x => x == Color.None ? 1 : (x == Color.White ? 256 : 256 * 256)) << 7 + ((WhitePiecesCount + 10) * BlackPiecesCount) << (WhiteToMove ? 0 : 5);
    }


    /// <summary>
    /// Checks whether piecePosition lies in a mill
    /// </summary>
    /// <param name="piecePosition"></param>
    public bool CheckMill(int piecePosition)
    {
        if (Pieces[piecePosition] == Color.None)
            return false;

        return MILLS.Any(x => x.Contains(piecePosition) &&
            //just one color
            x.Select(x => Pieces[x]).Distinct().Count() == 1);
    }

    private List<State> GenerateTakeStates()
    {
        List<State> states = new();
        foreach (int x in PiecePostions(EnemyColor))
        {
            if (CheckMill(x))
                continue;

            State newState = Clone();
            newState.RemovePiece(x);
            states.Add(newState);
        }

        return states;
    }

    public List<State> GenerateNextStates()
    {
        List<State> states = new();
        if (Phase == Phase.Place)
        {
            foreach (int x in EmptyPositions())
            {
                State newState = Clone();
                newState.SetPiece(x, CurrentColor);
                if (CurrentColor == Color.White)
                    newState.WhitePiecesToPlace--;
                else
                    newState.BlackPiecesToPlace--;

                if (newState.CheckMill(x))
                    states.AddRange(newState.GenerateTakeStates());
                else
                    states.Add(newState);

            }
        }
        else if (Phase == Phase.Play)
        {
            foreach (int pos in PiecePostions(CurrentColor))
            {
                State newState = Clone();
                newState.RemovePiece(pos);
                foreach (int newPos in FindPossibleMoves(pos))
                {
                    State newerState = newState.Clone();
                    newerState.SetPiece(newPos, CurrentColor);

                    if (newerState.CheckMill(newPos))
                        states.AddRange(newerState.GenerateTakeStates());
                    else
                        states.Add(newerState);
                }
            }
        }

        //Todo: find working (and logical) way to achieve this
        return states.Select(x =>
        {
            x.WhiteToMove = !x.WhiteToMove;
            return x;
        }).ToList();
    }

    private IEnumerable<int> FindPossibleMoves(int pos)
    {
        if (!CanJump(CurrentColor))
            return CONNECTIONS.Where(x => x.Contains(pos))
                .Select(x => x.Where(y => y != pos).ToArray()[0])
                .Where(x => Pieces[x] == Color.None);
        else
            return EmptyPositions();
    }

    public float SingleEvaluate()
    {
        return WhitePiecesCount - BlackPiecesCount;
    }

    public override string ToString()
    {
        string res = $"{CurrentColor} to move\n";

        if (Phase == Phase.Place)
        {
            res += $"White to place: {WhitePiecesToPlace}, black to place: {BlackPiecesToPlace}\n";
        }
        if (Phase == Phase.Play)
        {
            if (WhiteCanJump)
                res += "White can jump\n";
            if (BlackCanJump)
                res += "Black can jump\n";
        }

        string field = @"
        0-------------1-------------2
        |             |             |
        |    3--------4--------5    |
        |    |        |        |    |
        |    |    6---7---8    |    |
        |    |    |       |    |    |
        9----10----11       12----13----14
        |    |    |       |    |    |
        |    |    15---16---17    |    |
        |    |        |        |    |
        |    18--------19--------20    |
        |             |             |
        21-------------22-------------23";


        Func<Color, string> colToString = (x) =>
        {
            if (x == Color.White) return "W";
            else if (x == Color.Black) return "B";
            return "O";
        };

        for (int i = 23; i >= 0; i--)
        {
            field = field.Replace(i.ToString(), colToString(Pieces[i]));
        }

        res += field;


        return res;
    }
}