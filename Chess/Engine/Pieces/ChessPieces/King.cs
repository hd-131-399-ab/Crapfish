using Chess.Engine.Board;
using Chess.Engine.Chess;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Chess.Engine.Pieces.ChessPieces
{
    public class King : ChessPiece
    {
        public List<ChessPiece> PinnedPieces { get; set; } = new();
        public List<CastleMove> _CastleMoves { get; set; } = new();

        #region Vectors
        static Square _UpLeft = new(-1, -1);
        static Square _Up = new(0, -1);
        static Square _UpRight = new(1, -1);

        static Square _Right = new(1, 0);
        static Square _RightDown = new(1, 1);

        static Square _Down = new(0, 1);
        static Square _DownLeft = new(-1, 1);
        static Square _Left = new(-1, 0);
        #endregion

        public bool IsChecked { get; set; }
        public int CheckCount { get; set; }

        private bool _canCastle = true; 
        public bool CanCastle
        {
            get
            {
                if (_canCastle)
                    return CheckCastleRights();
                else
                    return false;
            }

            set
            {
                _canCastle = value;
            }

        }

        public King(Square position, PieceColor pieceColor)
        {
            Position = position;
            Color = pieceColor;
            Value = 0;

            AssignFENType();

            Piece = Create();
        }

        private void AssignFENType()
        {
            if (Color == PieceColor.White)
            {
                FENType = 'K';
            }
            else
            {
                FENType = 'k';
            }
        }

        protected override Image Create()
        {
            Image piece = new();

            if (Color == PieceColor.White)
            {
                piece.Source = new BitmapImage(new Uri(@"Mats/king.png", UriKind.Relative));
            }
            else
            {
                piece.Source = new BitmapImage(new Uri(@"Mats/_king.png", UriKind.Relative));
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
            _CastleMoves.Clear();

            CheckCount = 0;

            CalculateKingMoves();            

            if (CanCastle)
            {
                if (Color == PieceColor.White)
                {
                    //White Queen
                    CalculateCastleMove(new(7, 0), new(7, 3), new(7, 2));
                    //White King
                    CalculateCastleMove(new(7, 7), new(7, 5), new(7, 6));
                }
                else
                {
                    //Black Queen
                    CalculateCastleMove(new(0, 0), new(0, 3), new(0, 2));
                    //Black King
                    CalculateCastleMove(new(0, 7), new(0, 5), new(0, 6));
                }
            }

            return _AllMoves;
        }

        private void CalculateKingMoves()
        {
            HandlePieceInformation(_UpLeft);
            HandlePieceInformation(_Up);
            HandlePieceInformation(_UpRight);

            HandlePieceInformation(_Right);
            HandlePieceInformation(_RightDown);
            
            HandlePieceInformation(_Down);
            HandlePieceInformation(_DownLeft);
            HandlePieceInformation(_Left);
        }

        private void HandlePieceInformation(Square vector)
        {
            int row = Position.Y + (int)vector.Y;
            int column = Position.X + (int)vector.X;

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


        private bool CheckCastleRights()
        {
            Square kingsSquare;

            if (Color == PieceColor.White)
            {
                kingsSquare = new(7, 4);
            }
            else
            {
                kingsSquare = new(0, 4);
            }

            try
            {
                King king = (King)ChessBoard._ChessBoard.GetPieceAt(kingsSquare);
            }
            catch
            {
                return false;
            }

            if (Moved)
                return false;

            return true;
        }

        private void CalculateCastleMove(Square rookPos, Square newRookPos, Square newKingPos)
        {
            ChessPiece rook = ChessBoard._ChessBoard.GetPieceAt(rookPos);

            if (rook != null && rook.FENType.ToString().ToLower() == "r" && rook.Color == Color && !rook.Moved)
            {
                Rook r = (Rook)rook;

                int column = Position.X;
                --column;

                for (int i = column; i > 0; i--)
                {
                    ChessPiece piece = ChessBoard._ChessBoard.GetPieceAt(new Square(rookPos.Y, i));

                    if (piece != null)
                        return;
                }

                r.CastleMoves.Add(newRookPos);

                _AllMoves.Add(newKingPos);
                _CastleMoves.Add(new CastleMove(this, newKingPos, r, newRookPos));
            }
        }


        public void FindPins()
        {
            FindStraightPinns();
            FindDiagonalPins();
        }

        //TODO: Stop it, GET SOME HELP!!!!!!!!!
        private void FindStraightPinns()
        {
            // Pin to the left
            _emptySquares.Clear();
            _pieceInWay = null;
            for (int i = Position.X - 1; i > -1; i--)
            {
                if (!PinPosition(new(Position.Y, i), new() { "r", "q" }))
                {
                    break;
                }
            }

            // Pin to the right
            _emptySquares.Clear();
            _pieceInWay = null;
            for (int i = Position.X + 1; i < 8; i++)
            {
                if (!PinPosition(new(Position.Y, i), new() { "r", "q" }))
                {
                    break;
                }
            }

            // Pin up
            _emptySquares.Clear();
            _pieceInWay = null;
            for (int i = Position.Y - 1; i > -1; i--)
            {
                if (!PinPosition(new(i, Position.X), new() { "r", "q" }))
                {
                    break;
                }
            }

            // Pin down
            _emptySquares.Clear();
            _pieceInWay = null;
            for (int i = Position.Y + 1; i < 8; i++)
            {
                if (!PinPosition(new(i, Position.X), new() { "r", "q" }))
                {
                    break;
                }
            }
        }


        private void FindDiagonalPins()
        {
            int row = Position.Y;

            //LeftDown
            _emptySquares.Clear();
            _pieceInWay = null;
            for (int i = Position.X - 1; i > -1; i--)
            {
                if (!PinPosition(new(++row, i), new() { "q", "b" }))
                {
                    break;
                }
            }

            //RightUp
            row = Position.Y;
            _emptySquares.Clear();
            _pieceInWay = null;
            for (int i = Position.X + 1; i < 8; i++)
            {                
                if (!PinPosition(new(--row, i), new() { "q", "b" }))
                {
                    break;
                }
            }

            //LeftUp
            row = Position.Y;
            _emptySquares.Clear();
            _pieceInWay = null;
            for (int i = Position.X - 1; i > -1; i--)
            {
                if (!PinPosition(new(--row, i), new() { "q", "b" }))
                {
                    break;
                }
            }

            //RightDown
            row = Position.Y;
            _emptySquares.Clear();
            _pieceInWay = null;
            for (int i = Position.X + 1; i < 8; i++)
            {
                if (!PinPosition(new(++row, i), new() { "q", "b" }))
                {
                    break;
                }
            }
        }

        private ChessPiece _pieceInWay = null;
        private List<Square> _emptySquares = new();

        private bool PinPosition(Square position, List<string> pinerFENTypes)
        {
            ChessPiece piece = null;

            try
            {
                piece = ChessBoard._ChessBoard.GetPieceAt(position);
            }
            catch
            {
                return true;
            }

            if (piece != null)
            {
                if (Color == piece.Color)
                {
                    _pieceInWay = piece;
                }
                else
                {
                    if (pinerFENTypes.Contains(piece.FENType.ToString().ToLower()))
                    {
                        if (_pieceInWay == null)
                        {
                            Check(piece, _emptySquares);
                        }
                        else
                        {
                            PinnedPieces.Add(_pieceInWay);

                            _pieceInWay.IsPinned = true;
                            TryEnablePinMoves(piece, _pieceInWay, _emptySquares);
                        }
                    }
                    return false;
                }
            }
            else
            {
                _emptySquares.Add(position);
            }
            
            return true;
        }

        private void TryEnablePinMoves(ChessPiece pinner, ChessPiece pinnedPiece, List<Square> emptySquares)
        {
            if (pinnedPiece._LegalCaptures.Contains(pinner))
            {
                pinnedPiece._LegalCaptures.Clear();
                pinnedPiece._LegalCaptures.Add(pinner);

                pinnedPiece._LegalMoves.Clear();

                foreach (Square pos in emptySquares)
                {
                    if (pinnedPiece._LegalMoves.Contains(pos))
                    {
                        pinnedPiece._LegalMoves.Add(pos);
                    }
                }

                pinnedPiece._AllMoves.Clear();
                pinnedPiece._AllMoves.AddRange(pinnedPiece._LegalMoves);
                pinnedPiece._AllMoves.Add(pinner.Position);
            }
            else
            {
                pinnedPiece.CanMove = false;
            }
        }

        public void FindChecks()
        {
            foreach (ChessPiece piece in ChessBoard._ChessBoard.PieceList)
            {
                if (piece._LegalCaptures.Contains(this))
                {
                    Check(piece);
;               }
            }
        }

        //TODO: Wubba Lubba dub-dub
        private void Check(ChessPiece checker, List<Square> emptyFields)
        {
            IsChecked = true;
            ChessBoard._ChessBoard.CheckMarker.Move(Position);

            CheckCount++;

            if (Color == PieceColor.White)
            {
                ChessGame._CurrentGame.WhiteChecked = true;
            }                
            else
            {
                ChessGame._CurrentGame.BlackChecked = true;
            }

            if (CheckCount < 2)
            {
                foreach (ChessPiece piece in ChessBoard._ChessBoard.PieceList)
                {
                    if (piece.Color == Color && !piece.IsKing)
                    {
                        piece._AllMoves.Clear();

                        UpdatePieceCaptures(piece, checker);
                        UpdatePieceMovement(piece, emptyFields);
                    }
                }
            }
        }
        public void Check(ChessPiece checker)
        {
            IsChecked = true;
            ChessBoard._ChessBoard.CheckMarker.Move(Position);

            CheckCount++;

            if (Color == PieceColor.White)
            {
                ChessGame._CurrentGame.WhiteChecked = true;
            }
            else
            {
                ChessGame._CurrentGame.BlackChecked = true;
            }

            if (CheckCount < 2)
            {
                foreach (ChessPiece piece in ChessBoard._ChessBoard.PieceList)
                {
                    if (piece.Color == Color && !piece.IsKing)
                    {
                        piece._AllMoves.Clear();

                        UpdatePieceCaptures(piece, checker);

                        piece._LegalMoves.Clear();
                        piece._AllMoves.Clear();
                    }
                }
            }
        }

        private void UpdatePieceCaptures(ChessPiece piece, ChessPiece checker)
        {
            List<ChessPiece> legalCaptures = new();

            if (piece._LegalCaptures.Contains(checker))
            {
                legalCaptures.Add(checker);
                piece._AllMoves.Add(checker.Position);
            }

            piece._LegalCaptures = legalCaptures;
        }

        private void UpdatePieceMovement(ChessPiece piece, List<Square> emptyFields)
        {
            List<Square> legalMoves = new();

            foreach (Square move in piece._LegalMoves)
            {
                if (emptyFields.Contains(move))
                {
                    legalMoves.Add(move);
                }
            }

            piece._LegalMoves = legalMoves;
            piece._AllMoves.AddRange(legalMoves);
        }
    }
}