namespace GameLogic
{
    public class EnPassant(Position from, Position to) : Move
    {
        public override MoveType Type => MoveType.EnPassant;
        public override Position FromPosition => from;
        public override Position ToPosition => to;

        private readonly Position capturePosition = new(from.Row, to.Column);

        public override bool Execute(Board board)
        {
            new NormalMove(FromPosition, ToPosition).Execute(board);
            board[capturePosition] = null;

            return true;
        }
    }
}
