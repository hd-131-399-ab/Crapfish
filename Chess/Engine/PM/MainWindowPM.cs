using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Chess
{
    public partial class MainWindow
    {
        public static Grid ChessBoard;
        public static Grid WinnerGrd;
        public static Label WinnerLbl;

        private void InitializePMV()
        {
            ChessBoard = chessBoard;
            WinnerGrd = winnerGrd;

            WinnerLbl = winnerLbl;
        }
    }
}
