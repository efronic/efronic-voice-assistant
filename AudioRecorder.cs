using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

public class AudioRecorder
{
    private readonly int _silenceThreshold; // Threshold for detecting silence
    private readonly int _silenceDuration; // Duration of silence to detect before stopping
    private bool _isRecording;
    private WaveInEvent _waveIn;
    private MemoryStream _memoryStream;
    private List<byte> _audioBuffer;
    private const int SilenceThreshold = 100;
    public AudioRecorder()
    {
        _waveIn = new WaveInEvent();
        _waveIn.WaveFormat = new WaveFormat(16000, 1); // 16kHz, Mono
        _waveIn.DataAvailable += OnDataAvailable;
        _waveIn.RecordingStopped += OnRecordingStopped;

        _memoryStream = new MemoryStream();
        _audioBuffer = new List<byte>();

    }

    public void Start()
    {
        // Start recording logic
        _audioBuffer.Clear();
        _waveIn.StartRecording();
    }

    public void Stop()
    {
        // Stop recording logic
        _waveIn.StopRecording();
    }
    private void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        _audioBuffer.AddRange(e.Buffer);
    }

    private void OnRecordingStopped(object sender, StoppedEventArgs e)
    {
        _memoryStream.Write(_audioBuffer.ToArray(), 0, _audioBuffer.Count);
    }


    public bool IsSilent()
    {
        short[] pcmData = GetPcmData();
        foreach (var sample in pcmData)
        {
            if (Math.Abs(sample) > SilenceThreshold)
            {
                return false;
            }
        }
        return true;
    }
    public async Task MonitorSilence()
    {
        while (!IsSilent())
        {
            await Task.Delay(100); // Check every 100ms for silence
        }
    }
    public short[] GetPcmData()
    {
        byte[] byteData = _audioBuffer.ToArray();
        short[] pcmData = new short[byteData.Length / sizeof(short)];
        Buffer.BlockCopy(byteData, 0, pcmData, 0, byteData.Length);
        return pcmData;
    }
}
