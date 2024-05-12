using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chess.Engine.Board
{
    public class SelectionGrid
    {
        private Rectangle SelectionGrd;

        Square _Position;
        public Square Position
        {
            get => _Position;

            set
            {
                Grid.SetRow(SelectionGrd, value.Row);
                Grid.SetColumn(SelectionGrd, value.Column);

                _Position = value;
            }
        }

        Visibility _CurrentVisibility;
        public Visibility CurrentVisibility
        {
            get => _CurrentVisibility;

            set
            {
                SelectionGrd.Visibility = value;

                _CurrentVisibility = value;
            }
        }

        public void CreateNewSelectionGrd(Visibility visibilityState)
        {          
            SelectionGrd = new Rectangle()
            {
                Fill = Brushes.CornflowerBlue,
                Opacity = 0.3,
                Visibility = visibilityState
            };

            SelectionGrd.MouseUp += OnSelectionGrid_MouseUp;

            CurrentVisibility = visibilityState;

            MainWindow._MainWindow.chessBoard.Children.Add(SelectionGrd);
        }

        public void SwitchVisibility()
        {
            if (CurrentVisibility == Visibility.Visible)
            {
                CurrentVisibility = Visibility.Hidden;
            }
            else
            {
                CurrentVisibility = Visibility.Visible;
            }
        }

        public void Move(Square newPosition)
        {
            Position = newPosition;

            CurrentVisibility = Visibility.Visible;
        }

        private void OnSelectionGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SwitchVisibility();
            MoveGrid.DestroyAll();
            CastleGrid.DestroyAll();

            ChessBoard._ChessBoard.SelectedPiece = null;
        }
    }
}
