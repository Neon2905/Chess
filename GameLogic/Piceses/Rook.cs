
namespace GameLogic
{
    public class Rook(Player color) : Piece
    {
        public override PieceType Type => PieceType.Rook;
        public override Player Color => color;

        private static readonly Direction[] directions = [ Direction.North, Direction.South, Direction.East, Direction.West ];

        public override Piece Copy() => new Rook(Color) { HasMoved = this.HasMoved };

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return MovePositionsInDirs(from, board, directions).Select(to => new NormalMove(from, to));
        }
    }
}
