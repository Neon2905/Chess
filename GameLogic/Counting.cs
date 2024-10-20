namespace GameLogic
{
    public class Counting
    {
        private readonly Dictionary<PieceType, int> whiteCount = [];
        private readonly Dictionary<PieceType, int> blackCount = [];

        public int TotalCount { get; private set; }

        public Counting()
        {
            foreach (PieceType type in Enum.GetValues(typeof(PieceType)))
            {
                whiteCount[type] = 0;
                blackCount[type] = 0;
            }
        }

        public void Increment(Player color, PieceType type)
        {
            if (color == Player.White)
                whiteCount[type]++;
            else if (color == Player.Black)
                blackCount[type]++;
            TotalCount++;
        }

        public int White(PieceType type) => whiteCount[type];

        public int Black(PieceType type) => blackCount[type];
    }
}
