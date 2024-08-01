using Microsoft.Extensions.Configuration;

partial class Program
{
    private static readonly int _led1Pin = 18;
    private static readonly int _led2Pin = 24;

    private static HttpClient _httpClient = new HttpClient();
    private static SpeechSynthesizer? _speechSynthesizer;
    // private static AIClient? _aiClient;
    private static AIClient? _aiClient;
    private static SpeechToTextController? _speechToTextController;
    private static MicToText? _micToText;
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
        "What would you like me to do?",
        "How can I be of service?",
        "What do you need?",
        "How can I assist you today?",
        "What can I help you with?",
        "Ready when you are.",
        "What do you have in mind?",
        "How can I support you?",
        "What would you like to know?",
        "I'm here to help.",
        "What can I do for you today?"
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
            var openAiApiKey = Configuration["OPENAI_API_KEY"] ?? "OPENAI_API_KEY";
            var copilotApiKey = Configuration["MS_COPILOT_API_KEY_1"] ?? Configuration["MS_COPILOT_API_KEY_2"];
            var openAiGptModel = Configuration["OPENAI_GPT_MODEL"] ?? "gpt-4";
            var openAiEndpoint = Configuration["OPENAI_API_ENDPOINT"] ?? "chat/completions";
            var copilotEndpoint = Configuration["MS_COPILOT_API_ENDPOINT"] ?? "https://efronic-voice-assistant.openai.azure.com/";
            var copilotModel = Configuration["MS_COPILOT_GPT_MODEL"] ?? "gpt-4o";
            var pvAccessKey = Configuration["PV_ACCESS_KEY"] ?? "PV_ACCESS_KEY";
            var openAiBaseAddress = Configuration["OPENAI_BASE_URL"] ?? "https://api.openai.com/v1/";
            var copilotBaseAddress = Configuration["MS_COPILOT_BASE_URL"] ?? "https://efronic-voice-assistant.openai.azure.com/";
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
            var mpg123Path = Configuration["MPG123_PATH"] ?? "mpg123";
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
            var options = Configuration.GetSection("Options").Get<Options>();
            foreach (var prop in options.GetType().GetProperties())
            {
                Console.WriteLine($"{prop.Name}: {prop.GetValue(options)}");
            }
            var keywordPaths = new List<string> { @"C:\Code\efronic-voice-assistant\Hey-Wiz_en_windows_v3_0_0.ppn" };
            var sensitivities = new List<float> { 0.5f };
            int audioDeviceIndex = -1;

            _aiClient = new AIClient(openAiBaseAddress, openAiApiKey, openAiGptModel, openAiEndpoint);
            // _aiClient = new AIClient(copilotBaseAddress, copilotApiKey, copilotModel, copilotEndpoint);

            _speechSynthesizer = new SpeechSynthesizer(awsAccessKeyId, awsSecretAccessKey, Amazon.RegionEndpoint.USEast1, mpg123Path);


            _picovoiceHandler = new PicovoiceHandler(pvAccessKey, _led1Pin, _led2Pin, keywordPaths, sensitivities, audioDeviceIndex);
            _picovoiceHandler.WakeWordDetected += OnWakeWordDetected;
            _picovoiceHandler.Start();


            // _speechToTextController = new SpeechToTextController();
            _speechToTextController = new SpeechToTextController(cheetahAccessKey, cheetahModelPath, cheetahEndpointDurationSec, cheetahEnableAutomaticPunctuation, 1);

            _micToText = new MicToText(options.Encoder, options.Decoder, options.Joiner, options.Tokens);


            _whisperClient = new WhisperClient(whisperApiUrl, openAiApiKey, whisperModel);

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
            if (_micToText != null)
            {
                // transcript = await _speechToTextController.SpeechToTextAsync(CancellationToken.None);
                transcript = _speechToTextController?.SpeechToText();
                // MicToText._isRecording = true;
                // transcript = _micToText.MTT();
                Console.WriteLine($"Spoken transcript from mic.: {transcript}");

            }

            string? gptResponse = null;
            if (transcript != null && transcript != "")
            {
                gptResponse = _aiClient != null ? await _aiClient.GetResponseAsync(transcript) : throw new Exception("CopilotClient is null.");
                // gptResponse = _aiClient != null ? await _aiClient.GetResponseAsync(transcript) : throw new Exception("ChatGPTClient is null.");
                Console.WriteLine($"Response from AI: {gptResponse}");
            }
            else throw new Exception("Transcript from MicToText is null or empty.");

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
