using NAudio.Wave;

namespace GameLogic
{
    public static class SoundEffect
    {
        public static void Play()
        {
            using var audioFile = new AudioFileReader("H:\\C#_Games\\Chess\\GameLogic\\Sound-Effects\\castle.mp3");
            using var outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);
            outputDevice.Play();

            // Keep the program running while the sound is playing
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(1000); // reduce the interval as needed
            }
        }
    }
}
