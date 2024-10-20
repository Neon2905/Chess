namespace GameLogic
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Player StartPlayer { get; private set; }
        public Result Result { get; private set; } = null;
        public bool IsGameOver => Result != null;
        public Position CurrentKingPosition => Board.KingPosition(CurrentPlayer);

        private int noCaptureMoves = 0;
        private bool IsFiftyMoveRule => (noCaptureMoves / 2) == 50;
        private bool IsThreefoldRepetition => stateHistory[stateString] == 3;
        private string stateString;

        private readonly Dictionary<string, int> stateHistory = [];

        public GameState(Player player, Board board)
        {
            CurrentPlayer = StartPlayer = (player != Player.None) ? player : Player.White;
            Board = board;

            stateString = new StateString(CurrentPlayer, board).ToString();
            stateHistory[stateString] = 1;
        }

        public IEnumerable<Move> LegalMovesForPiece(Position position)
        {
            if (Board.IsEmpty(position) || Board[position].Color != CurrentPlayer)
                return [];

            Piece piece = Board[position];
            return piece.GetMoves(position, Board).Where(move => move.IsLegal(Board));
        }

        public void MakeMove(Move move)
        {
            Board.SetPawnSkipPosition(CurrentPlayer, null);
            bool captureOrPawn = move.Execute(Board);

            if (captureOrPawn)
            {
                noCaptureMoves = 0;
                stateHistory.Clear();
            }
            else
                noCaptureMoves++;

            CurrentPlayer = CurrentPlayer.Opponent();
            UpdateStateString();
            CheckForGameOver();
        }

        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos => Board[pos].GetMoves(pos, Board) );

            return moveCandidates.Where(move => move.IsLegal(Board));
        }

        private void CheckForGameOver()
        {
            if (!AllLegalMovesFor(CurrentPlayer).Any())
                if (Board.IsInCheck(CurrentPlayer))
                    Result = Result.Win(CurrentPlayer.Opponent());
                else
                    Result = Result.Draw(EndReason.Stalemate);
            else if (Board.InsufficientMaterial())
                Result = Result.Draw(EndReason.InsufficientMaterial);
            else if (IsFiftyMoveRule)
                Result = Result.Draw(EndReason.InsufficientMaterial);
            else if (IsThreefoldRepetition)
                Result = Result.Draw(EndReason.ThreefoldRepetition);
        }

        private void UpdateStateString()
        {
            stateString = new StateString(CurrentPlayer, Board).ToString();

            if (!stateHistory.ContainsKey(stateString))
                stateHistory[stateString] = 1;
            else
                stateHistory[stateString]++;
        }
    }
}
