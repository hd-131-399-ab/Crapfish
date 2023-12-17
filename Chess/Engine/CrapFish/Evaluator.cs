using Chess.Engine.Board;
using Chess.Engine.Chess;
using Chess.Engine.Pieces;

namespace Chess.Engine.CrapFish
{
    internal class Evaluator
    {
        public float Evaluation { get; set; }

        public void EvaluatePositions(int depth)
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
                }
                else
                {
                    if (!piece.IsPinned)
                    {
                        Evaluation -= piece.Value;
                    }
                }
            }
        }

        public void UpdateEvaluation()
        {

        }
    }
}
