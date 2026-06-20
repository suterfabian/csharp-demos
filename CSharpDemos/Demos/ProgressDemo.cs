using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class ProgressDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 9;
    public string Title => "09 - Progress";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        // IProgress<int> merkt sich beim Erstellen den aktuellen SynchronizationContext.
        IProgress<int> progress = new Progress<int>(value =>
        {
            logger.WriteLine($"Fortschritt: {value}%"); // UI-Zugriff
        });

        await Task.Run(async () =>
        {
            for (var i = 0; i <= 100; i += 20)
            {
                cancellationToken.ThrowIfCancellationRequested();

                progress.Report(i);
                await Task.Delay(300, cancellationToken);
            }
        }, cancellationToken);

        logger.WriteLine("Fertig.");
    }
    
    public string Description =>
        """
        Demonstriert Fortschrittsmeldungen mit IProgress<T>.

        Die Arbeit läuft in einem Hintergrund-Task.
        Über progress.Report(...) werden Fortschrittswerte gemeldet.

        Progress<T> sorgt dafür, dass die Ausgabe wieder im ursprünglichen
        Kontext ausgeführt wird.
        """;
}