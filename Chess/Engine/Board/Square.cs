using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Engine.Board
{
    public struct Square
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Square(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public static bool IsEqual(Square pos1, Square pos2) => pos1.Row == pos2.Row && pos1.Column == pos2.Column;
    }
}
