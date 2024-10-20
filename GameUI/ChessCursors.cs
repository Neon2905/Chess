using System.IO;
using System.Windows;
using System.Windows.Input;

namespace GameUI
{
    public static class ChessCursors
    {
        public static readonly Cursor White = LoadCursor("Assets/CursorW.cur");
        public static readonly Cursor Black = LoadCursor("Assets/CursorB.cur");
        private static Cursor LoadCursor(string filePath)
        {
            Stream stream = Application.GetResourceStream(new Uri(filePath, UriKind.Relative)).Stream;
            return new Cursor(stream, true);
        }
    }
}
