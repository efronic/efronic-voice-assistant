using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;

public class ChatGPTClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public ChatGPTClient(string baseAddress, string apiKey, string model = "gpt-4")
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        _model = model;
    }

    public async Task<string> GetResponseAsync(string userMessage)
    {
        try
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 100
            };

            var response = await _httpClient.PostAsJsonAsync("v1/chat/completions", requestBody);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<dynamic>(responseString);
            if (responseData is not null)
            {
                return responseData.choices[0].message.content.ToString();
            }
            else
            {
                throw new ApplicationException("An error occurred while processing the response.");
            }
        }
        catch (HttpRequestException httpEx)
        {
            // Log and handle HTTP request errors
            throw new ApplicationException("An error occurred while communicating with the OpenAI API.", httpEx);
        }
        catch (Exception ex)
        {
            // Log and handle other errors
            throw new ApplicationException("An unexpected error occurred while processing the request.", ex);
        }
    }
}
