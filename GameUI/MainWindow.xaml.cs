using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using GameLogic;
using System.Windows.Input;

namespace GameUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8];
        private readonly Dictionary<Position, Move> moveCache = [];

        private GameState gameState;
        private Position selectedPosition = null;

        private bool IsOnMenuScreen => MenuContnainer.Content != null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();

            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
        }

        private void InitializeBoard()
        {
            foreach (var row in Enumerable.Range(0, 8))
            {
                foreach (var col in Enumerable.Range(0, 8))
                {
                    pieceImages[row, col] = new();
                    BoardGrid.Children.Add(pieceImages[row, col]);

                    Rectangle highlight = new();
                    highlights[row, col] = highlight;
                    HighlightGrid.Children.Add(highlight);
                }
            }
        }

        private void DrawBoard(Board board)
        {
            foreach (var row in Enumerable.Range(0, 8))
                foreach (var col in Enumerable.Range(0, 8))
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        pieceImages[row, col].Source = Images.GetImage(board[row, col]);
                    });
        }

        private void CacheMoves(IEnumerable<Move > moves)
        {
            moveCache.Clear();

            foreach (var move in moves)
                moveCache[move.ToPosition] = move;
        }

        private void ShowHighlights()
        {
            Color color = Color.FromArgb(150, 125, 255, 125);

            foreach (var to in moveCache.Keys)
                highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
        }

        private void HideHighlights()
        {
            foreach (var to in moveCache.Keys)
                highlights[to.Row, to.Column].Fill = Brushes.Transparent;
        }

        private void ShowWarnedPosition()
        {
            Color color = Color.FromArgb(200, 250, 20, 20);
            highlights[warnedPosition.Row, warnedPosition.Column].Fill = new SolidColorBrush(color);
        }

        private void HideWarnedPosition()
        {
            if (warnedPosition != null)
                highlights[warnedPosition.Row, warnedPosition.Column].Fill = Brushes.Transparent;
        }

        private void BorderGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsOnMenuScreen)
                return;

            Point point = e.GetPosition(BoardGrid);
            Position pos = ToSquarePositions(point);

            // If selected piece is already selected
            if (selectedPosition == pos)
            {
                selectedPosition = null;
                HideHighlights();
            }
            else if (selectedPosition == null || gameState.Board[pos]?.Color == gameState.CurrentPlayer)
                OnFromPositionSelected(pos);
            else
                OnToPositionSelected(pos);
        }

        private Position ToSquarePositions(Point point)
        {
            double squareSize = BorderGrid.ActualWidth / 8;
            int row = (int)(point.Y /squareSize);
            int col = (int)(point.X /squareSize);
            return new(row, col);
        }

        private void OnFromPositionSelected(Position pos)
        {
            //Hide previous highlights
            HideHighlights();

            IEnumerable<Move> moves = gameState.LegalMovesForPiece(pos);

            if (moves.Any())
            {
                selectedPosition = pos;
                CacheMoves(moves);
                ShowHighlights();
                SoundEffect.Play(Sound.PreMove);
            }
        }

        private void OnToPositionSelected(Position pos)
        {
            selectedPosition = null;
            HideHighlights();

            if (moveCache.TryGetValue(pos, out Move move))
            {
                if (move.Type == MoveType.PawnPromotion)
                    HandlePromotion(move.FromPosition, move.ToPosition);
                else
                    HandleMove(move);
            }
        }

        private void HandlePromotion(Position from, Position to)
        {
            pieceImages[to.Row, to.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn);
            pieceImages[from.Row, from.Column].Source = null;

            PromotionMenu promotionMenu = new(gameState.CurrentPlayer);
            MenuContnainer.Content = promotionMenu;

            promotionMenu.PieceSelected += type =>
            {
                MenuContnainer.Content = null;
                Move promotionMove = new PawnPromotion(from, to, type);
                HandleMove(promotionMove);
            };
        }

        private Position warnedPosition;
        private void HandleMove(Move move)
        {
            Sound soundType;

            var pieceInToPosition = gameState.Board[move.ToPosition];
            bool doesCaputre = pieceInToPosition != null;

            soundType = (doesCaputre) ? Sound.Capture
                        : (move.Type == MoveType.Normal) || (move.Type == MoveType.DoublePawn) 
                            ? (gameState.CurrentPlayer == gameState.StartPlayer) ? Sound.MoveSelf : Sound.MoveOpponent
                        : (move.Type == MoveType.PawnPromotion) ? Sound.Promote
                        : Sound.Castle;

            gameState.MakeMove(move);
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);

            // [This is dumb. Should make less stupid method]
            if (gameState.Board.IsInCheck(gameState.CurrentPlayer))
            {
                warnedPosition = gameState.CurrentKingPosition;
                ShowWarnedPosition();
                soundType = Sound.MoveCheck;
            }
            else
            {
                HideWarnedPosition();
            }

            SoundEffect.Play(soundType);
            if (gameState.IsGameOver) ShowGameOver();
        }

        private void SetCursor(Player player)
        {
            Cursor = player == Player.White ? ChessCursors.White : ChessCursors.Black;
        }

        private void ShowGameOver()
        {
            GameOverMenu gameOverMenu = new(gameState);
            MenuContnainer.Content = gameOverMenu;

            SoundEffect.Play(Sound.GameEnd);

            gameOverMenu.OptionSelected += option =>
            {
                if (option == Option.Restart)
                {
                    MenuContnainer.Content = null;
                    RestartGame();
                }
                else Application.Current.Shutdown();
            };
        }

        private void RestartGame()
        {
            selectedPosition = null;
            HideHighlights();
            HideWarnedPosition();
            moveCache.Clear();
            Player winner = gameState.Result?.Winner?? Player.White;
            gameState = new GameState(winner, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
            SoundEffect.Play();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!IsOnMenuScreen && e.Key == Key.Escape)
                ShowPauseMenu();
        }

        private void ShowPauseMenu()
        {
            var pauseMenu = new PauseMenu();
            MenuContnainer.Content = pauseMenu;

            pauseMenu.OptionSelected += option =>
            {
                MenuContnainer.Content = null;

                if (option == Option.Restart)
                    RestartGame();
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SoundEffect.Play();
        }
    }
}