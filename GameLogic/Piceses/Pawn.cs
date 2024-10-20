namespace GameLogic
{
    public class Pawn(Player color) : Piece
    {
        public override PieceType Type => PieceType.Pawn;
        public override Player Color => color;

        private Direction Forward => Color is Player.White ? Direction.North : Direction.South;

        public override Piece Copy() => new Pawn(Color) { HasMoved = this.HasMoved };

        private static bool CanMoveTo(Position pos, Board board) => Board.IsInside(pos) && board.IsEmpty(pos);

        private bool CanCaptureAt(Position pos, Board board)
        {
            if (!Board.IsInside(pos) || board.IsEmpty(pos)) 
                return false;

            return board[pos].Color != this.Color;
        }

        private static IEnumerable<Move> PromotionMoves(Position from, Position to)
        {
            yield return new PawnPromotion(from, to, PieceType.Knight);
            yield return new PawnPromotion(from, to, PieceType.Bishop);
            yield return new PawnPromotion(from, to, PieceType.Rook);
            yield return new PawnPromotion(from, to, PieceType.Queen);
        }

        private IEnumerable<Move> ForwardMoves(Position from, Board board)
        {
            Position oneMovePos = from + Forward;

            if (CanMoveTo(oneMovePos, board))
            {
                if (oneMovePos.Row == 0 || oneMovePos.Row == 7)
                    foreach (var proMove in PromotionMoves(from, oneMovePos)) yield return proMove;
                else
                    yield return new NormalMove(from, oneMovePos);

                Position twoMovePos = oneMovePos + Forward;

                if (!HasMoved && CanMoveTo(twoMovePos, board))
                    yield return new DoublePawn(from, twoMovePos);
            }
        }

        private IEnumerable<Move> DiagonalMoves(Position from, Board board)
        {
            foreach (var direction in new Direction[] { Direction.West, Direction.East })
            {
                Position to = from + Forward + direction;

                if (to == board.GetPawnSkipPosition(Color.Opponent()))
                    yield return new EnPassant(from, to);

                else if (CanCaptureAt(to, board)) 
                    if (to.Row == 0 || to.Row == 7)
                        foreach (var proMove in PromotionMoves(from, to)) yield return proMove;
                    else
                        yield return new NormalMove(from, to);
            }
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return ForwardMoves(from, board).Concat(DiagonalMoves(from, board));
        }

        public override bool CanCaptureOpponentKing(Position from, Board board)
        {
            return DiagonalMoves(from, board).Any(move =>
            {
                var piece = board[move.ToPosition];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
