using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chess.Engine.Board
{
    public class BoardMaker
    {
        private bool _drawWhiteSquare = true;

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
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(j != 0)
                    {
                        _drawWhiteSquare = !_drawWhiteSquare;
                    }

                    Square pos = new(i, j);

                    CreateNewSquare(pos);
                }
            }
        }

        private void CreateNewSquare(Square position)
        {
            Rectangle square = new();

            if (_drawWhiteSquare)
            {
                square.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(_whiteHex);
            }
            else
            {
                square.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(_blackHex);
            }

            Grid.SetRow(square, position.Row);
            Grid.SetColumn(square, position.Column);

            MainWindow.ChessBoard.Children.Add(square);
        }
    }
}
