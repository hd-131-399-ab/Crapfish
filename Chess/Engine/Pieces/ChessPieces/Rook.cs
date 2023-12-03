using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Chess.Engine.Board;
using Chess.Engine.Chess;

namespace Chess.Engine.Pieces.ChessPieces
{
    public class Rook : RangedPiece
    {
        public List<Square> CastleMoves { get; set; } = new();

        public Rook(Square position, PieceColor pieceColor)
        {
            Position = position;

            Color = pieceColor;
            Value = 5;
            AttackPattern = AttackType.StraightPierce;
            AssignFENType();

            Piece = Create();
        }

        private void AssignFENType()
        {
            if (Color == PieceColor.White)
            {
                FENType = 'R';
            }
            else
            {
                FENType = 'r';
            }
        }

        protected override Image Create()
        {
            Image piece = new();

            if (Color == PieceColor.White)
            {
                piece.Source = new BitmapImage(new Uri(@"Mats/rook.png", UriKind.Relative));
            }
            else
            {
                piece.Source = new BitmapImage(new Uri(@"Mats/_rook.png", UriKind.Relative));
            }

            Grid.SetRow(piece, Position.Row);
            Grid.SetColumn(piece, Position.Column);

            piece.MouseUp += OnChessPiece_MouseUp;

            MainWindow.ChessBoard.Children.Add(piece);

            return piece;
        }

        internal override List<Square> CalculateAllLegalMoves()
        {
            _AllMoves.Clear();
            _LegalMoves.Clear();
            _LegalCaptures.Clear();

            CalculateStraightMoves();

            if (Color == PieceColor.White)
                ChessGame.CurrentGame.VirtualWhiteMoves.AddRange(VirtualMoves);
            else
                ChessGame.CurrentGame.VirtualBlackMoves.AddRange(VirtualMoves);

            return _AllMoves;
        }
    }
}
