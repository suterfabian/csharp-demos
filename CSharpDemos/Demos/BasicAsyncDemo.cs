using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class BasicAsyncDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 1;
    public string Title => "01 - Basic async/await";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        logger.WriteLine("Start");
        logger.WriteLine($"Vor await - Thread ID: {Environment.CurrentManagedThreadId}");

        await Task.Delay(2000, cancellationToken);

        logger.WriteLine($"Nach await - Thread ID: {Environment.CurrentManagedThreadId}");
        logger.WriteLine("Ende");
    }

    public string Description =>
        """
        Zeigt die Grundlagen von async und await.

        Die Methode startet synchron, wartet dann asynchron mit Task.Delay
        und läuft anschliessend weiter.

        Dabei wird sichtbar, dass await den Thread nicht blockiert,
        sondern die Ausführung später fortsetzt.
        """;
}