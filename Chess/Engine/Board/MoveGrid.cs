using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Chess.Engine.Chess;
using Chess.Engine.Pieces;
using Chess.Engine.Pieces.ChessPieces;

namespace Chess.Engine.Board
{
    //TODO:structure
    public class MoveGrid
    {
        public Rectangle MoveGrd;
        public static List<MoveGrid> MoveGrids { get; set; } = new();
        public Square Position { get; set; }
        public enum MoveTypes { Move, Capture, EnPassant }
        public MoveTypes MoveType { get; set; }

        public MoveGrid(Square position, MoveTypes moveType)
        {
            //TODO: CastleGrid : MoveGrid
            Position = position;
            MoveType = moveType;

            Create();
        }

        protected void Create()
        {
            Rectangle moveGrid = new();

            if (MoveType == MoveTypes.Move)
            {
                moveGrid.Fill = Brushes.Green;
            }
            else
            {
                moveGrid.Fill = Brushes.Orange;
            }

            moveGrid.Opacity = 0.3;

            moveGrid.MouseUp += OnMoveGrid_MouseUp;

            Grid.SetRow(moveGrid, Position.Row);
            Grid.SetColumn(moveGrid, Position.Column);

            MoveGrd = moveGrid;

            MainWindow._MainWindow.chessBoard.Children.Add(MoveGrd);
            MoveGrids.Add(this);
        }

        public static void DestroyAll()
        {
            foreach (MoveGrid moveGrid in MoveGrids)
            {
                moveGrid.Destroy();
            }

            MoveGrids.Clear();
        }

        private void Destroy() => MainWindow._MainWindow.chessBoard.Children.Remove(MoveGrd);

        private void OnMoveGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ChessBoard._ChessBoard.CheckMarker.Visibility == Visibility.Visible)
            {
                ChessBoard._ChessBoard.CheckMarker.SwitchVisibility();
            }

            ClickBehaviour();
            ChessGame._CurrentGame.SwitchMoves();
        }

        //TODO: CaptureGrid : MoveGrid
        protected virtual void ClickBehaviour()
        {
            ChessPiece piece = ChessBoard._ChessBoard.SelectedPiece;

            if (piece is Pawn pawn)
            {
                if (piece.Color == ChessPiece.PieceColor.White && Position.Row == piece.Position.Row - 2)
                {
                    pawn.DoublePushedLastTurn = true;
                }
                else if (piece.Color == ChessPiece.PieceColor.Black && Position.Row == piece.Position.Row + 2)
                {
                    pawn.DoublePushedLastTurn = true;
                }

                ChessPiece queen = pawn.TryPromote();

                if (queen != null)
                {
                    piece = queen;
                }
            }

            switch (MoveType)
            {
                case MoveTypes.Move:
                    piece.MovePiece(Position);
                    break;
                case MoveTypes.Capture:
                    piece.Capture(ChessBoard._ChessBoard.GetPieceAt(Position));
                    break;
            }
        }
    }
}
