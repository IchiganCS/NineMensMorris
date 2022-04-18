using System.Linq;
using System.Collections.Generic;

public enum Color
{
    None, White, Black
}

public enum Phase
{
    Place, Play
}

public class State
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
    private int[][] MILLS = new int[][]{
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
    private int[][] CONNECTIONS = new int[][]{
        new int[]{0, 1}, new int[]{1, 2}, new int[]{2, 14}, new int[]{14, 23}, new int[]{23, 22}, new int[]{22, 21}, new int[]{21, 9}, new int[]{9, 0},
        new int[]{1, 4}, new int[]{9, 10}, new int[]{13, 14}, new int[]{19, 22},
        new int[]{3, 4}, new int[]{4, 5}, new int[]{5, 13}, new int[]{13, 20}, new int[]{20, 19}, new int[]{19, 18}, new int[]{18, 10}, new int[]{10, 3},
        new int[]{4, 7}, new int[]{12, 13}, new int[]{16, 19}, new int[]{10, 11},
        new int[]{6, 7}, new int[]{7, 8}, new int[]{8, 12}, new int[]{12, 17}, new int[]{17, 16}, new int[]{16, 15}, new int[]{15, 11}, new int[]{11, 6}
    };

    public int WhiteStonesToPlace { get; set; } = 9;
    public int BlackStonesToPlace { get; set; } = 9;

    public Phase Phase { 
        get => (WhiteStonesToPlace > 0 || BlackStonesToPlace > 0) ? Phase.Place : Phase.Play; }

    public bool WhiteToMove { get; set; } = true;
    public bool BlackToMove
    {
        get => !WhiteToMove;
        set => WhiteToMove = !value;
    }

    public int WhitePiecesOnBoard()
    {
        return Pieces.Count(x => x == Color.White);
    }
    public int BlackPiecesOnBoard()
    {
        return Pieces.Count(x => x == Color.Black);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>None if no winner has been found yet</returns>
    public Color DetermineWinner()
    {
        if (Pieces.Count(x => x == Color.White) < 3)
            return Color.Black;
        else if (Pieces.Count(x => x == Color.Black) < 3)
            return Color.White;

        return Color.None;
    }

    public List<State> TakePiece(Color color)
    {
        IEnumerable<int> inMill = AllPiecesInMill();
        List<State> states = new();
        for (int i = 0; i < Pieces.Length; i++)
        {
            if (Pieces[i] != color || inMill.Contains(i))
                continue;
            State n = Clone();
            n.Pieces[i] = Color.None;
            states.Add(n);
        }
        return states;
    }

    public IEnumerable<int> AllPiecesInMill()
    {
        List<int> pieces = new();
        foreach (int[] mill in MILLS)
        {
            List<Color> p = new();
            foreach (int i in mill)
                p.Add(Pieces[i]);
            if (p.Distinct().ToArray().Length == 1 && p[0] != Color.None)
                pieces.AddRange(mill);
        }
        return pieces.Distinct();
    }

    public State Clone()
    {
        State x = new();
        x.Pieces = Pieces.ToArray();
        return x;
    }

    /// <summary>
    /// Checks whether piecePosition lies in a mill
    /// </summary>
    /// <param name="piecePosition"></param>
    public bool CheckMill(int piecePosition)
    {
        return MILLS.Any(x => x.Contains(piecePosition) &&
        x.Select(x => Pieces[x]).Distinct().ToArray().Length == 1);
    }

    public bool MovePossible(int connectionIndex)
    {
        int field1 = CONNECTIONS[connectionIndex][0];
        int field2 = CONNECTIONS[connectionIndex][1];

        if (Pieces[field1] != Color.None)
            return Pieces[field2] == Color.None;
        else
            return Pieces[field2] != Color.None;
    }

    public bool PlacePossible(int position) => Pieces[position] == Color.None;
    public bool JumpPossible(int sotart, int end) => Pieces[end] == Color.None;


    public State[] GenerateNextStates()
    {
        return null; 
    }

    public void TakeEnemyPiece() {
        Console.WriteLine("Which piece do you want to take?");
        int x = Convert.ToInt32(Console.ReadLine());

        int[] enemyPieces = Pieces.Select((col, i) => {
            if (col == (WhiteToMove ? Color.Black : Color.White))
                return i;
            return -1;
        }).Where(x => x != -1).ToArray();

        if (enemyPieces.Contains(x))
            Pieces[x] = Color.None;

        
    }

    public void ExecutePlace(int pos) {
        Pieces[pos] = WhiteToMove ? Color.White : Color.Black;

        if (CheckMill(pos))
            TakeEnemyPiece();
    }

    public void ExecuteAction() {
        if (Phase == Phase.Place) {
            Console.WriteLine("Where do you want to put your piece?");
            int pos = Convert.ToInt32(Console.ReadLine());

            if (PlacePossible(pos)) {
                ExecutePlace(pos);
            }

            if (WhiteToMove)
                WhiteStonesToPlace--;
            else
                BlackStonesToPlace--;
        }
        else {

        }

        WhiteToMove = !WhiteToMove;
    }

    public override string ToString()
    {
        string res = "";

        if (Phase == Phase.Place)
        {
            res += $"White to place: {WhiteStonesToPlace}, black to place: {BlackStonesToPlace}\n";
        }
        if (Phase == Phase.Play)
        {
            if (WhitePiecesOnBoard() < 4)
                res += "White can jump\n";
            if (BlackPiecesOnBoard() < 4)
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