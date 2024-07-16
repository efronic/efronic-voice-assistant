using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

public class WhisperClient
{
    private readonly HttpClient _httpClient;
    private readonly string _whisperApiUrl;
    private readonly string _apiKey;

    public WhisperClient(string whisperApiUrl, string apiKey)
    {
        _httpClient = new HttpClient();
        _whisperApiUrl = whisperApiUrl;
        _apiKey = apiKey;
    }

    public async Task<string> ConvertPcmToTextAsync(short[] pcmData)
    {
        byte[] byteData = ConvertPcmToByteArray(pcmData);

        using var content = new ByteArrayContent(byteData);
        content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
        content.Headers.Add("Authorization", $"Bearer {_apiKey}");

        HttpResponseMessage response = await _httpClient.PostAsync(_whisperApiUrl, content);

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<WhisperResponse>(responseString);
        // check if responseData is null
        if (responseData is not null)
        {
            return responseData.text;
        }
        else
        {
            throw new ApplicationException("An error occurred while processing the Whisper response.");
        }
    }

    private static byte[] ConvertPcmToByteArray(short[] pcmData)
    {
        byte[] byteData = new byte[pcmData.Length * sizeof(short)];
        Buffer.BlockCopy(pcmData, 0, byteData, 0, byteData.Length);
        return byteData;
    }
}

public class WhisperResponse
{
    public string text { get; set; }
}
