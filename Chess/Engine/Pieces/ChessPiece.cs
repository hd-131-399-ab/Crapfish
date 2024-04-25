using Chess.Engine.Board;
using Chess.Engine.Pieces.ChessPieces;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using static Chess.Engine.Board.MoveGrid;

namespace Chess.Engine.Pieces
{
    public abstract class ChessPiece
    {
        public Image Piece { get; set; }

        private Square _Position;
        public Square Position
        {
            get => _Position;

            set
            {
                if (Piece != null)
                {
                    ChessBoard._ChessBoard.SetPieceToSquare(this, value);

                    Grid.SetRow(Piece, value.Y);
                    Grid.SetColumn(Piece, value.X);
                }

                _Position = value;
            }
        }

        public enum PieceColor { Black, White }
        public PieceColor Color;

        public bool CanMove { get; set; }
        public char FENType { get; set; }
        public int Value { get; set; }

        public enum AttackType { DiagonalPierce, StraightPierce, HybridPierce, Diagonal }
        public AttackType AttackPattern { get; set; }

        public bool IsPinned { get; set; }
        public bool Moved { get; set; }
        public bool IsCovered { get; set; }
        public bool IsKing => FENType.ToString().ToLower() == "k";

        public List<Square> _AllMoves = new();
        public List<Square> _LegalMoves = new();
        public List<ChessPiece> _LegalCaptures = new();

        protected abstract Image Create();
        internal abstract List<Square> CalculateLegalMoves();

        public void MovePiece(Square newPosition)
        {
            Position = newPosition;
            Moved = true;

            //TODO: Print and frame this Methode!
            //ChessBoard.SetPieceToSquare(this, newPosition);
            
            //ChessBoard._ChessBoard.SelectionGrd.SwitchVisibility();
        }

        public void Capture(ChessPiece capturedPiece)
        {
            Square destinationPosition = capturedPiece.Position;

            ChessBoard._ChessBoard.RemovePieceAt(destinationPosition);
            ChessBoard._ChessBoard.SetPieceToSquare(this, destinationPosition);

            Position = destinationPosition;
            Moved = true;
        }
        
        private void ShowLegalMoves()
        {
            foreach (Square move in _LegalMoves)
            {
                new MoveGrid(move, MoveTypes.Move);
            }

            foreach (ChessPiece piece in _LegalCaptures)
            {
                new MoveGrid(piece.Position, MoveTypes.Capture);
            }

            if (IsKing)
            {
                King king = (King)this;

                foreach (CastleMove castleMove in king._CastleMoves)
                {
                    ShowCastleMoves(castleMove.KingPos, castleMove);
                }
            }

            if (this is Pawn pawn)
            {
                if (pawn._EnPassantCapture != null)
                {
                    new EnPassantGrid(pawn._EnPassantCapture.Position, pawn._EnPassantPosition, MoveTypes.EnPassant);
                }
            }
        }

        private void ShowCastleMoves(Square pos, CastleMove castleMove)
        {
            new CastleGrid(pos, castleMove);
        }

        internal void OnChessPiece_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChessPiece piece = this;

            if (piece.CanMove)
            {
                MoveGrid.DestroyAll();
                CastleGrid.DestroyAll();

                ChessBoard._ChessBoard.SelectedPiece = piece;

                ChessBoard._ChessBoard.SelectionGrd.Move(Position);

                ShowLegalMoves();
            }
        }
    }
}
