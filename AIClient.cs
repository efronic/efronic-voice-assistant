using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Diagnostics;
using System.Threading;

public class AIClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly int _maxRetries;
    private readonly string _apiEndpoint;

    public AIClient(string baseAddress, string apiKey, string model, string apiEndpoint, int maxRetries = 5)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
        _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        _model = model;
        _maxRetries = maxRetries;
        _apiEndpoint = apiEndpoint;
    }

    public async Task<string> GetResponseAsync(string userMessage)
    {
        int retryCount = 0;
        int initialDelay = 1000; // initial delay in milliseconds
        int maxDelay = 32000; // maximum delay in milliseconds
        Random random = new Random();

        while (retryCount < _maxRetries)
        {
            try
            {
                Console.WriteLine($"Sending message to: {_apiEndpoint}");
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

                var response = await _httpClient.PostAsJsonAsync(_apiEndpoint, requestBody);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    Debug.WriteLine("Rate limit exceeded. No rate limit headers available.");

                    // Calculate exponential backoff delay
                    int delay = Math.Min(initialDelay * (int)Math.Pow(2, retryCount), maxDelay);
                    delay += random.Next(100, 1000); // Add some randomness to delay

                    retryCount++;
                    Debug.WriteLine($"Retry {retryCount}: Waiting for {delay} milliseconds due to rate limiting.");
                    await Task.Delay(delay);
                    continue;
                }

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<AIResponse>(responseString);
                if (responseData != null && responseData.choices != null && responseData.choices.Length > 0)
                {
                    Console.WriteLine($"responseData from azure openai: {responseData}");
                    return responseData.choices[0].message.content;
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

        throw new ApplicationException("Exceeded the maximum number of retries.");
    }
}
public class AIResponse
{
    public Choice[] choices { get; set; }
}

public class Choice
{
    public Message message { get; set; }
}

public class Message
{
    public string content { get; set; }
}