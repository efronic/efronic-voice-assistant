using Amazon.Polly;
using Amazon.Polly.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class SpeechSynthesizer
{
    private readonly AmazonPollyClient _pollyClient;

    public SpeechSynthesizer(string awsAccessKeyId, string awsSecretAccessKey, Amazon.RegionEndpoint region)
    {
        _pollyClient = new AmazonPollyClient(awsAccessKeyId, awsSecretAccessKey, region);
    }

    public async Task SynthesizeSpeechAsync(string text, string filePath = "speech.mp3")
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

            // Play the audio file using mpg123
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "mpg123",
                    Arguments = $"\"{filePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
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
