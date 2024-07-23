using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Pv;

public class PicovoiceHandler
{
    private readonly string _accessKey;
    private readonly int _led1Pin;
    private readonly int _led2Pin;
    // private readonly GpioController _gpioController;
    private Picovoice? _picovoice;
    private readonly List<string> _keywordPaths;
    private readonly List<float> _sensitivities;
    private readonly int _audioDeviceIndex;
    private bool _isRunning;

    public event Func<Task> WakeWordDetected;

    public PicovoiceHandler(string accessKey, int led1Pin, int led2Pin, List<string> keywordPaths, List<float> sensitivities, int audioDeviceIndex)
    {
        _accessKey = accessKey;
        _led1Pin = led1Pin;
        _led2Pin = led2Pin;
        _keywordPaths = keywordPaths;
        _sensitivities = sensitivities;
        _audioDeviceIndex = audioDeviceIndex;
    }

    public void Start()
    {
        _isRunning = true;
        Task.Run(() => ListenForWakeWord());
    }
    public void Stop()
    {
        _isRunning = false;
    }
    private async Task ListenForWakeWord()
    {
        try
        {
            using (Porcupine porcupine = Porcupine.FromKeywordPaths(_accessKey, _keywordPaths, sensitivities: _sensitivities))
            {
                try
                {
                    using (PvRecorder recorder = PvRecorder.Create(frameLength: porcupine.FrameLength, deviceIndex: _audioDeviceIndex))
                    {
                        recorder.Start();
                        while (_isRunning)
                        {
                            short[] frame = recorder.Read();
                            int result = porcupine.Process(frame);
                            if (result >= 0)
                            {
                                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Detected '{Path.GetFileNameWithoutExtension(_keywordPaths[result])}'");
                                if (WakeWordDetected != null)
                                {
                                    await WakeWordDetected.Invoke();
                                }
                            }
                            Thread.Yield();
                        }
                        recorder.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing or using PvRecorder: {ex.Message}");
                }
            }
        }
        catch (PorcupineIOException pioEx)
        {
            Console.WriteLine($"Porcupine IO Exception: {pioEx.Message}. Please check your keyword paths and access key.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
    // private async Task OnWakeWordDetected()
    // {
    //     // Simulate wake word detection event
    //     Console.WriteLine("Wake word detected.");
    //     if (WakeWordDetected != null)
    //     {
    //         await WakeWordDetected.Invoke();
    //     }
    // }

    // public async Task FadeLedsAsync()
    // {
    //     if (_pwmController1 == null || _pwmController2 == null)
    //     {
    //         _pwmController1 = new PwmController(_gpioController, _led1Pin);
    //         _pwmController2 = new PwmController(_gpioController, _led2Pin);
    //     }

    //     // Perform fade out
    //     await _pwmController1.FadeAsync(2000); // Fade duration in milliseconds
    //     await _pwmController2.FadeAsync(2000); // Fade duration in milliseconds
    // }

    // public void Stop()
    // {
    //     if (_cancellationTokenSource != null)
    //     {
    //         _cancellationTokenSource.Cancel();
    //         if (_listeningTask != null)
    //         {
    //             _listeningTask.Wait();
    //         }
    //         _cancellationTokenSource.Dispose();
    //         _cancellationTokenSource = null!;
    //         _listeningTask = null;
    //     }
    //     Console.WriteLine("PicovoiceHandler stopped.");
    // }
}
