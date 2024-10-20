namespace GameLogic
{
    public class Bishop(Player color) : Piece
    {
        public override PieceType Type => PieceType.Bishop;
        public override Player Color => color;

        private static readonly Direction[] directions = [Direction.NorthWest, Direction.NorthEast, Direction.SouthWest, Direction.SouthEast];

        public override Piece Copy() => new Bishop(Color) { HasMoved = this.HasMoved };

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return MovePositionsInDirs(from, board, directions).Select(to => new NormalMove(from, to));
        }
    }
}
