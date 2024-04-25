using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Chess.Engine.Chess;
using Chess.Engine.Pieces.ChessPieces;

namespace Chess.Engine.Board
{
    public class CastleGrid
    {
        public static List<CastleGrid> CastleGrids { get; set; } = new();

        public Rectangle CastleGrd;

        public Square Position { get; set; }

        public CastleMove _CastleMove;

        public CastleGrid(Square pos, CastleMove castleMove)
        {
            Position = pos;
            _CastleMove = castleMove;

            Create();
        }

        private void Create()
        {
            Rectangle castleGrid = new();

            castleGrid.Fill = Brushes.Green;
            castleGrid.Opacity = 0.3;

            castleGrid.MouseUp += OnCastleGrid_MouseUp;

            Grid.SetRow(castleGrid, Position.Y);
            Grid.SetColumn(castleGrid, Position.X);

            CastleGrd = castleGrid;

            MainWindow._MainWindow.chessBoard.Children.Add(CastleGrd);
            CastleGrids.Add(this);
        }

        public static void DestroyAll()
        {
            foreach (CastleGrid moveGrid in CastleGrids)
            {
                moveGrid.Destroy();
            }

            CastleGrids.Clear();
        }

        public void Destroy()
        {
            MainWindow._MainWindow.chessBoard.Children.Remove(CastleGrd);
        }

        private void OnCastleGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            King king = _CastleMove.King;
            Rook rook = _CastleMove.Rook;

            Castle(king, rook);

            ChessGame._CurrentGame.SwitchMoves();
        }

        private void Castle(King king, Rook rook)
        {
            king.MovePiece(_CastleMove.KingPos);
            rook.MovePiece(_CastleMove.RookPos);

            TakeCastleRights(king);
        }

        private void TakeCastleRights(King king)
        {
            if (king.Color == Pieces.ChessPiece.PieceColor.White)
            {
                ChessGame._CurrentGame.WhiteCastleRights = ChessGame.CastleRights.None;
            }
            else
            {
                ChessGame._CurrentGame.BlackCastleRights = ChessGame.CastleRights.None;
            }
        }
    }
}
