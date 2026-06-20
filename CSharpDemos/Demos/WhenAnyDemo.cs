using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class WhenAnyDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 14;
    public string Title => "14 - Task.WhenAny";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var fast = LoadAsync("Schnell", 500, cancellationToken);
        var slow = LoadAsync("Langsam", 2000, cancellationToken);

        var finished = await Task.WhenAny(fast, slow);

        logger.WriteLine(await finished);
        logger.WriteLine("WhenAny liefert den zuerst fertigen Task.");
    }

    private static async Task<string> LoadAsync(string name, int delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken);
        return $"{name} ist fertig.";
    }
    
    public string Description =>
        """
        Demonstriert das Warten auf den zuerst abgeschlossenen Task.

        Mehrere asynchrone Operationen laufen parallel.
        Task.WhenAny liefert zurück, sobald die erste Operation fertig ist.

        Die übrigen Tasks laufen dabei weiter, sofern sie nicht separat
        abgebrochen werden.
        """;
}