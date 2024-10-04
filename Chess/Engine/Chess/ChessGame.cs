using Chess.Engine.Board;
using Chess.Engine.Pieces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Chess.Engine.Chess
{
    public class ChessGame
    {
        public static ChessGame _CurrentGame;

        private Rules _rules = new();

        public enum GameMode { LocalMultiplayer, Multiplayer, SinglePlayer }
        private GameMode CurrentGameMode;

        public bool WhiteChecked { get; set; }
        public bool BlackChecked { get; set; }

        public enum CastleRights { Both, KingSide, QueenSide, None }

        public CastleRights WhiteCastleRights { get; set; } = CastleRights.Both;
        public CastleRights BlackCastleRights { get; set; } = CastleRights.Both;


        public List<Square> LegalWhiteMoves = new();
        public List<Square> RelevantWhiteMoves => LegalWhiteMoves.Distinct().ToList();
        public List<Square> VirtualWhiteMoves { get; set; } = new();


        public List<Square> LegalBlackMoves = new();
        public List<Square> OpponentPawnCaptures = new();
        public List<Square> RelevantBlackMoves => LegalBlackMoves.Distinct().ToList();
        public List<Square> VirtualBlackMoves { get; set; } = new();
        
        public string FEN { get; set; }

        public bool WhiteToMove = false;

        public ChessGame(string fEN, GameMode gameMode)
        {
            _CurrentGame = this;
            LegalWhiteMoves = new();
            LegalBlackMoves = new();

            CurrentGameMode = gameMode;

            ChessBoard._ChessBoard = new();

            FEN = fEN;

            //FEN = "3R3R/3K/8/3r1rk/8/8/8/8";

            //promotion
            //FEN = "8/PPP1PPPP/8/8/k2K/8/pppppppp/8";

            //pinn
            //FEN = "8/8/3rbr/3bkb/3rbr/8/8/KQ";

            //mixed
            //FEN = "r5/8/1b2QR/B5r/8/p2P3N/1k2K/8";

            //en passant
            //FEN = "k2K/8/8/8/p1p1p1p1/8/PPPPPPPP/8";

            //mate
            //FEN = "K/4rr/8/8/8/8/7k";

            //empty castle stiuation
            //FEN = "r3k2r/8/8/8/8/8/8/R3K2R";

            //double check(black to move)
            //FEN = "8/8/8/8/Rrb3K/1k/8/4R";

            //easy check
            //FEN = "3k2BB/8/3PPPP/4QQ/4RRK/8/8/2rrbbpq";
            WhiteToMove = true;

            new BoardMaker("#909090", "#505050");

            FENReader fENReader = new();
            fENReader.LoadPosition(FEN);

            ChessBoard._ChessBoard.SelectionGrd.CreateNewSelectionGrd(Visibility.Hidden);
            ChessBoard._ChessBoard.CheckMarker = new(Visibility.Hidden);

            InitNewGame();
        }

        private void InitNewGame()
        {
            MoveGrid.DestroyAll();
            CastleGrid.DestroyAll();

            HandlePieceMovement(WhiteToMove);

            ChessBoard._ChessBoard.GetEveryMove(out LegalWhiteMoves, out LegalBlackMoves, out OpponentPawnCaptures);

            _rules.OnMovesSwitched();
        }

        public void SwitchMoves()
        {
            MoveGrid.DestroyAll();
            CastleGrid.DestroyAll();

            VirtualWhiteMoves.Clear();
            VirtualBlackMoves.Clear();
            ChessBoard._ChessBoard.SelectedPiece = null;

            WhiteToMove = !WhiteToMove;

            switch (CurrentGameMode)
            {
                case GameMode.LocalMultiplayer:
                    SwitchLocalMultiplayerMoves();
                    break;
                case GameMode.Multiplayer:
                    break;
                case GameMode.SinglePlayer:
                    SwitchSingleplayerMoves();
                    break;
            }

            _rules.OnMovesSwitched();
        }        

        private void SwitchLocalMultiplayerMoves()
        {            
            HandlePieceMovement(WhiteToMove);

            ChessBoard._ChessBoard.GetEveryMove(out LegalWhiteMoves, out LegalBlackMoves, out OpponentPawnCaptures);
            ChessBoard._ChessBoard.SelectionGrd.SwitchVisibility();
        }

        private void SwitchMultiplayerMoves()
        {

        }

        private void SwitchSingleplayerMoves()
        {
            if (WhiteToMove)
            {
                SwitchLocalMultiplayerMoves();
            }
            else
            {
                //get move
            }
        }

        private void HandlePieceMovement(bool prepareForWhiteMove)
        {
            bool allowWhiteToMove;
            bool allowBlackToMove;

            allowBlackToMove = !prepareForWhiteMove;
            allowWhiteToMove = prepareForWhiteMove;

            foreach (ChessPiece piece in ChessBoard._ChessBoard.PieceList)
            {
                if (piece.Color == ChessPiece.PieceColor.White)
                {
                    piece.CanMove = allowWhiteToMove;
                }
                else
                {
                    piece.CanMove = allowBlackToMove;
                }

                piece.IsPinned = false;
                piece.IsCovered = false;
            }
        }
    }
}
