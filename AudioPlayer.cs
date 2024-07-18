using System;
using System.IO;
// using NAudio.Wave;
using System.Threading;
public class AudioPlayer
{
    public void PlayMp3(string filePath)
    {
        // Ensure the file exists
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found: " + filePath);
            return;
        }

        // using (var mp3FileReader = new Mp3FileReader(filePath))
        // {
        //     // `Mp3FileReader` implements `WaveStream`, so it can be directly used with `WaveOutEvent`
        //     using (var waveOut = new WaveOutEvent())
        //     {
        //         waveOut.Init(mp3FileReader);
        //         waveOut.Play();

        //         // Wait until playback completes
        //         while (waveOut.PlaybackState == PlaybackState.Playing)
        //         {
        //             Thread.Sleep(100);
        //         }
        //     }
        // }
    }
}
