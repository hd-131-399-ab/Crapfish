using Chess.Engine.Board;
using Chess.Engine.Chess;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Chess.Engine.Pieces.ChessPieces
{
    public class Pawn : ChessPiece
    {
        public bool DoublePushedLastTurn { get; set; } = false;
        public ChessPiece _EnPassantCapture = null;
        public Square _EnPassantPosition;

        #region Vectors
        static Vector _up = new(0, 1);
        static Vector _doubleUp = new(0, 2);
        static Vector _rightUp = new(1, 1);
        static Vector _leftUp = new(-1, 1);
        static Vector _right = new(1, 0);
        static Vector _left = new(-1, 0);
        #endregion

        public Pawn(Square position, PieceColor pieceColor)
        {
            Position = position;
            Color = pieceColor;
            Value = 1;
            AssignFENType();

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

            MainWindow._MainWindow.chessBoard.Children.Add(piece);

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
            AdjustMoveVectors(PieceColor.White);
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

        private void AdjustMoveVectors(PieceColor adjustTo)
        {
            if (Color == PieceColor.White)
            {
                _up.Y = -1;
                _doubleUp.Y = -2;
                _rightUp.Y = -1;
                _leftUp.Y = -1;
            }
            else
            {
                _up.Y = 1;
                _doubleUp.Y = 2;
                _rightUp.Y = 1;
                _leftUp.Y = 1;
            }
        }
        
        private void HandlePieceInformation(Vector vector, bool capture)
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

        private void CalculateEnPassantMoves(Vector vector)
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
                        ChessGame._CurrentGame.LegalWhiteMoves.Remove(move);
                    }

                    ChessPiece queen = ChessBoard._ChessBoard.AddChessPiece('Q', PieceColor.White, new Square(Position.Row-1, Position.Column));
                    
                    ChessGame._CurrentGame.LegalWhiteMoves.AddRange(queen.CalculateLegalMoves());
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
                        ChessGame._CurrentGame.LegalBlackMoves.Remove(move);
                    }

                    ChessPiece queen = ChessBoard._ChessBoard.AddChessPiece('q', PieceColor.Black, new Square(Position.Row + 1, Position.Column));
                    ChessGame._CurrentGame.LegalBlackMoves.AddRange(queen.CalculateLegalMoves());
                    return queen;
                }
            }

            return null;
        }
    }
}
