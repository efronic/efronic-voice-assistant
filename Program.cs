using System;
using System.Device.Gpio;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

partial class Program
{
    private static readonly int _led1Pin = 18;
    private static readonly int _led2Pin = 24;
    private static GpioController _gpioController;
    private static HttpClient _httpClient = new HttpClient();
    private static SpeechSynthesizer _speechSynthesizer;
    private static ChatGPTClient _chatGPTClient;
    static async Task Main(string[] args)
    {
        // InitializeGpio();
        // await InitializeOpenAi();
        _gpioController = new GpioController();
        _gpioController.OpenPin(_led1Pin, PinMode.Output);
        _gpioController.OpenPin(_led2Pin, PinMode.Output);
        _gpioController.Write(_led1Pin, PinValue.Low);
        _gpioController.Write(_led2Pin, PinValue.Low);

        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        IConfiguration configuration = builder.Build();

        var openai_api_key = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "OPENAI_API_KEY";
        var GPT_model = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "gpt-4";

        _chatGPTClient = new ChatGPTClient(openai_api_key, GPT_model);
        _speechSynthesizer = new SpeechSynthesizer("your-aws-access-key-id", "your-aws-secret-access-key", Amazon.RegionEndpoint.USEast1);
        var prompt = new[] {
            "How may I assist you?",
            "How may I help?",
            "What can I do for you?",
            "Ask me anything.",
            "Yes?",
            "I'm here.",
            "I'm listening.",
            "What would you like me to do?"
        };

        // Main loop
        while (true)
        {
            // Call wake word detection, listen, and other methods
            WakeWord().Wait();
        }

    }

    // static void InitializeGpio()
    // {
    //     _gpioController = new GpioController();
    //     _gpioController.OpenPin(_led1Pin, PinMode.Output);
    //     _gpioController.Write(_led1Pin, PinValue.Low);
    //     _gpioController.OpenPin(_led2Pin, PinMode.Output);
    //     _gpioController.Write(_led2Pin, PinValue.Low);

    // }
    static async Task WakeWord()
    {
        Console.WriteLine("Waiting for wake word...");
        // Implement wake word detection using a suitable library or custom code
        await Task.Delay(1000); // Simulating wake word detection

        _gpioController.Write(_led1Pin, PinValue.High);
        _gpioController.Write(_led2Pin, PinValue.High);

        Console.WriteLine("Wake word detected");
        string response = await _chatGPTClient.GetResponseAsync("Hello, how can I help you?");
        Console.WriteLine(response);
        await _speechSynthesizer.SynthesizeSpeechAsync(response);
    }

    // static async Task InitializeOpenAi()
    // {
    //     var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    //     _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
    //     // Further implementation for OpenAI API communication
    // }
    // static async Task<string> ChatGPT(string query)
    // {
       
    //     string apiUrl = "https://api.openai.com/v1/engines/" + GPT_model + "/completions";
    //     var requestContent = new
    //     {
    //         prompt = query,
    //         max_tokens = 150
    //     };
    //     var content = new StringContent(JsonSerializer.Serialize(requestContent), System.Text.Encoding.UTF8, "application/json");
    //     _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + openai_api_key);

    //     HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
    //     response.EnsureSuccessStatusCode(); // Throw if not a success code.

    //     string responseString = await response.Content.ReadAsStringAsync();
    //     var jsonResponse = JsonDocument.Parse(responseString);
    //     if (jsonResponse.RootElement.TryGetProperty("choices", out JsonElement choicesElement) &&
    //                 choicesElement.GetArrayLength() > 0 &&
    //                 choicesElement[0].TryGetProperty("text", out JsonElement textElement))
    //     {
    //         return textElement.GetString() ?? "No response from ChatGPT.";
    //     }

    //     return "No response from ChatGPT.";
    // }
}