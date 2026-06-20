using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class ConfigureAwaitDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 10;
    public string Title => "10 - ConfigureAwait";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        logger.WriteLine($"Vor await: {Environment.CurrentManagedThreadId}");

        await Task.Delay(500, cancellationToken);

        logger.WriteLine($"Nach normalem await: {Environment.CurrentManagedThreadId}");
        
        // ConfigureAwait(true)
        //     = wie normales await
        //     = versuche zurück auf den ursprünglichen Context
        //
        // ConfigureAwait(false)
        //     = kein Zurückwechseln erzwingen
        //     = Fortsetzung darf auf ThreadPool laufen
        await Task.Delay(500, cancellationToken).ConfigureAwait(false);

        logger.WriteLine($"Nach ConfigureAwait(true): {Environment.CurrentManagedThreadId}");
    }

    public string Description =>
        """
        Demonstriert den Unterschied zwischen normalem await
        und ConfigureAwait(false).

        Ein normales await versucht, nach dem Warten in den ursprünglichen
        Kontext zurückzukehren.

        ConfigureAwait(false) erzwingt diese Rückkehr nicht.
        Die Fortsetzung darf dadurch auf einem ThreadPool-Thread laufen.
        """;
}