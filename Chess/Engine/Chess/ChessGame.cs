using Chess.Engine.Board;
using Chess.Engine.CrapFish;
using Chess.Engine.Pieces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Chess.Engine.Chess
{
    public class ChessGame
    {
        public static ChessGame CurrentGame;

        private Rules _rules = new();

        public enum GameMode { LocalMultiplayer, Multiplayer, SinglePlayer }
        private GameMode CurrentGameMode;

        public bool WhiteChecked { get; set; }
        public bool BlackChecked { get; set; }

        public enum CastleRights { KingQueen, KingSide, QueenSide, None }

        public CastleRights WhiteCastleRights { get; set; } = CastleRights.KingQueen;
        public CastleRights BlackCastleRights { get; set; } = CastleRights.KingQueen;


        public List<Square> LegalWhiteMoves = new();
        public List<Square> RelevantWhiteMoves => LegalWhiteMoves.Distinct().ToList();
        public List<Square> VirtualWhiteMoves { get; set; } = new();


        public List<Square> LegalBlackMoves = new();
        public List<Square> RelevantBlackMoves => LegalBlackMoves.Distinct().ToList();
        public List<Square> VirtualBlackMoves { get; set; } = new();
        
        public string FEN { get; set; }

        public bool WhiteToMove = true;

        public ChessGame(string fEN, GameMode gameMode)
        {
            CurrentGame = this;
            CurrentGameMode = gameMode;

            ChessBoard._ChessBoard = new();

            FEN = fEN;

            //FEN = "3r3r/3k/8/3R1RK/8/8/8/8";

            //promotion
            //FEN = "8/PPP1PPPP/8/8/k2K/8/pppppppp/8";

            //pinn
            //FEN = "8/8/3rbr/3bkb/3rbr/8/8/KQ";

            //mixed
            //FEN = "r5/8/1b2QR/B5r/8/p2P3N/1k2K/8";

            //en pasant
            //FEN = "k2K/8/8/8/p1p1p1p1/8/PPPPPPPP/8";

            //mate
            //FEN = "K/4rr/8/8/8/8/7k";

            //empty castle stiuation
            //FEN = "r3k2r/8/8/8/8/8/8/R3K2R";


            //double check(black to move)
            //FEN = "8/8/8/8/Rrb3K/1k/8/4R";

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

            ChessBoard._ChessBoard.GetEveryMove(out LegalWhiteMoves, out LegalBlackMoves);

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

            ChessBoard._ChessBoard.GetEveryMove(out LegalWhiteMoves, out LegalBlackMoves);
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
                Evaluator evaluator = new(1);
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
