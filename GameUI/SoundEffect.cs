using System.Diagnostics;
using System.IO;
using System.Windows.Media;

namespace GameUI
{
    public static class SoundEffect
    {
        private static string SoundPath => Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/Sound-Effects";
        public static void Play(Sound soundType = Sound.GameStart)
        {
            var mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri($"{SoundPath}/{soundType}.mp3"));
            mediaPlayer.Play();
        }
    }
}
