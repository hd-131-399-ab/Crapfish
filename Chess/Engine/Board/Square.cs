namespace Chess.Engine.Board
{
    public struct Square
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Square(int row, int column)
        {
            Y = row;
            X = column;
        }

        public static bool IsEqual(Square pos1, Square pos2) => pos1.Y == pos2.Y && pos1.X == pos2.X;

        public static Square Add(Square vector1, Square vector2) => new Square(vector1.X + vector2.X, vector1.Y + vector2.Y);
    }
}
