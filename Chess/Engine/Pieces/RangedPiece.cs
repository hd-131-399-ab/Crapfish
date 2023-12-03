using Chess.Engine.Board;
using System.Collections.Generic;
using System.Numerics;

namespace Chess.Engine.Pieces
{
    public abstract class RangedPiece : ChessPiece
    {
        public List<Square> VirtualMoves { get; set; } = new();

        internal override List<Square> CalculateLegalMoves()
        {
            return CalculateAllLegalMoves();
        }

        internal abstract List<Square> CalculateAllLegalMoves();

        protected void CalculateStraightMoves()
        {
            _nextMoveIsVirtual = false;

            Vector2 left = new(-1, 0);
            Vector2 right = new(1, 0);
            Vector2 down = new(0, 1);
            Vector2 up = new(0, -1);

            CalculateLine(left);
            CalculateLine(right);
            CalculateLine(down);
            CalculateLine(up);
        }

        protected void CalculateDiagonalMoves()
        {
            _nextMoveIsVirtual = false;

            Vector2 leftDown = new(-1, 1);
            Vector2 rightUp = new(1, -1);
            Vector2 leftUp = new(-1, -1);
            Vector2 rightDown = new(1, 1);

            CalculateLine(leftDown);
            CalculateLine(rightUp);
            CalculateLine(leftUp);
            CalculateLine(rightDown);
        }

        private bool _nextMoveIsVirtual { get; set; }
        protected void CalculateLine(Vector2 vector)
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
