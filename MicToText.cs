using PortAudioSharp;
using System.Threading;
using SherpaOnnx;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class MicToText
{
    // Configuration variables
    private static string _tokensPath;
    private static string _encoderPath;
    private static string _decoderPath;
    private static string? _joinerPath;
    private static string _provider;
    private static int _numThreads;
    private static string _decodingMethod;
    private static bool _debug;
    private static int _sampleRate;
    private static int _maxActivePaths;
    private static bool _enableEndpoint;
    private static float _rule1MinTrailingSilence;
    private static float _rule2MinTrailingSilence;
    private static float _rule3MinUtteranceLength;
    private static int? _audioDeviceIndex;
    public static bool _isRecording = false;

    public MicToText(
                     string encoderPath,
                     string decoderPath,
                     string? joinerPath, // Made optional
                     string tokensPath,
                     string provider = "cpu", // Default value
                     int numThreads = 1, // Default value
                     string decodingMethod = "greedy_search", // Default value
                     bool debug = false, // Default value
                     int sampleRate = 16000, // Default value
                     int maxActivePaths = 4, // Default value
                     bool enableEndpoint = true, // Default value
                     float rule1MinTrailingSilence = 2.4F, // Default value
                     float rule2MinTrailingSilence = 0.8F, // Default value
                     float rule3MinUtteranceLength = 20.0F, // Default value
                     int? audioDeviceIndex = null)
    {
        _tokensPath = tokensPath;
        _encoderPath = encoderPath;
        _decoderPath = decoderPath;
        _joinerPath = joinerPath;
        _provider = provider;
        _numThreads = numThreads;
        _decodingMethod = decodingMethod;
        _debug = debug;
        _sampleRate = sampleRate;
        _maxActivePaths = maxActivePaths;
        _enableEndpoint = enableEndpoint;
        _rule1MinTrailingSilence = rule1MinTrailingSilence;
        _rule2MinTrailingSilence = rule2MinTrailingSilence;
        _rule3MinUtteranceLength = rule3MinUtteranceLength;
        _audioDeviceIndex = audioDeviceIndex;
    }

    public string? MTT()
    {
        _isRecording = true;
        OnlineRecognizerConfig config = new OnlineRecognizerConfig
        {
            FeatConfig = { SampleRate = _sampleRate, FeatureDim = 80 },
            ModelConfig =
            {
                Transducer = { Encoder = _encoderPath, Decoder = _decoderPath, Joiner = _joinerPath },
                Tokens = _tokensPath,
                Provider = _provider,
                NumThreads = _numThreads,
                Debug = _debug ? 1 : 0
            },
            DecodingMethod = _decodingMethod,
            MaxActivePaths = _maxActivePaths,
            EnableEndpoint = _enableEndpoint ? 1 : 0,
            Rule1MinTrailingSilence = _rule1MinTrailingSilence,
            Rule2MinTrailingSilence = _rule2MinTrailingSilence,
            Rule3MinUtteranceLength = _rule3MinUtteranceLength
        };
        Console.WriteLine($"Configuration loaded _audioDeviceIndex '{_audioDeviceIndex}'");
        Console.WriteLine($"Configuration loaded _sampleRate '{_sampleRate}'");
        Console.WriteLine($"Configuration loaded _tokensPath '{_tokensPath}'");
        Console.WriteLine($"Configuration loaded _encoderPath '{_encoderPath}'");
        Console.WriteLine($"Configuration loaded _decoderPath '{_decoderPath}'");
        Console.WriteLine($"Configuration loaded _joinerPath '{_joinerPath}'");
        Console.WriteLine($"Configuration loaded _provider '{_provider}'");
        Console.WriteLine($"Configuration loaded _numThreads '{_numThreads}'");
        Console.WriteLine($"Configuration loaded _decodingMethod '{_decodingMethod}'");
        Console.WriteLine($"Configuration loaded _debug '{_debug}'");
        Console.WriteLine($"Configuration loaded _maxActivePaths '{_maxActivePaths}'");
        Console.WriteLine($"Configuration loaded _enableEndpoint '{_enableEndpoint}'");
        Console.WriteLine($"Configuration loaded _rule1MinTrailingSilence '{_rule1MinTrailingSilence}'");
        Console.WriteLine($"Configuration loaded _rule2MinTrailingSilence '{_rule2MinTrailingSilence}'");
        Console.WriteLine($"Configuration loaded _rule3MinUtteranceLength '{_rule3MinUtteranceLength}'");
        Console.WriteLine($"Config loaded '{config}'");



        OnlineRecognizer recognizer = new OnlineRecognizer(config);
        OnlineStream createStreamObj = recognizer.CreateStream();

        Console.WriteLine(PortAudio.VersionInfo.versionText);
        PortAudio.Initialize();

        int deviceIndex = _audioDeviceIndex ?? PortAudio.DefaultInputDevice;
        if (deviceIndex == PortAudio.NoDevice)
        {
            Console.WriteLine("No default input device found");
            Environment.Exit(1);
        }

        DeviceInfo info = PortAudio.GetDeviceInfo(deviceIndex);
        Console.WriteLine($"Use default device {deviceIndex} ({info.name})");

        StreamParameters param = new StreamParameters
        {
            device = deviceIndex,
            channelCount = 1,
            sampleFormat = SampleFormat.Float32,
            suggestedLatency = info.defaultLowInputLatency,
            hostApiSpecificStreamInfo = IntPtr.Zero
        };

        long lastAudioTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        PortAudioSharp.Stream.Callback callback = (IntPtr input, IntPtr output, UInt32 frameCount, ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData) =>
        {
            float[] samples = new float[frameCount];
            Marshal.Copy(input, samples, 0, (Int32)frameCount);
            createStreamObj.AcceptWaveform(_sampleRate, samples);
            if (!IsSilent(samples))
            {
                lastAudioTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            return StreamCallbackResult.Continue;
        };

        PortAudioSharp.Stream stream = new PortAudioSharp.Stream(param, null, _sampleRate, 0, StreamFlags.ClipOff, callback, IntPtr.Zero);

        Console.WriteLine("Started! Please speak");

        stream.Start();

        string transcript = "";
        string lastText = "";
        int segmentIndex = 0;

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            _isRecording = false;
        };

        while (_isRecording)
        {
            while (recognizer.IsReady(createStreamObj))
            {
                recognizer.Decode(createStreamObj);
            }

            var text = recognizer.GetResult(createStreamObj).Text;
            bool isEndpoint = recognizer.IsEndpoint(createStreamObj);

            if (!string.IsNullOrWhiteSpace(text) && lastText != text)
            {
                lastText = text;
                Console.Write($"\r{segmentIndex}: {lastText}");
                // transcript += lastText;
            }

            if (isEndpoint)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    transcript += lastText;
                    ++segmentIndex;
                    Console.WriteLine();
                }
                recognizer.Reset(createStreamObj);
            }
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (currentTimestamp - lastAudioTimestamp > 5000)
            {
                Console.WriteLine("\nDetected 5 seconds of silence. Stopping recording.");
                _isRecording = false;
            }
            Thread.Sleep(200); // ms
        }

        stream.Stop();
        PortAudio.Terminate();
        // Reset _isRecording for the next iteration
        _isRecording = false;

        return transcript;
    }
    private static bool IsSilent(float[] samples, float threshold = 0.01f)
    {
        foreach (var sample in samples)
        {
            if (Math.Abs(sample) > threshold)
            {
                return false;
            }
        }
        return true;
    }
}
