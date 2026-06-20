using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class TaskRunDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 2;
    public string Title => "02 - Task.Run";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        logger.WriteLine($"UI Thread vor Task.Run: {Environment.CurrentManagedThreadId}");

        var workerThreadId = await Task.Run(async () =>
        {
            await Task.Delay(1500, cancellationToken);
            return Environment.CurrentManagedThreadId;
        }, cancellationToken);

        logger.WriteLine($"Worker Thread in Task.Run: {workerThreadId}");
        logger.WriteLine($"UI Thread nach await: {Environment.CurrentManagedThreadId}");
    }
    
    public string Description =>
        """
        Demonstriert die Ausführung von Arbeit mit Task.Run.

        Die Arbeit wird auf einen ThreadPool-Thread ausgelagert.
        Die UI bleibt währenddessen frei und wird nicht blockiert.

        Nach dem await läuft die Methode im ursprünglichen Kontext weiter.
        """;
}