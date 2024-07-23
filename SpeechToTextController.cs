using System.Globalization;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Media;

public class SpeechToTextController
{
    private readonly ISpeechToText _speechToText;
    private string RecognitionText;

    public SpeechToTextController()
    {
        _speechToText = SpeechToText.Default;
    }

    public async Task<string> SpeechToTextAsync(CancellationToken cancellationToken)
    {
        var isGranted = await _speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            await Toast.Make("Permission not granted").Show(CancellationToken.None);
            return string.Empty;
        }

        var recognitionResult = await _speechToText.ListenAsync(
            CultureInfo.GetCultureInfo("en-US"),
            new Progress<string>(partialText =>
            {
                RecognitionText += partialText + " ";
            }), cancellationToken);

        if (recognitionResult.IsSuccessful)
        {
            RecognitionText = recognitionResult.Text;
        }
        else
        {
            await Toast.Make(recognitionResult.Exception?.Message ?? "Unable to recognize speech").Show(CancellationToken.None);
        }

        return RecognitionText;
    }
}
