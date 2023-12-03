using Chess.Engine.Board;
using Chess.Engine.Chess;
using Chess.Engine.Pieces;

namespace Chess.Engine.CrapFish
{
    internal class Evaluator
    {
        public float Evaluation { get; set; }

        public Evaluator(int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                EvaluatePosition(ChessBoard._ChessBoard.Pieces);
            }
        }

        public void EvaluatePosition(ChessPiece[,] position)
        {
            foreach (ChessPiece piece in ChessBoard._ChessBoard.PieceList)
            {
                if (piece.Color == ChessPiece.PieceColor.White)
                {
                    if (!piece.IsPinned)
                    {
                        Evaluation += piece.Value;
                    }

                    if (piece.IsCovered)
                    {
                        Evaluation += .5f;
                    }

                    Evaluation += piece._LegalCaptures.Count / 5;
                    Evaluation += piece._LegalMoves.Count / 10;
                }
                else
                {
                    if (!piece.IsPinned)
                    {
                        Evaluation -= piece.Value;
                    }

                    if (piece.IsCovered)
                    {
                        Evaluation -= .5f;
                    }

                    Evaluation -= piece._LegalCaptures.Count / 5;
                    Evaluation -= piece._LegalMoves.Count / 10;
                }
            }
        }
    }
}
