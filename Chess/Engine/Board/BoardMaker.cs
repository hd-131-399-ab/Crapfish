using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chess.Engine.Board
{
    public class BoardMaker
    {
        private string _whiteHex;
        private string _blackHex;
        
        public BoardMaker(string whiteHex, string blackHex)
        {
            _whiteHex = whiteHex;
            _blackHex = blackHex;

            CreateNewBoard();
        }

        public void CreateNewBoard()
        {
            bool drawWhiteSquare = true;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(j != 0)
                    {
                        drawWhiteSquare = !drawWhiteSquare;
                    }

                    Square pos = new(i, j);

                    CreateNewSquare(pos, drawWhiteSquare);
                }
            }
        }

        private void CreateNewSquare(Square position, bool white)
        {
            Rectangle square = new();

            if (white)
            {
                square.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(_whiteHex);
            }
            else
            {
                square.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(_blackHex);
            }

            Grid.SetRow(square, position.Row);
            Grid.SetColumn(square, position.Column);

            MainWindow._MainWindow.chessBoard.Children.Add(square);
        }
    }
}
