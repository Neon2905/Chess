using GameLogic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameUI
{
    public static class Images
    {
        private static readonly Dictionary<PieceType, ImageSource> whiteSources = new()
        {
            { PieceType.Pawn, LoadImage("PawnW") },
            { PieceType.Bishop, LoadImage("BishopW") },
            { PieceType.Knight, LoadImage("KnightW") },
            { PieceType.Rook, LoadImage("RookW") },
            { PieceType.Queen, LoadImage("QueenW") },
            { PieceType.King, LoadImage("KingW") }
        };

        private static readonly Dictionary<PieceType, ImageSource> blackSources = new()
        {
            { PieceType.Pawn, LoadImage("PawnB") },
            { PieceType.Bishop, LoadImage("BishopB") },
            { PieceType.Knight, LoadImage("KnightB") },
            { PieceType.Rook, LoadImage("RookB") },
            { PieceType.Queen, LoadImage("QueenB") },
            { PieceType.King, LoadImage("KingB") }
        };

        private static ImageSource LoadImage(string imageName) => new BitmapImage(new Uri($"Assets/{imageName}.png", UriKind.RelativeOrAbsolute));

        public static ImageSource GetImage(Player color, PieceType type)
        {
            return color switch
            {
                Player.White => whiteSources[type],
                Player.Black => blackSources[type],
                _ => new BitmapImage()
            };
        }

        public static ImageSource GetImage(Piece piece) => piece is null ? new BitmapImage() : GetImage(piece.Color, piece.Type);
    }
}
