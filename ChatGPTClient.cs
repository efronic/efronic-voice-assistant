using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

public class ChatGPTClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public ChatGPTClient(string apiKey, string model = "gpt-4")
    {
        _httpClient = new HttpClient();
        _apiKey = apiKey;
        _model = model;
    }

    public async Task<string> GetResponseAsync(string query)
    {
        string apiUrl = $"https://api.openai.com/v1/engines/{_model}/completions";
        var requestContent = new
        {
            prompt = query,
            max_tokens = 150
        };
        var content = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey);

        HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode(); // Throw if not a success code.

        string responseString = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseString);

        if (jsonResponse.RootElement.TryGetProperty("choices", out JsonElement choicesElement) &&
            choicesElement.GetArrayLength() > 0 &&
            choicesElement[0].TryGetProperty("text", out JsonElement textElement))
        {
            return textElement.GetString() ?? "No response from ChatGPT.";
        }

        return "No response from ChatGPT.";
    }
}
