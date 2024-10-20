
namespace GameLogic
{
    public class Queen(Player color) : Piece
    {
        public override PieceType Type => PieceType.Queen;
        public override Player Color => color;

        private static readonly Direction[] directions = [ Direction.North, Direction.East, Direction.South, Direction.West,
                                        Direction.NorthEast, Direction.NorthWest, Direction.SouthEast, Direction.SouthWest ];

        public override Piece Copy() => new Queen(Color) { HasMoved = this.HasMoved };

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return MovePositionsInDirs(from, board, directions).Select(to => new NormalMove(from, to));
        }
    }
}
