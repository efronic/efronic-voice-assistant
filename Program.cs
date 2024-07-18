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
    // private static GpioController? _gpioController;
    private static HttpClient _httpClient = new HttpClient();
    private static SpeechSynthesizer? _speechSynthesizer;
    private static ChatGPTClient? _chatGPTClient;
    private static SpeechToTextController? _speechToTextController;
    // private static AudioPlayer _audioPlayer;
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
            // Initialize GPIO
            // _gpioController = new GpioController();
            // _gpioController.OpenPin(_led1Pin, PinMode.Output);
            // _gpioController.OpenPin(_led2Pin, PinMode.Output);
            // _gpioController.Write(_led1Pin, PinValue.Low);
            // _gpioController.Write(_led2Pin, PinValue.Low);

            // Load configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Base settings
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true) // Environment-specific settings
            .AddEnvironmentVariables(); // Environment variables


            Configuration = builder.Build();

            // var host = Host.CreateDefaultBuilder(args)
            //             .ConfigureServices((context, services) =>
            //             {
            //                 services.AddSingleton<IConfiguration>(Configuration);
            //                 services.AddSingleton<ConfigurationReloader>();
            //             })
            //             .ConfigureLogging(logging =>
            //             {
            //                 logging.AddConsole();
            //             })
            //             .Build();

            // var configReloader = host.Services.GetRequiredService<ConfigurationReloader>();
            // configReloader.StartListening();

            // await host.RunAsync();


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
                cheetahAudioDeviceIndex = -1; // Default value if parsing fails
            }
            var cheetahAccessKey = Configuration["CHEETAH_ACCESS_KEY"] ?? "";
            var cheetahModelPath = Configuration["CHEETAH_MODEL_PATH"] ?? "";
            float cheetahEndpointDurationSec;
            if (!float.TryParse(Configuration["CHEETAH_ENDPOINT_DURATION_SEC"], out cheetahEndpointDurationSec))
            {
                cheetahEndpointDurationSec = 3.0f; // Default value if parsing fails
            }
            bool cheetahEnableAutomaticPunctuation;
            if (!bool.TryParse(Configuration["CHEETAH_ENABLE_AUTOMATIC_PUNCTUATION"], out cheetahEnableAutomaticPunctuation))
            {
                cheetahEnableAutomaticPunctuation = true; // Default value if parsing fails
            }
            var keywordPaths = new List<string> { "./Hey-Wiz_en_windows_v3_0_0.ppn" }; // Add your keyword paths
            var sensitivities = new List<float> { 0.5f }; // Adjust sensitivities as needed
            int audioDeviceIndex = -1; // Default audio device index
            // Initialize ChatGPTClient
            _chatGPTClient = new ChatGPTClient(baseAddress, openaiApiKey, gptModel);

            // Initialize SpeechSynthesizer
            _speechSynthesizer = new SpeechSynthesizer(awsAccessKeyId, awsSecretAccessKey, Amazon.RegionEndpoint.USEast1);

            // Initialize PicovoiceHandler
            _picovoiceHandler = new PicovoiceHandler(pvAccessKey, _led1Pin, _led2Pin, keywordPaths, sensitivities, audioDeviceIndex);
            _picovoiceHandler.WakeWordDetected += OnWakeWordDetected;
            _picovoiceHandler.Start();

            // Initialize Recorder
            _speechToTextController = new SpeechToTextController(cheetahAccessKey, cheetahModelPath, cheetahEndpointDurationSec, cheetahEnableAutomaticPunctuation, 1);
            // _audioPlayer = new AudioPlayer();
            _whisperClient = new WhisperClient(whisperApiUrl, openaiApiKey, whisperModel);
            // Main loop to keep the application running
            Console.WriteLine("Application started. Press Ctrl+C to exit.");
            while (true)
            {
                await Task.Delay(1000); // Keeping the main thread alive
            }
        }
        catch (Exception ex)
        {
            // Log the exception and exit gracefully
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Optionally, log to a file or logging service
        }
    }

    private static async Task OnWakeWordDetected()
    {
        Console.WriteLine("Wake word detected, listening for speech...");
        try
        {
            // Simulate waiting for the speech capture
            await Task.Delay(5000); // Replace with actual speech capture logic

            // Use a random prompt from the array to interact with the user
            var random = new Random();
            string prompt = _prompts[random.Next(_prompts.Length)];
            Console.WriteLine($"Prompt: {prompt}");

            // Synthesize the prompt to speech
            if (_speechSynthesizer != null)
                await _speechSynthesizer.SynthesizeSpeechAsync(prompt);

            // Converting speech to text
            string? transcript = _speechToTextController?.SpeechToText();

            // Simulate listening for voice commands
            // await Task.Delay(5000); // Replace with actual listening logic

            // Wait for silence to detect end of speech
            // if (_speechToText != null)
            //     await _speechToText.StopIfSilenceDetected();
            // _recorder.Stop();

            // Process the recording
            // short[] pcmData = _speechToTextController?.GetPcmData() ?? throw new Exception("Recorder is null.");
            // string transcript = _whisperClient != null ? await _whisperClient.ConvertPcmToTextAsync(pcmData) : throw new Exception("Whisper client is null."); // This should be replaced with actual speech-to-text result



            // Get a response from ChatGPT
            string? gptResponse = null;
            if (transcript != null)
                gptResponse = _chatGPTClient != null ? await _chatGPTClient.GetResponseAsync(transcript) : throw new Exception("ChatGPTClient is null.");
            else throw new Exception("Transcript is null.");

            // Play the response
            // _audioPlayer.PlayMp3("speech.mp3");
            if (_speechSynthesizer != null && gptResponse != null)
                await _speechSynthesizer.SynthesizeSpeechAsync(gptResponse);
            else
                throw new Exception("SpeechSynthesizer or GPT response is null.");

            // Fade LEDs
            // await FadeLedsAsync(fadeIn: false);
            Console.WriteLine($"Response has been spoken: {gptResponse}");

            // Synthesize the response to speech
            if (_speechSynthesizer != null && gptResponse != null)
                await _speechSynthesizer.SynthesizeSpeechAsync(gptResponse);
            else
                throw new Exception("SpeechSynthesizer or GPT response is null.");

            Console.WriteLine("Response has been spoken.");

            // Reset GPIO
            // if (_gpioController != null)
            // {
            //     _gpioController.Write(_led1Pin, PinValue.Low);
            //     _gpioController.Write(_led2Pin, PinValue.Low);
            // }
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"HTTP error occurred: {httpEx.Message}");
            // Handle HTTP request exceptions
        }
        catch (TaskCanceledException taskEx)
        {
            Console.WriteLine($"Task was canceled: {taskEx.Message}");
            // Handle task cancellation exceptions
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Handle other exceptions
        }
    }

    // private static async Task FadeLedsAsync(bool fadeIn = true)
    // {
    //     try
    //     {
    //         using var pwmController = _gpioController != null ? new PwmController(_gpioController, _led1Pin) : throw new Exception("_gpioController is null.");
    //         await pwmController.FadeAsync(2000, fadeIn); // Fade duration in milliseconds
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"An error occurred while fading LEDs: {ex.Message}");
    //         // Handle exceptions related to PWM control
    //     }
    // }
}
