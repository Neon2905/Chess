namespace GameLogic
{
    public enum Player
    {
        None,
        White,
        Black
    }

    public static class PlayerExtension
    {
        public static Player Opponent(this Player player) =>
            player switch
            {
                Player.White => Player.Black,
                Player.Black => Player.White,
                _ => Player.None
            };
    }
}
