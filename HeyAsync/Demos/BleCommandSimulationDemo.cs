using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class BleCommandSimulationDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 30;
    public string Title => "30 - BLE Command Simulation";

    public BleCommandSimulationDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        await SendCommandAsync("LED_ON", cancellationToken);
        await SendCommandAsync("READ_STATUS", cancellationToken);
        await SendCommandAsync("LED_OFF", cancellationToken);

        _logger.WriteLine("BLE-Kommandos simuliert.");
    }

    private async Task SendCommandAsync(string command, CancellationToken cancellationToken)
    {
        _logger.WriteLine($"Sende: {command}");

        await Task.Delay(300, cancellationToken);

        _logger.WriteLine($"Antwort: OK für {command}");
    }
}