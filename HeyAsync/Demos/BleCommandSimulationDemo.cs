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

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        await SendCommandAsync("LED_ON");
        await SendCommandAsync("READ_STATUS");
        await SendCommandAsync("LED_OFF");

        _logger.WriteLine("BLE-Kommandos simuliert.");
    }

    private async Task SendCommandAsync(string command)
    {
        _logger.WriteLine($"Sende: {command}");

        await Task.Delay(300);

        _logger.WriteLine($"Antwort: OK für {command}");
    }
}