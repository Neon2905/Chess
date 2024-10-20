namespace GameLogic
{
    public class Castle : Move
    {
        public override MoveType Type { get; }
        public override Position FromPosition { get; }
        public override Position ToPosition { get; }

        private readonly Direction kingMoveDir;
        private readonly Position rookFromPosition;
        private readonly Position rookToPosition;

        public Castle(MoveType type, Position kingPos)
        {
            Type = type;
            FromPosition = kingPos;

            if (type == MoveType.CastleKS)
            {
                kingMoveDir = Direction.East;
                ToPosition = new(kingPos.Row, 6);
                rookFromPosition = new(kingPos.Row, 7);
                rookToPosition = new(kingPos.Row, 5);
            }
            else if (type == MoveType.CastleQS)
            {
                kingMoveDir = Direction.West;
                ToPosition = new(kingPos.Row, 2);
                rookFromPosition = new(kingPos.Row, 0);
                rookToPosition = new(kingPos.Row, 3);
            }
        }

        public override bool Execute(Board board)
        {
            new NormalMove(FromPosition, ToPosition).Execute(board);
            new NormalMove(rookFromPosition, rookToPosition).Execute(board);

            return false;
        }

        public override bool IsLegal(Board board)
        {
            var player = board[FromPosition].Color;

            if (board.IsInCheck(player))
                return false;

            Board copy = board.Copy();
            var kingPositionInCopy = FromPosition;

            foreach (var i in Enumerable.Range(0, 2))
            {
                new NormalMove(kingPositionInCopy, kingPositionInCopy + kingMoveDir).Execute(copy);
                kingPositionInCopy += kingMoveDir;

                if (copy.IsInCheck(player))
                    return false;
            }

            return true;
        }
    }
}
