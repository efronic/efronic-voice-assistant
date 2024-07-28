using Amazon.Polly;
using Amazon.Polly.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class SpeechSynthesizer
{
    private readonly AmazonPollyClient _pollyClient;
    private string _mpg123Path;

    public SpeechSynthesizer(string awsAccessKeyId, string awsSecretAccessKey, Amazon.RegionEndpoint region, string mpg123PathAppSettings = "mpg123")
    {
        _pollyClient = new AmazonPollyClient(awsAccessKeyId, awsSecretAccessKey, region);
        _mpg123Path = mpg123PathAppSettings;
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
            string mpg123Path = "mpg123"; // Default for Unix-like systems
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Adjust the path to where mpg123 is located on your Windows system
                mpg123Path = _mpg123Path;
            }


            // Play the audio file using mpg123
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = mpg123Path,
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
