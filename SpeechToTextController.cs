using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Pv;

public class SpeechToTextController
{
    private readonly int _silenceThreshold; // Threshold for detecting silence
    private readonly int _silenceDuration; // Duration of silence to detect before stopping
    private bool _isRecording;
    // private WaveInEvent _waveIn;
    private MemoryStream _memoryStream;
    private List<byte> _audioBuffer;
    private static string? _accessKey;
    private static string? _modelPath;
    private static int? _audioDeviceIndex;
    private static bool _enableAutomaticPunctuation;
    private static float _endpointDurationSec;
    private const int SilenceThreshold = 100;
    private System.Timers.Timer _silenceTimer;
    private PvRecorder? _recorder;

    public SpeechToTextController(string accessKey,
            string modelPath,
            float endpointDurationSec,
            bool enableAutomaticPunctuation,
            int? audioDeviceIndex)
    {
        _accessKey = accessKey;
        _modelPath = modelPath;
        _endpointDurationSec = endpointDurationSec;
        _enableAutomaticPunctuation = enableAutomaticPunctuation;
        _audioDeviceIndex = audioDeviceIndex;

        // _waveIn = new WaveInEvent();
        // _waveIn.WaveFormat = new WaveFormat(16000, 1); // 16kHz, Mono
        // _waveIn.DataAvailable += OnDataAvailable;
        // _waveIn.RecordingStopped += OnRecordingStopped;

        _memoryStream = new MemoryStream();
        _audioBuffer = new List<byte>();

        _silenceTimer = new System.Timers.Timer(10000); // 10 seconds
        _silenceTimer.Elapsed += OnSilenceTimerElapsed;
        _silenceTimer.AutoReset = false;
    }

    public string? SpeechToText()
    {
        // Start recording logic
        // _audioBuffer.Clear();
        // _waveIn.StartRecording();
        using (Cheetah cheetah = Cheetah.Create(
                accessKey: _accessKey,
                modelPath: _modelPath,
                endpointDurationSec: _endpointDurationSec,
                enableAutomaticPunctuation: _enableAutomaticPunctuation))
        {

            // create recorder
            _recorder = PvRecorder.Create(cheetah.FrameLength, _audioDeviceIndex.Value);
            Console.WriteLine($"Using device: {_recorder.SelectedDevice}");
            Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                _recorder.Stop();
                Console.WriteLine("Stopping...");
            };
            var transcript = "";

            _recorder.Start();
            _silenceTimer.Start();
            Console.WriteLine(">>> Press `CTRL+C` to exit:\n");

            try
            {
                while (_recorder.IsRecording)
                {
                    short[] frame = _recorder.Read();

                    CheetahTranscript result = cheetah.Process(frame);
                    if (!string.IsNullOrEmpty(result.Transcript))
                    {
                        Console.Write(result.Transcript);
                        transcript += result.Transcript;
                        _silenceTimer.Stop(); // Stop the silence timer since we received audio
                        _silenceTimer.Start(); // Restart the silence timer
                    }
                    if (result.IsEndpoint)
                    {
                        CheetahTranscript finalTranscriptObj = cheetah.Flush();
                        Console.WriteLine(finalTranscriptObj.Transcript);
                        transcript += finalTranscriptObj.Transcript;
                        _recorder.Stop();
                        break;
                    }
                }
            }
            catch (CheetahActivationLimitException)
            {
                Console.WriteLine($"AccessKey '{_accessKey}' has reached its processing limit.");
            }
            return transcript;
        }
    }

    private void OnSilenceTimerElapsed(object sender, ElapsedEventArgs e)
    {
        Console.WriteLine("No speech detected for 5 seconds. Stopping...");
        if (_recorder != null && _recorder.IsRecording)
        {
            _recorder.Stop();
        }
    }
}
