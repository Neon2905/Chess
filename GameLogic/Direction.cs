namespace GameLogic
{
    public class Direction (int rowDelta, int colDelta)
    {
        public static readonly Direction North = new(-1, 0);
        public static readonly Direction East = new(0, 1);
        public static readonly Direction South = new(1, 0);
        public static readonly Direction West = new(0, -1);
        public static readonly Direction NorthEast = North + East;
        public static readonly Direction NorthWest = North + West;
        public static readonly Direction SouthEast = South + East;
        public static readonly Direction SouthWest = South + West;

        public int RowDelta { get; } = rowDelta;
        public int ColumnDelta { get; } = colDelta;

        public static Direction operator +(Direction a, Direction b) => new( a.RowDelta + b.RowDelta, a.ColumnDelta + b.ColumnDelta);

        public static Direction operator *(int scalar, Direction dir) => new( scalar * dir.RowDelta, scalar * dir.ColumnDelta);
    }
}
