using Chess.Engine.Pieces.ChessPieces;
using System;
using System.Collections;
using System.IO;

namespace Chess.Engine.Board
{
    public struct CastleMove
    {
        public Square KingPos;
        public Square RookPos;
        public King King;
        public Rook Rook;

        public CastleMove(King king, Square kingPos, Rook rook, Square rookPos)
        {
            King = king;
            KingPos = kingPos;

            Rook = rook;
            RookPos = rookPos;
        }
    }
}