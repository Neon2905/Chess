namespace GameLogic
{
    public class DoublePawn(Position from, Position to) : Move
    {
        public override MoveType Type => MoveType.DoublePawn;
        public override Position FromPosition => from;
        public override Position ToPosition => to;

        private readonly Position skippedPosition = new((from.Row + to.Row) / 2, from.Column);

        public override bool Execute(Board board)
        {
            Player player = board[FromPosition].Color;
            board.SetPawnSkipPosition(player, skippedPosition);
            new NormalMove(FromPosition, ToPosition).Execute(board);

            return true;
        }
    }
}
