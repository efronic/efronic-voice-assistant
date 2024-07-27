using System;
using System.Collections.Generic;
using System.IO;
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
            using (PvRecorder recorder = PvRecorder.Create(cheetah.FrameLength, _audioDeviceIndex.Value))
            {
                Console.WriteLine($"Using device: {recorder.SelectedDevice}");
                Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
                {
                    e.Cancel = true;
                    recorder.Stop();
                    Console.WriteLine("Stopping...");
                };
                var transcript = "";

                recorder.Start();
                Console.WriteLine(">>> Press `CTRL+C` to exit:\n");

                try
                {
                    while (recorder.IsRecording)
                    {
                        short[] frame = recorder.Read();

                        CheetahTranscript result = cheetah.Process(frame);
                        if (!string.IsNullOrEmpty(result.Transcript))
                        {
                            Console.Write(result.Transcript);
                            transcript += result.Transcript;
                        }
                        if (result.IsEndpoint)
                        {
                            CheetahTranscript finalTranscriptObj = cheetah.Flush();
                            Console.WriteLine(finalTranscriptObj.Transcript);
                            transcript += finalTranscriptObj.Transcript;
                            recorder.Stop();
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
    }

    // public void Stop()
    // {
    //     // Stop recording logic
    //     _waveIn.StopRecording();
    // }
    // private void OnDataAvailable(object sender, WaveInEventArgs e)
    // {
    //     _audioBuffer.AddRange(e.Buffer);
    // }

    // private void OnRecordingStopped(object sender, StoppedEventArgs e)
    // {
    //     _memoryStream.Write(_audioBuffer.ToArray(), 0, _audioBuffer.Count);
    // }


    // public bool IsSilent()
    // {
    //     short[] pcmData = GetPcmData();
    //     foreach (var sample in pcmData)
    //     {
    //         if (Math.Abs(sample) > SilenceThreshold)
    //         {
    //             return false;
    //         }
    //     }
    //     return true;
    // }
    // public async Task StopIfSilenceDetected()
    // {
    //     while (!IsSilent())
    //     {
    //         await Task.Delay(100); // Check every 100ms for silence
    //     }
    //     Stop();
    // }
    // public short[] GetPcmData()
    // {
    //     byte[] byteData = _audioBuffer.ToArray();
    //     short[] pcmData = new short[byteData.Length / sizeof(short)];
    //     Buffer.BlockCopy(byteData, 0, pcmData, 0, byteData.Length);
    //     return pcmData;
    // }
}
