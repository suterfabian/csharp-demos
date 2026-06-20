using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class SemaphoreSlimDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 7;
    public string Title => "07 - SemaphoreSlim";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using SemaphoreSlim semaphore = new(2);

        List<Task> tasks = [];

        for (var number = 1; number <= 5; number++)
        {
            tasks.Add(RunTaskAsync(number, semaphore, cancellationToken));
        }

        await Task.WhenAll(tasks);

        logger.WriteLine("Maximal 2 Tasks liefen gleichzeitig.");
    }

    private async Task RunTaskAsync(
        int number,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            logger.WriteLine($"Task {number} startet.");
            await Task.Delay(1000, cancellationToken);
            logger.WriteLine($"Task {number} fertig.");
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    public string Description =>
        """
        Demonstriert die Begrenzung paralleler Aufgaben mit SemaphoreSlim.

        Es werden mehrere Tasks gestartet, aber nur zwei dürfen gleichzeitig
        im geschützten Bereich arbeiten.

        WaitAsync wartet asynchron auf einen freien Platz.
        Release gibt den Platz wieder frei.
        """;
}