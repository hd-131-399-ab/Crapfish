using Chess.Engine.Board;
using Chess.Engine.Pieces;

namespace Chess.Engine.Chess
{
    public class FENReader
    {
        public void LoadPosition(string fEN)
        {
            ClearPosition();

            Square position = new(0, 0);

            char[] fENTypes = fEN.ToCharArray();

            foreach (char fENType in fENTypes)
            {
                if (fENType == '/')
                {
                    position.Row++;
                    position.Column = 0;
                }
                else
                {
                    int fENNum;

                    if (int.TryParse(fENType.ToString(), out fENNum))
                    {
                        position.Column += fENNum;
                    }
                    else
                    {
                        ChessBoard._ChessBoard.AddChessPiece(fENType, GetColorFromFenType(fENType), position);
                        position.Column++;
                    }
                }
            }
        }

        public ChessPiece.PieceColor GetColorFromFenType(char fENType)
        {
            if (char.IsUpper(fENType))
            {
                return ChessPiece.PieceColor.White;
            }
            else
            {
                return ChessPiece.PieceColor.Black;
            }
        }

        public void ClearPosition()
        {
            ChessGame._CurrentGame.FEN = null;

            ChessBoard._ChessBoard.RemoveAllPieces();
        }
    }
}
