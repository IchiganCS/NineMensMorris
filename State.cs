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
    private static int[][] MILLS = new int[][]{
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
    private static int[][] CONNECTIONS = new int[][]{
        new int[]{0, 1}, new int[]{1, 2}, new int[]{2, 14}, new int[]{14, 23}, new int[]{23, 22}, new int[]{22, 21}, new int[]{21, 9}, new int[]{9, 0},
        new int[]{1, 4}, new int[]{9, 10}, new int[]{13, 14}, new int[]{19, 22},
        new int[]{3, 4}, new int[]{4, 5}, new int[]{5, 13}, new int[]{13, 20}, new int[]{20, 19}, new int[]{19, 18}, new int[]{18, 10}, new int[]{10, 3},
        new int[]{4, 7}, new int[]{12, 13}, new int[]{16, 19}, new int[]{10, 11},
        new int[]{6, 7}, new int[]{7, 8}, new int[]{8, 12}, new int[]{12, 17}, new int[]{17, 16}, new int[]{16, 15}, new int[]{15, 11}, new int[]{11, 6}
    };

    public int WhitePiecesToPlace { get; set; } = 9;
    public int BlackPiecesToPlace { get; set; } = 9;

    public int PiecesToPlace(Color color) {
        return color == Color.White ? WhitePiecesToPlace : BlackPiecesToPlace;
    }

    public Phase Phase { 
        get => (WhitePiecesToPlace > 0 || BlackPiecesToPlace > 0) ? Phase.Place : Phase.Play; }

    public bool WhiteToMove { get; set; } = true;
    public bool BlackToMove
    {
        get => !WhiteToMove;
        set => WhiteToMove = !value;
    }

    public bool WhiteCanJump {
        get => Pieces.Count(x => x == Color.White) + WhitePiecesToPlace < 4;
    }
    public bool BlackCanJump {
        get => Pieces.Count(x => x == Color.Black) + BlackPiecesToPlace < 4;
    }
    public bool CanJump(Color color) {
        return color == Color.White ? WhiteCanJump : BlackCanJump;
    }

    public Color PlayerToMove { 
        get => WhiteToMove ? Color.White : Color.Black;
    }
    public Color EnemyColor {
        get => WhiteToMove ? Color.Black : Color.White;
    }

    public int PieceCount(Color color)
    {
        return Pieces.Count(x => x == color) + 
            PiecesToPlace(color);
    }

    public IEnumerable<int> PiecePostions(Color color) {
        return Enumerable.Range(0, Pieces.Length).Where(x => Pieces[x] == color);
    }

    /// <returns>None if no winner has been found yet</returns>
    public Color DetermineWinner()
    {
        if (PieceCount(Color.White) < 3)
            return Color.Black;
        else if (PieceCount(Color.Black) < 3)
            return Color.White;

        return Color.None;
    }


    public State Clone()
    {
        State x = new();
        x.Pieces = Pieces.ToArray();
        x.BlackPiecesToPlace = BlackPiecesToPlace;
        x.WhitePiecesToPlace = WhitePiecesToPlace;
        x.WhiteToMove = WhiteToMove;
        return x;
    }

    public override bool Equals(object obj) {
        if (obj is null || !(obj is State))
            return false;
        else
            return Equals(obj as State);
    }
    public bool Equals(State state) {
        if (state is null)
            return false;
        return state.WhiteToMove == WhiteToMove && 
            state.BlackPiecesToPlace == BlackPiecesToPlace &&
            state.WhitePiecesToPlace == WhitePiecesToPlace &&
            state.Pieces.Equals(Pieces);
    }

    /// <summary>
    /// Checks whether piecePosition lies in a mill
    /// </summary>
    /// <param name="piecePosition"></param>
    public bool CheckMill(int piecePosition)
    {
        return MILLS.Any(x => x.Contains(piecePosition) &&
            //just one color
            x.Select(x => Pieces[x]).Distinct().Count() == 1);
    }

    public bool MovePossible(int start, int end)
    {
        if (!CONNECTIONS.Any(x => x.Contains(start) && x.Contains(end)))
            return false;

        if (Pieces[start] != Color.None)
            return Pieces[end] == Color.None;
        return false;
    }

    public bool PlacePossible(int position) => Pieces[position] == Color.None;
    public bool JumpPossible(int start, int end) => Pieces[end] == Color.None;


    public State[] GenerateNextStates()
    {
        return null; 
    }

    public void TakeEnemyPiece() {
        Console.WriteLine("Which piece do you want to take?");
        int x = Convert.ToInt32(Console.ReadLine());

        if (Pieces[x] == EnemyColor && !CheckMill(x))
            RemovePiece(x);

        else
            Console.WriteLine("tried taking from mill");
        
    }

    
    public void RemovePiece(int pos) {
        Pieces[pos] = Color.None;
    }
    public void PlacePiece(int pos, Color col) {
        Pieces[pos] = col;

        if (CheckMill(pos))
            TakeEnemyPiece();
    }

    public void ExecuteAction() {
        if (Phase == Phase.Place) {
            Console.WriteLine("Where do you want to put your piece?");
            int pos = Convert.ToInt32(Console.ReadLine());

            if (PlacePossible(pos)) {
                PlacePiece(pos);
            }

            if (WhiteToMove)
                WhitePiecesToPlace--;
            else
                BlackPiecesToPlace--;
        }
        else {
            Console.WriteLine("Select the piece you want to move.");
            int start = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Where do you want to move your piece?");
            int end = Convert.ToInt32(Console.ReadLine());

            if ((WhiteToMove && WhitePiecesCount() < 4 
              || BlackToMove && BlackPiecesCount() < 4) && JumpPossible(start, end))
                ExecuteJump(start, end);
            else if (MovePossible(start, end)) {
                ExecuteMove(start, end);
            }
        }

        WhiteToMove = !WhiteToMove;
    }

    public override string ToString()
    {
        string res = "";

        if (Phase == Phase.Place)
        {
            res += $"White to place: {WhitePiecesToPlace}, black to place: {BlackPiecesToPlace}\n";
        }
        if (Phase == Phase.Play)
        {
            if (WhitePiecesCount() < 4)
                res += "White can jump\n";
            if (BlackPiecesCount() < 4)
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