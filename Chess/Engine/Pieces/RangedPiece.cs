using Chess.Engine.Board;
using System.Collections.Generic;

namespace Chess.Engine.Pieces
{
    public abstract class RangedPiece : ChessPiece
    {
        public List<Square> VirtualMoves { get; set; } = new();

        internal override List<Square> CalculateLegalMoves() => CalculateAllLegalMoves();

        internal abstract List<Square> CalculateAllLegalMoves();

        #region Vecors
        static Vector _Left = new(-1, 0);
        static Vector _Right = new(1, 0);
        static Vector _Up = new(0, -1);
        static Vector _Down = new(0, 1);

        static Vector _LeftDown = new(-1, 1);
        static Vector _RightUp = new(1, -1);
        static Vector _LeftUp = new(-1, -1);
        static Vector _RightDown = new(1, 1);
        #endregion

        protected void CalculateStraightMoves()
        {
            _nextMoveIsVirtual = false;

            CalculateLine(_Left);
            CalculateLine(_Right);
            CalculateLine(_Down);
            CalculateLine(_Up);
        }

        protected void CalculateDiagonalMoves()
        {
            _nextMoveIsVirtual = false;

            CalculateLine(_LeftDown);
            CalculateLine(_RightUp);
            CalculateLine(_LeftUp);
            CalculateLine(_RightDown);
        }

        private bool _nextMoveIsVirtual { get; set; }
        protected void CalculateLine(Vector vector)
        {
            int newRow = Position.Row;
            int newColumn = Position.Column;

            for (int i = 0; i < 7; i++)
            {
                newRow = newRow + (int)vector.Y;
                newColumn = newColumn + (int)vector.X;

                if (newRow is > -1 and < 8 && newColumn is > -1  and < 8)
                {
                    if (!ReadPieceInformation(newRow, newColumn))
                        break;
                }
                else
                {
                    break;
                }
            }
        }

        protected virtual bool ReadPieceInformation(int row, int column)
        {
            bool goOn = true;

            ChessPiece piece = ChessBoard._ChessBoard.GetPieceAt(new Square(row, column));

            if (piece == null)
            {
                if (_nextMoveIsVirtual)
                {
                    VirtualMoves.Add(new(row, column));
                    _nextMoveIsVirtual = false;
                    goOn = false;
                }
                else
                {
                    _LegalMoves.Add(new(row, column));
                    _AllMoves.Add(new(row, column));
                }
            }
            else
            {
                if (_nextMoveIsVirtual)
                {
                    VirtualMoves.Add(new(row, column));
                    _nextMoveIsVirtual = false;
                    goOn = false;
                }

                if (piece.Color == Color)
                {
                    piece.IsCovered = true;
                    goOn = false;
                }
                else
                {
                    if (piece.IsKing)
                    {
                        _nextMoveIsVirtual = true;
                    }
                    else
                    {
                        _LegalCaptures.Add(piece);
                        _AllMoves.Add(piece.Position);
                        goOn = false;
                    }
                }
            }            

            return goOn;
        }
    }
}
