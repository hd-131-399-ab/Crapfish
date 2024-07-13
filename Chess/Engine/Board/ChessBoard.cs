using Chess.Engine.Pieces;
using Chess.Engine.Pieces.ChessPieces;
using System;
using System.Collections.Generic;

namespace Chess.Engine.Board
{
    public class ChessBoard
    {
        public static ChessBoard _ChessBoard;

        public ChessPiece[,] Pieces { get; set; } = new ChessPiece[8, 8];
        public List<ChessPiece> PieceList { get; set; } = new();

        public ChessPiece SelectedPiece { get; set; }
        public SelectionGrid SelectionGrd { get; set; } = new();

        public CheckMarker CheckMarker { get; set; }

        public List<Square> LegalMoves { get; set; } = new();

        private Dictionary<char, Type> FENAssignment = new()
        {
            { 'r', typeof(Rook) },
            { 'n', typeof(Knight) },
            { 'b', typeof(Bishop) },
            { 'q', typeof(Queen) },
            { 'k', typeof(King) },
            { 'p', typeof(Pawn) },
            { 'R', typeof(Rook) },
            { 'N', typeof(Knight) },
            { 'B', typeof(Bishop) },
            { 'Q', typeof(Queen) },
            { 'K', typeof(King) },
            { 'P', typeof(Pawn) }
        };

        public ChessPiece GetPieceAt(Square position)
        {
            try
            {
                return Pieces[position.Row, position.Column];
            }
            catch
            {
                return null;
            }
        }

        public ChessPiece AddChessPiece(char fEN, ChessPiece.PieceColor color, Square position)
        {
            ChessPiece piece = Activator.CreateInstance(FENAssignment[fEN], position, color) as ChessPiece;
 
            Pieces[position.Row, position.Column] = piece;
            PieceList.Add(piece);

            return piece;
        }        

        public King GetKing(bool returnWhiteKing)
        {
            King king = null;

            foreach (ChessPiece piece in PieceList)
            {
                if (piece.IsKing && (int)piece.Color == Convert.ToInt32(returnWhiteKing))
                {
                    king = (King)piece;
                }
            }

            return king;
        }

        public King GetKing(ChessPiece.PieceColor kingColor)
        {
            King king = null;

            foreach (ChessPiece piece in PieceList)
            {
                if (piece.IsKing && piece.Color == kingColor)
                {
                    king = (King)piece;
                }
            }

            return king;
        }

        public void SetPieceToSquare(ChessPiece piece, Square destinationSquare)
        {
            Pieces[piece.Position.Row, piece.Position.Column] = null;
            Pieces[destinationSquare.Row, destinationSquare.Column] = piece;
        }

        public void RemovePieceAt(Square position)
        {
            ChessPiece piece = GetPieceAt(position);

            MainWindow._MainWindow.chessBoard.Children.Remove(piece.Piece);

            PieceList.Remove(piece);
            Pieces[position.Row, position.Column] = null;
        }

        public void RemoveAllPieces()
        {           
            foreach (ChessPiece piece in PieceList)
            {
                MainWindow._MainWindow.chessBoard.Children.Remove(piece.Piece);
            }

            PieceList.Clear();
            Pieces = new ChessPiece[8, 8];
        }

        public void GetEveryMove(out List<Square> whiteMoves, out List<Square> blackMoves, out List<Square> opponentPawnCaptures)
        {
            whiteMoves = new();
            blackMoves = new();
            opponentPawnCaptures = new();

            King whiteKing = null;
            King blackKing = null;

            foreach (ChessPiece piece in PieceList)
            {
                if (piece.IsKing)
                {
                    if (piece.Color == ChessPiece.PieceColor.White)
                    {
                        whiteKing = (King)piece;
                    }
                    else
                    {
                        blackKing = (King)piece;
                    }
                }
                else if (piece is Pawn pawn)
                {
                    pawn.CalculateLegalMoves();
                    opponentPawnCaptures.AddRange(pawn.CapturePositions);
                }

                if (piece.Color == ChessPiece.PieceColor.White)
                {
                    whiteMoves.AddRange(piece.CalculateLegalMoves());
                }
                else
                {
                    blackMoves.AddRange(piece.CalculateLegalMoves());
                }
            }

            whiteMoves.AddRange(whiteKing.CalculateLegalMoves());
            blackMoves.AddRange(blackKing.CalculateLegalMoves());
        }

        public bool ColorToBool(ChessPiece.PieceColor color)
        {
            if (color == ChessPiece.PieceColor.White)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
