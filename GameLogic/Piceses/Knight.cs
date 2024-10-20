namespace GameLogic
{
    public class Knight(Player color) : Piece
    {
        public override PieceType Type => PieceType.Knight;
        public override Player Color => color;
        public override Piece Copy() => new Knight(Color) { HasMoved = this.HasMoved };

        private static IEnumerable<Position> PotentialToPositions(Position from)
        {
            foreach (var vDir in new Direction[] { Direction.North, Direction.South })
            {
                foreach (var hDir in new Direction[] { Direction.West, Direction.East })
                {
                    yield return from + 2 * vDir + hDir;
                    yield return from + 2 * hDir + vDir;
                }
            }
        }

        private IEnumerable<Position> MovePositions(Position from, Board board)
        {
            return PotentialToPositions(from).Where(position => Board.IsInside(position)
                && (board.IsEmpty(position) || board[position].Color != this.Color));
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return MovePositions(from, board).Select(to => new NormalMove(from, to));
        }
    }
}
