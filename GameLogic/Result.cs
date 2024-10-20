namespace GameLogic
{
    public class Result(Player winner, EndReason reason)
    {
        public Player Winner { get; } = winner;
        public EndReason Reason { get; } = reason;

        public static Result Win(Player winner) => new(winner, EndReason.Checkmate);

        public static Result Draw(EndReason reason) => new(Player.None, reason);
    }
}
