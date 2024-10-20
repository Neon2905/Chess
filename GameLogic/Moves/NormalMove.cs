namespace GameLogic
{
    public class NormalMove(Position from, Position to) : Move
    {
        public override MoveType Type => MoveType.Normal;
        public override Position FromPosition => from;
        public override Position ToPosition => to;

        public override bool Execute(Board board)
        {
            Piece piece = board[FromPosition];
            bool capture = !board.IsEmpty(ToPosition);
            board[ToPosition] = piece;
            board[FromPosition] = null;
            piece.HasMoved = true;

            return capture || piece.Type == PieceType.Pawn;
        }
    }
}
