using Chess.Engine.Board;
using Chess.Engine.CrapFish;
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
        private Evaluator _evaluator = new();

        public List<Square> Moves { get; set; }
        public List<Square> OpponentsMoves { get; set; }
        public List<Square> OpponentsPawnCaptures { get; set; }
        public List<Square> VirtualOpponentsMoves { get; set; }
        public List<Square> LegalCheckMoves { get; set; }

        public void OnMovesSwitched()
        {
            LegalCheckMoves = new();

            if (ChessGame._CurrentGame.WhiteToMove)
            {
                Moves = ChessGame._CurrentGame.RelevantWhiteMoves;
                OpponentsMoves = ChessGame._CurrentGame.RelevantBlackMoves;
                VirtualOpponentsMoves = ChessGame._CurrentGame.VirtualBlackMoves;
            }
            else
            {
                Moves = ChessGame._CurrentGame.RelevantBlackMoves;
                OpponentsMoves = ChessGame._CurrentGame.RelevantWhiteMoves;
                VirtualOpponentsMoves = ChessGame._CurrentGame.VirtualWhiteMoves;
            }
            OpponentsPawnCaptures = ChessGame._CurrentGame.OpponentPawnCaptures;
            
            King king = ChessBoard._ChessBoard.GetKing(ChessGame._CurrentGame.WhiteToMove);

            king.FindPins();
            RestrictKingMovement(king);

            HandleMultiCheckMoves(king);
            king.FindChecks();

            TryEndgame(king);
            //_evaluator.EvaluatePositions(1);
        }

        private void RestrictKingMovement(King king)
        {
            foreach (Square move in king._AllMoves)
            {                
                if (OpponentsMoves.Contains(move) || VirtualOpponentsMoves.Contains(move) || OpponentsPawnCaptures.Contains(move))
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
                    //sus Moves.Remove(move);
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

        private void HandleMultiCheckMoves(King king)
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
                MainWindow._MainWindow.winnerLbl.Content = "I have the highround";
            }
            else
            {
                MainWindow._MainWindow.winnerLbl.Content = "Execute Order 66";
            }

            MainWindow._MainWindow.winnerGrd.Visibility = Visibility.Visible;
        }

        private void Remis()
        {
            MainWindow._MainWindow.winnerLbl.Content = "Remis";
            MainWindow._MainWindow.winnerGrd.Visibility = Visibility.Visible;
        }
    }
}
         