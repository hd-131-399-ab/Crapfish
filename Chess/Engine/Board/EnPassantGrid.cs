using Chess.Engine.Chess;
using Chess.Engine.Pieces;
using Chess.Engine.Pieces.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Engine.Board
{
    public class EnPassantGrid : MoveGrid
    {
        public Square CapturePos { get; set; }

        public EnPassantGrid(Square capture, Square position, MoveTypes moveType) : base(position, moveType)
        {
            CapturePos = capture;
        }

        protected override void ClickBehaviour()
        {
            ChessPiece piece = ChessBoard._ChessBoard.SelectedPiece;

            piece.MovePiece(Position);

            ChessBoard._ChessBoard.RemovePieceAt(CapturePos);
        }
    }
}
