using System;
using System.Device.Gpio;
using System.Threading.Tasks;
using Pv;

public class PicovoiceHandler
{
    private readonly string _accessKey;
    private readonly int _led1Pin;
    private readonly int _led2Pin;
    // private readonly GpioController _gpioController;
    private Picovoice? _picovoice;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _listeningTask;
    private PwmController? _pwmController1;
    private PwmController? _pwmController2;

    public event Func<Task> WakeWordDetected = () => Task.CompletedTask;

    public PicovoiceHandler(string accessKey, int led1Pin, int led2Pin
    // , GpioController gpioController
    )
    {
        _accessKey = accessKey;
        _led1Pin = led1Pin;
        _led2Pin = led2Pin;
        // _gpioController = gpioController;
    }

    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _listeningTask = Task.Run(() => ListenForWakeWord(_cancellationTokenSource.Token));
        Console.WriteLine("PicovoiceHandler started.");
    }

    private async Task ListenForWakeWord(CancellationToken cancellationToken)
    {
        using (Porcupine porcupine = Porcupine.FromKeywordPaths(_accessKey, new string[] { "path/to/keyword.ppn" }, "path/to/model.pv", new float[] { 0.5f }))
        {
            using (PvRecorder recorder = PvRecorder.Create(frameLength: porcupine.FrameLength, deviceIndex: -1))
            {
                recorder.Start();
                Console.WriteLine("Listening for wake word...");

                while (!cancellationToken.IsCancellationRequested)
                {
                    short[] frame = recorder.Read();
                    int result = porcupine.Process(frame);
                    if (result >= 0)
                    {
                        Console.WriteLine("Wake word detected.");
                        if (WakeWordDetected != null)
                        {
                            await WakeWordDetected.Invoke();
                        }
                    }
                }

                recorder.Stop();
            }
        }
    }
    private async Task OnWakeWordDetected()
    {
        // Simulate wake word detection event
        Console.WriteLine("Wake word detected.");
        if (WakeWordDetected != null)
        {
            await WakeWordDetected.Invoke();
        }
    }

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

    public void Stop()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            if (_listeningTask != null)
            {
                _listeningTask.Wait();
            }
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null!;
            _listeningTask = null;
        }
        Console.WriteLine("PicovoiceHandler stopped.");
    }
}
