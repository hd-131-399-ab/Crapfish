using Chess.Engine.Board;
using Chess.Engine.Pieces;
using Chess.Engine.Pieces.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Chess.Engine.Chess
{
    public class Rules
    {
        public List<Square> Moves { get; set; }
        public List<Square> OpponentsMoves { get; set; }
        public List<Square> VirtualOpponentsMoves { get; set; }
        public List<Square> LegalCheckMoves { get; set; }

        public void OnMovesSwitched()
        {
            LegalCheckMoves = new();

            if (ChessGame.CurrentGame.WhiteToMove)
            {
                Moves = ChessGame.CurrentGame.RelevantWhiteMoves;
                OpponentsMoves = ChessGame.CurrentGame.RelevantBlackMoves;
                VirtualOpponentsMoves = ChessGame.CurrentGame.VirtualBlackMoves;
            }
            else
            {
                Moves = ChessGame.CurrentGame.RelevantBlackMoves;
                OpponentsMoves = ChessGame.CurrentGame.RelevantWhiteMoves;
                VirtualOpponentsMoves = ChessGame.CurrentGame.VirtualWhiteMoves;
            }

            King king = ChessBoard._ChessBoard.GetKing(ChessGame.CurrentGame.WhiteToMove);

            king.FindPins();
            RestrictKingMovement(king);

            HandleCheckMoves(king);

            TryEndgame(king);
        }

        private void RestrictKingMovement(King king)
        {
            foreach (Square move in king._AllMoves)
            {
                if (OpponentsMoves.Contains(move) || VirtualOpponentsMoves.Contains(move))
                {
                    ChessPiece piece = ChessBoard._ChessBoard.GetPieceAt(move);

                    if (piece != null)
                    {
                        king._LegalCaptures.Remove(piece);
                    }
                    else
                    {
                        king._LegalMoves.Remove(move);
                    }

                    //king._AllMoves.Remove(move);
                    Moves.Remove(move);
                }
            }
   
            foreach (ChessPiece piece in king._LegalCaptures.ToList())
            {
                if (piece.IsCovered)
                {
                    king._LegalCaptures.Remove(piece);
                    king._AllMoves.Remove(piece.Position);
                    Moves.Remove(piece.Position);
                }
            }

            CastleMove[] castleMoves = king._CastleMoves.ToArray();

            for (int i = 0; i < castleMoves.Length; i++)
            {
                if (OpponentsMoves.Contains(castleMoves[i].KingPos) || OpponentsMoves.Contains(castleMoves[i].RookPos))
                {
                    king._CastleMoves.Remove(castleMoves[i]);
                    king._AllMoves.Remove(castleMoves[i].KingPos);
                }
            }
        }

        private void HandleCheckMoves(King king)
        {
            if (king.CheckCount > 1)
            {
                foreach (ChessPiece piece in ChessBoard._ChessBoard.PieceList)
                {
                    if (piece.Color == king.Color && !piece.IsKing)
                    {
                        piece.CanMove = false;
                    }
                }
            }
        }

        //TODO: Gameloop
        private void TryEndgame(King king)
        {
            if (Moves.Count == 0 && king.IsChecked)
            {
                Win(Convert.ToBoolean(king.Color));
            }

            if (Moves.Count == 0 && !king.IsChecked)
            {
                Remis();
            }
        }

        private void Win(bool whiteWon)
        {
            if (!whiteWon)
            {
                MainWindow.WinnerLbl.Content = "I have the highround";
            }
            else
            {
                MainWindow.WinnerLbl.Content = "Execute Order 66";
            }

            MainWindow.WinnerGrd.Visibility = Visibility.Visible;
        }

        private void Remis()
        {
            MainWindow.WinnerLbl.Content = "Remis";
            MainWindow.WinnerGrd.Visibility = Visibility.Visible;
        }
    }
}
         