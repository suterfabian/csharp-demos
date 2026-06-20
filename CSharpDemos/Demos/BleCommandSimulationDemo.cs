using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class BleCommandSimulationDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 30;
    public string Title => "30 - BLE Command Simulation";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        await SendCommandAsync("LED_ON", cancellationToken);
        await SendCommandAsync("READ_STATUS", cancellationToken);
        await SendCommandAsync("LED_OFF", cancellationToken);

        logger.WriteLine("BLE-Kommandos simuliert.");
    }

    private async Task SendCommandAsync(string command, CancellationToken cancellationToken)
    {
        logger.WriteLine($"Sende: {command}");

        await Task.Delay(300, cancellationToken);

        logger.WriteLine($"Antwort: OK für {command}");
    }

    public string Description =>
        """
        Simuliert das sequenzielle Senden von BLE-Kommandos.

        Jedes Kommando wird gesendet, danach wird asynchron auf eine
        simulierte Antwort gewartet.

        Die Kommandos werden nacheinander ausgeführt, weil jeder Aufruf
        mit await abgeschlossen wird, bevor das nächste Kommando startet.
        """;
}