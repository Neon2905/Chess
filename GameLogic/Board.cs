namespace GameLogic
{
    public class Board
    {
        private readonly Piece[,] pieces = new Piece[8, 8];

        private readonly Dictionary<Player, Position> pawnSkipPositions = new() { { Player.White, null }, { Player.Black, null } };
        public Piece this[int row, int col]
        {
            get => pieces[row, col];
            set => pieces[row, col] = value;
        }
        public Piece this[Position pos]
        {
            get => pieces[pos.Row, pos.Column];
            set => pieces[pos.Row, pos.Column] = value;
        }

        public Position GetPawnSkipPosition(Player player) => pawnSkipPositions[player];

        public Position SetPawnSkipPosition(Player player, Position pos) => pawnSkipPositions[player] = pos;

        public static Board Initial()
        {
            var board = new Board();
            board.AddStartPieces();
            return board;
        }

        private void AddStartPieces()
        {
            this[0, 0] = new Rook(Player.Black);
            this[0, 1] = new Knight(Player.Black); 
            this[0, 2] = new Bishop(Player.Black);
            this[0, 3] = new Queen(Player.Black);
            this[0, 4] = new King(Player.Black);
            this[0, 5] = new Bishop(Player.Black);
            this[0, 6] = new Knight(Player.Black);
            this[0, 7] = new Rook(Player.Black);
            
            this[7, 0] = new Rook(Player.White);
            this[7, 1] = new Knight(Player.White);
            this[7, 2] = new Bishop(Player.White);
            this[7, 3] = new Queen(Player.White);
            this[7, 4] = new King(Player.White);
            this[7, 5] = new Bishop(Player.White);
            this[7, 6] = new Knight(Player.White);
            this[7, 7] = new Rook(Player.White);

            foreach (var col in Enumerable.Range(0, 8))
            {
                this[1, col] = new Pawn(Player.Black);
                this[6, col] = new Pawn(Player.White);
            }
        }

        public static bool IsInside(Position pos) => pos.Row >=0 && pos.Row < 8 && pos.Column >=0 && pos.Column < 8;

        public bool IsEmpty(Position pos) => this[pos] == null;

        public IEnumerable<Position> PiecePositions
        {
            get
            {
                foreach (var col in Enumerable.Range(0, 8))
                {
                    foreach (var row in Enumerable.Range(0, 8))
                    {
                        var pos = new Position(row, col);
                        if (!IsEmpty(pos)) yield return pos;
                    }
                }
            }
        }

        public Position KingPosition(Player player) => FindPiece(player, PieceType.King);

        public IEnumerable<Position> PiecePositionsFor(Player player) => PiecePositions.Where(pos => this[pos].Color == player);

        public bool IsInCheck(Player player) 
            =>  PiecePositionsFor(player.Opponent()).Any(pos =>
                {
                    var piece = this[pos];
                    return piece.CanCaptureOpponentKing(pos, this);
                });

        public Board Copy()
        {
            var copy = new Board();

            foreach (var pos in PiecePositions)
                copy[pos] = this[pos].Copy();

            return copy;
        }

        public Counting CountPieces()
        {
            Counting counting = new();

            foreach (var pos in PiecePositions)
            {
                var piece = this[pos];
                counting.Increment(piece.Color, piece.Type);
            }

            return counting;
        }

        public bool InsufficientMaterial()
        {
            Counting counting = CountPieces();

            return IsKingVKing(counting) || IsKingBishopVKing(counting) ||
                    IsKingKnightVKing(counting) || IsKingBishopVKingBishop(counting);
        }

        private static bool IsKingVKing(Counting counting) => counting.TotalCount == 2;

        private static bool IsKingBishopVKing(Counting counting) => 
            counting.TotalCount == 3 && (counting.White(PieceType.Bishop) == 1 || counting.Black(PieceType.Bishop) == 1);

        private static bool IsKingKnightVKing(Counting counting) => 
            counting.TotalCount == 3 && (counting.White(PieceType.Knight) == 1 || counting.Black(PieceType.Knight) == 1);

        private bool IsKingBishopVKingBishop(Counting counting)
        {
            if (counting.TotalCount != 4) return false;

            if (counting.White(PieceType.Bishop) != 1 || counting.Black(PieceType.Bishop) != 1) return false;

            var wBishopPos = FindPiece(Player.White, PieceType.Bishop);
            var bBishopPos = FindPiece(Player.Black, PieceType.Bishop);

            return wBishopPos.SquareColor == bBishopPos.SquareColor;
        }

        private Position FindPiece(Player color, PieceType type) =>
            PiecePositionsFor(color).First(pos => this[pos].Type == type);

        private bool IsUnmovedKingAndRook(Position kingPos, Position rookPos)
        {
            if (IsEmpty(kingPos) || IsEmpty(rookPos)) return false;

            var king = this[kingPos];
            var rook = this[rookPos];

            return king.Type == PieceType.King && rook.Type == PieceType.Rook &&
                    !king.HasMoved && !rook.HasMoved;
        }

        public bool CastleRightKS(Player player)
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 7)),
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 7)),
                _ => false
            };
        }

        public bool CastleRightQS(Player player)
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 0)),
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 0)),
                _ => false
            };
        }

        private bool HasPawnInPosition(Player player, Position[] pawnPositions, Position skipPos)
        {
            foreach (var pos in pawnPositions.Where(IsInside))
            {
                var piece = this[pos];
                if (piece == null || piece.Color != player || piece.Type != PieceType.Pawn) continue;

                EnPassant move = new(pos, skipPos);
                if (move.IsLegal(this)) return true;
            }

            return false;
        }

        public bool CanCaptureEnpassant(Player player)
        {
            Position skipPos = GetPawnSkipPosition(player.Opponent());

            if (skipPos == null) return false;

            Position[] pawnPositions = player switch
            {
                Player.White => [ skipPos + Direction.SouthWest, skipPos + Direction.SouthEast ],
                Player.Black => [ skipPos + Direction.NorthWest, skipPos + Direction.NorthEast ],
                _ => []
            };

            return HasPawnInPosition(player, pawnPositions, skipPos);
        }
    }
}
