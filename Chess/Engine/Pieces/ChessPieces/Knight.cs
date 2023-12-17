using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Chess.Engine.Board;

namespace Chess.Engine.Pieces.ChessPieces
{
    public class Knight : ChessPiece
    {
        #region Vectors
        static Vector _rightUp = new(2, 1);
        static Vector _rightDown = new(2, -1);

        static Vector _leftUp = new(-2, 1);
        static Vector _leftDown = new(-2, -1);

        static Vector _upLeft = new(-1, 2);
        static Vector _upRight = new(1, 2);

        static Vector _downLeft = new(-1, -2);
        static Vector _downRight = new(1, -2);
        #endregion

        public Knight(Square position, PieceColor pieceColor)
        {
            Position = position;
            Color = pieceColor;
            Value = 3;
            AssignFENType();

            _AllMoves = new();
            _LegalMoves = new();
            _LegalCaptures = new();

            Piece = Create();
        }

        //TODO: Burn not-ranged-piece code
        private void AssignFENType()
        {
            if (Color == PieceColor.White)
            {
                FENType = 'N';
            }
            else
            {
                FENType = 'n';
            }
        }

        protected override Image Create()
        {
            Image piece = new();

            if (Color == PieceColor.White)
            {
                piece.Source = new BitmapImage(new Uri(@"Mats/knight.png", UriKind.Relative));
            }
            else
            {
                piece.Source = new BitmapImage(new Uri(@"Mats/_knight.png", UriKind.Relative));
            }

            Grid.SetRow(piece, Position.Row);
            Grid.SetColumn(piece, Position.Column);

            piece.MouseUp += OnChessPiece_MouseUp;

            MainWindow._MainWindow.chessBoard.Children.Add(piece);

            return piece;
        }

        internal override List<Square> CalculateLegalMoves()
        {
            _AllMoves.Clear();
            _LegalMoves.Clear();
            _LegalCaptures.Clear();

            CalculateForkMoves();

            return _AllMoves;
        }

        private void CalculateForkMoves()
        {
            HandlePieceInformation(_upLeft);
            HandlePieceInformation(_upRight);

            HandlePieceInformation(_rightUp);
            HandlePieceInformation(_rightDown);

            HandlePieceInformation(_downLeft);
            HandlePieceInformation(_downRight);

            HandlePieceInformation(_leftUp);
            HandlePieceInformation(_leftDown);
        }

        private void HandlePieceInformation(Vector vector)
        {
            int row = Position.Row + (int)vector.Y;
            int column = Position.Column + (int)vector.X;

            if (row is > -1 and < 8 && column is > -1 and < 8)
            {
                ChessPiece piece = ChessBoard._ChessBoard.GetPieceAt(new Square(row, column));

                if (piece == null)
                {
                    _LegalMoves.Add(new(row, column));
                    _AllMoves.Add(new(row, column));
                }
                else
                {
                    if (piece.Color != Color)
                    {
                        _LegalCaptures.Add(piece);
                        _AllMoves.Add(piece.Position);
                    }
                    else
                    {
                        piece.IsCovered = true;
                    }
                }
            }
        }
    }
}
