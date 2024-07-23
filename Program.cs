using System;
using System.Device.Gpio;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

partial class Program
{
    private static readonly int _led1Pin = 18;
    private static readonly int _led2Pin = 24;

    private static HttpClient _httpClient = new HttpClient();
    private static SpeechSynthesizer? _speechSynthesizer;
    private static ChatGPTClient? _chatGPTClient;
    private static SpeechToTextController? _speechToTextController;

    private static PicovoiceHandler? _picovoiceHandler;
    private static WhisperClient? _whisperClient;
    public static IConfiguration? Configuration { get; private set; }
    private static readonly string[] _prompts = {
        "How may I assist you?",
        "How may I help?",
        "What can I do for you?",
        "Ask me anything.",
        "Yes?",
        "I'm here.",
        "I'm listening.",
        "What would you like me to do?"
    };

    static async Task Main(string[] args)
    {
        try
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
                Configuration = builder.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load configuration: {ex.Message}");
            }
            Console.WriteLine($"efronic ASPNETCORE_ENVIRONMENT: {environment}");
            Console.WriteLine("Configuration Loaded:");
            foreach (var pair in Configuration.AsEnumerable())
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
            var openaiApiKey = Configuration["OPENAI_API_KEY"] ?? "OPENAI_API_KEY";
            var gptModel = Configuration["GPT_MODEL"] ?? "gpt-3.5-turbo-16k";
            var pvAccessKey = Configuration["PV_ACCESS_KEY"] ?? "PV_ACCESS_KEY";
            var baseAddress = Configuration["BASE_URL"] ?? "https://api.openai.com/v1/";
            var whisperApiUrl = Configuration["WHISPER_API_URL"] ?? "https://api.whisper.ai/v1/convert";
            var whisperModel = Configuration["WHISPER_MODEL"] ?? "whisper-1";
            var awsAccessKeyId = Configuration["AWS_ACCESS_KEY_ID"] ?? "AWS_ACCESS_KEY_ID";
            var awsSecretAccessKey = Configuration["AWS_SECRET_ACCESS_KEY"] ?? "AWS_SECRET_ACCESS_KEY";
            int cheetahAudioDeviceIndex;
            if (!int.TryParse(Configuration["CHEETAH_AUDIO_DEVICE_INDEX"], out cheetahAudioDeviceIndex))
            {
                cheetahAudioDeviceIndex = -1;
            }
            var cheetahAccessKey = Configuration["CHEETAH_ACCESS_KEY"] ?? "";
            var cheetahModelPath = Configuration["CHEETAH_MODEL_PATH"] ?? null;
            float cheetahEndpointDurationSec;
            if (!float.TryParse(Configuration["CHEETAH_ENDPOINT_DURATION_SEC"], out cheetahEndpointDurationSec))
            {
                cheetahEndpointDurationSec = 3.0f;
            }
            bool cheetahEnableAutomaticPunctuation;
            if (!bool.TryParse(Configuration["CHEETAH_ENABLE_AUTOMATIC_PUNCTUATION"], out cheetahEnableAutomaticPunctuation))
            {
                cheetahEnableAutomaticPunctuation = true;
            }
            var keywordPaths = new List<string> { @"C:\Code\efronic-voice-assistant\Hey-Wiz_en_windows_v3_0_0.ppn" };
            var sensitivities = new List<float> { 0.5f };
            int audioDeviceIndex = -1;

            _chatGPTClient = new ChatGPTClient(baseAddress, openaiApiKey, gptModel);


            _speechSynthesizer = new SpeechSynthesizer(awsAccessKeyId, awsSecretAccessKey, Amazon.RegionEndpoint.USEast1);


            _picovoiceHandler = new PicovoiceHandler(pvAccessKey, _led1Pin, _led2Pin, keywordPaths, sensitivities, audioDeviceIndex);
            _picovoiceHandler.WakeWordDetected += OnWakeWordDetected;
            _picovoiceHandler.Start();


            // _speechToTextController = new SpeechToTextController();f
            _speechToTextController = new SpeechToTextController(cheetahAccessKey, cheetahModelPath, cheetahEndpointDurationSec, cheetahEnableAutomaticPunctuation, 1);


            _whisperClient = new WhisperClient(whisperApiUrl, openaiApiKey, whisperModel);

            Console.WriteLine("Application started. Press Ctrl+C to exit.");
            while (true)
            {
                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"An error occurred: {ex.Message}");

        }
    }

    private static async Task OnWakeWordDetected()
    {
        Console.WriteLine("Wake word detected, listening for speech...");
        try
        {

            await Task.Delay(5000);


            var random = new Random();
            string prompt = _prompts[random.Next(_prompts.Length)];
            Console.WriteLine($"Prompt: {prompt}");


            if (_speechSynthesizer != null)
                await _speechSynthesizer.SynthesizeSpeechAsync(prompt);


            string? transcript = null;
            if (_speechToTextController != null)
            {
                // transcript = await _speechToTextController.SpeechToTextAsync(CancellationToken.None);
                transcript = _speechToTextController?.SpeechToText();

            }

            string? gptResponse = null;
            if (transcript != null)
                gptResponse = _chatGPTClient != null ? await _chatGPTClient.GetResponseAsync(transcript) : throw new Exception("ChatGPTClient is null.");
            else throw new Exception("Transcript is null.");



            if (_speechSynthesizer != null && gptResponse != null)
                await _speechSynthesizer.SynthesizeSpeechAsync(gptResponse);
            else
                throw new Exception("SpeechSynthesizer or GPT response is null.");



            Console.WriteLine($"Response has been spoken: {gptResponse}");


            if (_speechSynthesizer != null && gptResponse != null)
                await _speechSynthesizer.SynthesizeSpeechAsync(gptResponse);
            else
                throw new Exception("SpeechSynthesizer or GPT response is null.");

            Console.WriteLine("Response has been spoken.");

        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"HTTP error occurred: {httpEx.Message}");

        }
        catch (TaskCanceledException taskEx)
        {
            Console.WriteLine($"Task was canceled: {taskEx.Message}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");

        }
    }














}
