using System;
using System.Device.Pwm;
using System.Device.Gpio;
using System.Threading.Tasks;

public class PwmController : IDisposable
{
    private readonly PwmChannel _pwmChannel;
    private bool _disposed = false;

    public PwmController(GpioController gpioController, int pinNumber, int frequency = 1000)
    {
        // Ensure the GPIO pin is configured for PWM
        gpioController.OpenPin(pinNumber, PinMode.Output);

        // Initialize PWM channel
        _pwmChannel = PwmChannel.Create(pinNumber, frequency, 0); // Start with 0% duty cycle
    }

    public async Task FadeAsync(int durationMilliseconds, bool fadeIn = true)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(PwmController));

        const double minDutyCycle = 0.0;
        const double maxDutyCycle = 1.0;
        int steps = 100; // Number of fade steps
        double stepDuration = durationMilliseconds / (double)steps;
        double stepSize = (fadeIn ? (maxDutyCycle - minDutyCycle) : (minDutyCycle - maxDutyCycle)) / steps;

        for (int i = 0; i <= steps; i++)
        {
            double dutyCycle = minDutyCycle + (i * stepSize);
            _pwmChannel.DutyCycle = fadeIn ? dutyCycle : (maxDutyCycle - dutyCycle);
            await Task.Delay((int)stepDuration);
        }

        // Ensure final value is set
        _pwmChannel.DutyCycle = fadeIn ? maxDutyCycle : minDutyCycle;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose of managed resources
                _pwmChannel.Dispose();
            }

            // Dispose of unmanaged resources (if any)

            _disposed = true;
        }
    }
}
