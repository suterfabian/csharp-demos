using HeyAsync.Models;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class WhenAllDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 13;
    public string Title => "13 - Task.WhenAll";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var first = LoadAsync("A", 1000, cancellationToken);
        var second = LoadAsync("B", 1500, cancellationToken);
        var third = LoadAsync("C", 500, cancellationToken);

        var results = await Task.WhenAll(first, second, third);

        foreach (var result in results)
        {
            logger.WriteLine(result);
        }
    }

    private static async Task<string> LoadAsync(string name, int delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken);
        return $"Operation {name} fertig.";
    }
    
    public string Description =>
        """
        Demonstriert das parallele Warten auf mehrere Tasks.

        Mehrere asynchrone Operationen werden gleichzeitig gestartet.
        Task.WhenAll wartet, bis alle Operationen abgeschlossen sind.

        Danach stehen alle Ergebnisse gemeinsam zur Verfügung.
        """;
}