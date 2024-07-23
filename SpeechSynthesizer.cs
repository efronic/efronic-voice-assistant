using Amazon.Polly;
using Amazon.Polly.Model;
// using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

// converts text to speech using Amazon Polly
public class SpeechSynthesizer
{
    private readonly AmazonPollyClient _pollyClient;

    public SpeechSynthesizer(string awsAccessKeyId, string awsSecretAccessKey, Amazon.RegionEndpoint region)
    {
        _pollyClient = new AmazonPollyClient(awsAccessKeyId, awsSecretAccessKey, region);
    }

    public async Task SynthesizeSpeechAsync(string text , string filePath = "speech.mp3")
    {
        var synthesizeSpeechRequest = new SynthesizeSpeechRequest
        {
            Text = text,
            OutputFormat = OutputFormat.Mp3,
            VoiceId = VoiceId.Ruth,
            Engine = Engine.Generative
        };

        try
        {
            var synthesizeSpeechResponse = await _pollyClient.SynthesizeSpeechAsync(synthesizeSpeechRequest);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await synthesizeSpeechResponse.AudioStream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            // Play the audio file using Windows Media Player
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wmplayer",
                    Arguments = $"\"{filePath}\"",
                    UseShellExecute = true, // UseShellExecute is true for launching external apps
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
