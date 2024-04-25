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
        public List<Square> CapturePositions { get; set; } = new();

        public bool DoublePushedLastTurn { get; set; } = false;

        public ChessPiece _EnPassantCapture { get; set; } = null;
        public Square _EnPassantPosition { get; set; }

        #region Vectors
        static Square _up = new(1, 0);
        static Square _doubleUp = new(2, 0);
        static Square _rightUp = new(1, 1);
        static Square _leftUp = new(-1, 1);
        static Square _right = new(1, 0);
        static Square _left = new(-1, 0);
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

            Grid.SetRow(piece, Position.Y);
            Grid.SetColumn(piece, Position.X);

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
            CapturePositions.Add(Square.Add(Position, _leftUp));

            return _LegalMoves;
        }

        private void CalculatePawnMoves()
        {
            AdjustMoveVectors(PieceColor.White);
            HandlePieceInformation(_up, false);
            
            if (!Moved)
            {
                if (Color == PieceColor.White && Position.Y == 6)
                {
                    HandlePieceInformation(_doubleUp, false);
                }
                else if (Color == PieceColor.Black && Position.Y == 1)
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
        
        private void HandlePieceInformation(Square vector, bool capture)
        {
            ChessPiece piece = ChessBoard._ChessBoard.GetPieceAt(new Square(Position.Y + vector.Y, Position.X + vector.X));

            if (piece == null)
            {
                if (!capture)
                {
                    Square position = new(Position.Y + vector.Y, Position.X + vector.X);

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
                
                if (piece.Color == Color && capture)
                {
                    piece.IsCovered = true;
                }
            }
        }

        private void CalculateEnPassantMoves(Square vector)
        {
            ChessPiece piece = ChessBoard._ChessBoard.GetPieceAt(new Square(Position.Y + vector.Y, Position.X + vector.X));

            //TODO: ERROR?
            if (piece is Pawn pawn && piece.Color != Color)
            {
                //blockingPiece
                ChessPiece pieceToCapture = ChessBoard._ChessBoard.GetPieceAt(new Square(Position.Y + _up.Y, Position.X + vector.X));

                //TODO: War != null
                if (pieceToCapture != null && pawn.DoublePushedLastTurn)
                {
                    _EnPassantCapture = piece;
                    _EnPassantPosition = new Square(Position.Y + _up.Y, Position.X + vector.X);
                    _AllMoves.Add(new Square(piece.Position.Y + _up.Y, piece.Position.X + vector.X));
                }
            }
        }

        public ChessPiece TryPromote()
        {
            if (Color == PieceColor.White)
            {
                if (Position.Y == 1)
                {
                    ChessBoard._ChessBoard.RemovePieceAt(Position);

                    foreach (Square move in _AllMoves)
                    {
                        ChessGame._CurrentGame.LegalWhiteMoves.Remove(move);
                    }

                    ChessPiece queen = ChessBoard._ChessBoard.AddChessPiece('Q', PieceColor.White, new Square(Position.Y-1, Position.X));
                    
                    ChessGame._CurrentGame.LegalWhiteMoves.AddRange(queen.CalculateLegalMoves());
                    return queen;
                }
            }
            else
            {
                if (Position.Y == 6)
                {
                    ChessBoard._ChessBoard.RemovePieceAt(Position);
                    
                    foreach (Square move in _AllMoves)
                    {
                        ChessGame._CurrentGame.LegalBlackMoves.Remove(move);
                    }

                    ChessPiece queen = ChessBoard._ChessBoard.AddChessPiece('q', PieceColor.Black, new Square(Position.Y + 1, Position.X));
                    ChessGame._CurrentGame.LegalBlackMoves.AddRange(queen.CalculateLegalMoves());
                    return queen;
                }
            }

            return null;
        }
    }
}
