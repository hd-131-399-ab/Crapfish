using System;
using System.Diagnostics.CodeAnalysis;

namespace Chess.Engine.Board
{
    public struct Square
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public Square(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public static bool IsEqual(Square pos1, Square pos2) => pos1.Row == pos2.Row && pos1.Column == pos2.Column;

        public static Square Add(Square vector1, Square vector2) => new Square(vector1.Column + vector2.Column, vector1.Row + vector2.Row);
    }
}
