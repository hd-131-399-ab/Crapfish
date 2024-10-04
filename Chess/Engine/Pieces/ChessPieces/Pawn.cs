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

        public ChessPiece _EnPassantCapture { get; set; } = null;
        public Square _EnPassantPosition { get; set; }

        #region Vectors
        static Square _doubleUp = new(0, 0);
        static Square _rightUp = new(0, 1);
        static Square _leftUp = new(0, -1);
        static Square _right = new(0, 1);
        static Square _left = new(0, -1);
        static Square _up = new(1, 0);
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
            return _LegalMoves;
        }

        private void CalculatePawnMoves()
        {
            AdjustMoveVectors(Color);
            EvaluateMove(_up, false);
            
            if (!Moved)
            {
                EvaluateMove(_doubleUp, false);
            }

            EvaluateMove(_rightUp, true);
            EvaluateMove(_leftUp, true);

            CalculateEnPassantMoves(_left);
            CalculateEnPassantMoves(_right);
        }

        private void AdjustMoveVectors(PieceColor adjustTo)
        {
            if (Color == PieceColor.White)
            {
                _up.Row = -1;
                _doubleUp.Row = -2;
                _rightUp.Row = -1;
                _leftUp.Row = -1;
            }
            else
            {
                _up.Row = 1;
                _doubleUp.Row = 2;
                _rightUp.Row = 1;
                _leftUp.Row = 1;
            }
        }

        private void EvaluateMove(Square vector, bool capture)
        {
            ChessPiece piece = ChessBoard._ChessBoard.GetPieceAt(new Square(Position.Row + vector.Row, Position.Column + vector.Column));

            if (piece == null)
            {
                if (!capture)
                {
                    Square position = new(Position.Row + vector.Row, Position.Column + vector.Column);

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
                    piece.CoveringPiece = this;
                }
            }
        }

        private void CalculateEnPassantMoves(Square vector)
        {
            ChessPiece piece = ChessBoard._ChessBoard.GetPieceAt(new Square(Position.Row + vector.Row, Position.Column + vector.Column));//1 0

            //TODO FIX THIS FUCKING SHIT
            //TODO: ERROR?
            if (piece is Pawn pawn && piece.Color != Color)
            {
                // blockingPiece
                ChessPiece blockingPiece = ChessBoard._ChessBoard.GetPieceAt(new Square(Position.Row + _up.Row, Position.Column + vector.Column));

                //TODO: War != null
                if (blockingPiece == null && pawn.DoublePushedLastTurn)
                {
                    _EnPassantCapture = piece;
                    _EnPassantPosition = new Square(Position.Row + _up.Row, Position.Column + vector.Column);
                    _AllMoves.Add(new Square(piece.Position.Row + _up.Row, piece.Position.Column + vector.Column));
                }
            }
        }

        public ChessPiece TryPromote()
        {
            if 

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
