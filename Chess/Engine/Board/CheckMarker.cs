using Chess.Engine.Pieces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Chess.Engine.Board
{
    public class CheckMarker
    {
        private Grid _CheckMrker;
        private Grid CheckMrker
        {
            get => _CheckMrker;

            set
            {
                if (value == null)
                {
                    Position = new Square(0, 0);
                    Visibility = Visibility.Hidden;
                }

                _CheckMrker = value;
            }
        }

        private Visibility _Visibility;
        public Visibility Visibility
        {
            get => _Visibility;

            set
            {
                CheckMrker.Visibility = value;

                _Visibility = value;
            }
        }

        private Square _Position;
        public Square Position
        {
            get => _Position;

            set
            {
                Grid.SetRow(CheckMrker, value.Y);
                Grid.SetColumn(CheckMrker, value.X);

                _Position = value;
            }
        }

        public CheckMarker(Visibility visibility)
        {
            CreateNewCheckMarker(visibility);
        }

        public void CreateNewCheckMarker(Visibility visibility)
        {
            if (CheckMrker != null)
            {
                MainWindow._MainWindow.chessBoard.Children.Remove(CheckMrker);
            }

            CheckMrker = new()
            {
                Background = Brushes.Red,
                Opacity = 0.4
            };

            CheckMrker.MouseUp += OnCheckMarker_MouseUp;

            Visibility = visibility;
            Position = new(0, 0);

            MainWindow._MainWindow.chessBoard.Children.Add(CheckMrker);
        }

        public void Move(Square newPosition)
        {
            Position = newPosition;

            Visibility = Visibility.Visible;
        }

        public void SwitchVisibility()
        {
            if (Visibility == Visibility.Visible)
            {
                Visibility = Visibility.Hidden;
            }
            else
            {
                Visibility = Visibility.Visible;
            }
        }

        public void Destroy()
        {
            MainWindow._MainWindow.chessBoard.Children.Remove(CheckMrker);

            CheckMrker = null;
        }
        
        private void OnCheckMarker_MouseUp(object sender, MouseButtonEventArgs e) => ChessBoard._ChessBoard.GetPieceAt(Position).OnChessPiece_MouseUp(sender, e);
    }
}
