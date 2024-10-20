namespace GameLogic
{
    public class PawnPromotion(Position from, Position to, PieceType newType) : Move
    {
        public override MoveType Type => MoveType.PawnPromotion;

        private readonly PieceType newType = newType;

        public override Position FromPosition => from;
        public override Position ToPosition => to;

        public override bool Execute(Board board)
        {
            var pawn = board[FromPosition];
            board[FromPosition] = null;

            Piece promotionPiece = CreatePromotionPiece(pawn.Color);
            promotionPiece.HasMoved = true;
            board[ToPosition] = promotionPiece;

            return true;
        }

        private Piece CreatePromotionPiece(Player color)
        {
            return newType switch
            {
                PieceType.Knight => new Knight(color),
                PieceType.Bishop => new Bishop(color),
                PieceType.Rook => new Rook(color),
                _ => new Queen(color)
            };
        }
    }
}
