using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Chess.Engine.Board;
using Chess.Engine.Chess;

namespace Chess.Engine.Pieces.ChessPieces
{
    public class Bishop : RangedPiece
    {
        public Bishop(Square position, PieceColor pieceColor)
        {
            Position = position;

            Color = pieceColor;
            Value = 3;
            AttackPattern = AttackType.DiagonalPierce;
            AssignFENType();

            Piece = Create();
        }

        private void AssignFENType()
        {
            if (Color == PieceColor.White)
            {
                FENType = 'B';
            }
            else
            {
                FENType = 'b';
            }
        }

        protected override Image Create()
        {
            Image piece = new();

            if (Color == PieceColor.White)
            {
                piece.Source = new BitmapImage(new Uri(@"Mats/bishop.png", UriKind.Relative));
            }
            else
            {
                piece.Source = new BitmapImage(new Uri(@"Mats/_bishop.png", UriKind.Relative));
            }

            Grid.SetRow(piece, Position.Row);
            Grid.SetColumn(piece, Position.Column);

            piece.MouseUp += OnChessPiece_MouseUp;

            MainWindow._MainWindow.chessBoard.Children.Add(piece);

            return piece;
        }

        internal override List<Square> CalculateAllLegalMoves()
        {
            _AllMoves.Clear();
            _LegalMoves.Clear();
            _LegalCaptures.Clear();
            VirtualMoves.Clear();

            CalculateDiagonalMoves();

            if (Color == PieceColor.White)
                ChessGame._CurrentGame.VirtualWhiteMoves.AddRange(VirtualMoves);
            else
                ChessGame._CurrentGame.VirtualBlackMoves.AddRange(VirtualMoves);

            return _AllMoves;
        }
    }
}
