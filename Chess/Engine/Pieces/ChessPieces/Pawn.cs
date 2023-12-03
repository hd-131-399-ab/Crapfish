using Chess.Engine.Board;
using Chess.Engine.Chess;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Chess.Engine.Pieces.ChessPieces
{
    public class Pawn : ChessPiece
    {
        public bool DoublePushedLastTurn { get; set; } = false;
        public ChessPiece _EnPassantCapture = null;
        public Square _EnPassantPosition;

        private Vector2 _up;
        private Vector2 _doubleUp;
        private Vector2 _rightUp;
        private Vector2 _leftUp;
        private Vector2 _right;
        private Vector2 _left;

        public Pawn(Square position, PieceColor pieceColor)
        {
            Position = position;
            Color = pieceColor;
            Value = 1;
            AssignFENType();
            GetMoveVectors();

            Piece = Create();
        }

        private void AssignFENType()
        {
            if (Color == PieceColor.White)
            {
                FENType = 'P';
            }
            else
            {
                FENType = 'p';
            }
        }

        protected override Image Create()
        {
            Image piece = new();

            if (Color == PieceColor.White)
            {
                piece.Source = new BitmapImage(new Uri(@"Mats/pawn.png", UriKind.Relative));
            }
            else
            {
                piece.Source = new BitmapImage(new Uri(@"Mats/_pawn.png", UriKind.Relative));
            }

            Grid.SetRow(piece, Position.Row);
            Grid.SetColumn(piece, Position.Column);

            piece.MouseUp += OnChessPiece_MouseUp;

            MainWindow.ChessBoard.Children.Add(piece);

            return piece;
        }

        internal override List<Square> CalculateLegalMoves()
        {
            _AllMoves.Clear();
            _LegalMoves.Clear();
            _LegalCaptures.Clear();
            _EnPassantCapture = null;

            if (DoublePushedLastTurn)
            {
                DoublePushedLastTurn = false;
            }

            CalculatePawnMoves();
            
            return _AllMoves;
        }

        private void CalculatePawnMoves()
        {
            HandlePieceInformation(_up, false);
            
            if (!Moved)
            {
                if (Color == PieceColor.White && Position.Row == 6)
                {
                    HandlePieceInformation(_doubleUp, false);
                }
                else if (Color == PieceColor.Black && Position.Row == 1)
                {
                    HandlePieceInformation(_doubleUp, false);
                }
            }

            HandlePieceInformation(_rightUp, true);
            HandlePieceInformation(_leftUp, true);

            CalculateEnPassantMoves(_left);
            CalculateEnPassantMoves(_right);
        }

        private void GetMoveVectors()
        {
            if (Color == PieceColor.White)
            {
                _up = new(0, -1);
                _doubleUp = new(0, -2);
                _rightUp = new(1, -1);
                _leftUp = new(-1, -1);
            }
            else
            {
                _up = new(0, 1);
                _doubleUp = new(0, 2);
                _rightUp = new(1, 1);
                _leftUp = new(-1, 1);
            }

            _left = new(-1, 0);
            _right = new(1, 0);
        }

        private void HandlePieceInformation(Vector2 vector, bool capture)
        {
            ChessPiece piece = null;

            try
            {
                piece = ChessBoard._ChessBoard.GetPieceAt(new Square(Position.Row + (int)vector.Y, Position.Column + (int)vector.X));
            }
            catch
            {
                return;
            }

            if (piece == null)
            {
                if (!capture)
                {
                    Square position = new(Position.Row + (int)vector.Y, Position.Column + (int)vector.X);

                    _LegalMoves.Add(position);
                    _AllMoves.Add(position);
                }
            }
            else
            {
                if (piece.Color != Color && capture)
                {
                    _LegalCaptures.Add(piece);
                    _AllMoves.Add(piece.Position);
                }
                else if (capture)
                {
                    piece.IsCovered = true;
                }
            }
        }

        private void CalculateEnPassantMoves(Vector2 vector)
        {
            ChessPiece piece = null;

            try
            {
                piece = ChessBoard._ChessBoard.GetPieceAt(new Square(Position.Row + (int)vector.Y, Position.Column + (int)vector.X));
            }
            catch
            {
                return;
            }

            if (piece != null)
            {
                if (piece is Pawn pawn && piece.Color != Color)
                {
                    try
                    {
                        //blockingPiece
                        ChessPiece captureSquarePiece = ChessBoard._ChessBoard.GetPieceAt(new Square(Position.Row + (int)_up.Y, Position.Column + (int)vector.X));

                        if (captureSquarePiece == null && pawn.DoublePushedLastTurn)
                        {
                            _EnPassantCapture = piece;
                            _EnPassantPosition = new Square(Position.Row + (int)_up.Y, Position.Column + (int)vector.X);
                            _AllMoves.Add(new Square(piece.Position.Row + (int)_up.Y, piece.Position.Column + (int)vector.X));
                        }
                    }
                    catch { }
                }
            }
        }

        public ChessPiece TryPromote()
        {
            if (Color == PieceColor.White)
            {
                if (Position.Row == 1)
                {
                    ChessBoard._ChessBoard.RemovePieceAt(Position);

                    foreach (Square move in _AllMoves)
                    {
                        ChessGame.CurrentGame.LegalWhiteMoves.Remove(move);
                    }

                    ChessPiece queen = ChessBoard._ChessBoard.AddChessPiece('Q', PieceColor.White, new Square(Position.Row-1, Position.Column));
                    
                    ChessGame.CurrentGame.LegalWhiteMoves.AddRange(queen.CalculateLegalMoves());
                    return queen;
                }
            }
            else
            {
                if (Position.Row == 6)
                {
                    ChessBoard._ChessBoard.RemovePieceAt(Position);
                    
                    foreach (Square move in _AllMoves)
                    {
                        ChessGame.CurrentGame.LegalBlackMoves.Remove(move);
                    }

                    ChessPiece queen = ChessBoard._ChessBoard.AddChessPiece('q', PieceColor.Black, new Square(Position.Row + 1, Position.Column));
                    ChessGame.CurrentGame.LegalBlackMoves.AddRange(queen.CalculateLegalMoves());
                    return queen;
                }
            }

            return null;
        }
    }
}
