using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class RaceConditionDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 4;
    public string Title => "04 - Race Condition";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var counter = 0;

        await Task.Run(() =>
        {
            Parallel.For(0, 100_000, new ParallelOptions
            {
                CancellationToken = cancellationToken
            }, _ =>
            {
                counter++;
            });
        }, cancellationToken);

        logger.WriteLine("Erwartet: 100000");
        logger.WriteLine($"Tatsächlich: {counter}");
        logger.WriteLine("counter++ ist nicht atomar.");
    }
    
    public string Description =>
        """
        Demonstriert eine Race Condition bei parallelem Zugriff.

        Viele parallele Operationen erhöhen denselben Counter.
        Da counter++ nicht atomar ist, können einzelne Erhöhungen
        verloren gehen.

        Das tatsächliche Ergebnis kann deshalb kleiner sein als erwartet.
        """;
}