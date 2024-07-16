using Amazon.Polly;
using Amazon.Polly.Model;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;

public class SpeechSynthesizer
{
    private readonly AmazonPollyClient _pollyClient;

    public SpeechSynthesizer(string awsAccessKeyId, string awsSecretAccessKey, Amazon.RegionEndpoint region)
    {
        _pollyClient = new AmazonPollyClient(awsAccessKeyId, awsSecretAccessKey, region);
    }

    public async Task SynthesizeSpeechAsync(string text)
    {
        var synthesizeSpeechRequest = new SynthesizeSpeechRequest
        {
            Text = text,
            OutputFormat = OutputFormat.Mp3,
            VoiceId = VoiceId.Matthew
        };

        var synthesizeSpeechResponse = await _pollyClient.SynthesizeSpeechAsync(synthesizeSpeechRequest);

        using (var memoryStream = new MemoryStream())
        {
            synthesizeSpeechResponse.AudioStream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var waveStream = new Mp3FileReader(memoryStream))
            using (var waveOut = new WaveOutEvent())
            {
                waveOut.Init(waveStream);
                waveOut.Play();

                while (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(100);
                }
            }
        }
    }
}
