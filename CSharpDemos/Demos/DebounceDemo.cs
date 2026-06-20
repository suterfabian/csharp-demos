using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class DebounceDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 27;
    public string Title => "27 - Debounce";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        CancellationTokenSource? debounceCts = null;

        try
        {
            for (var i = 1; i <= 5; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                debounceCts?.Cancel();
                debounceCts?.Dispose();

                debounceCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                logger.WriteLine($"Eingabe {i}");

                _ = RunDebouncedAsync(i, debounceCts.Token);

                await Task.Delay(200, cancellationToken);
            }

            await Task.Delay(1000, cancellationToken);
        }
        finally
        {
            debounceCts?.Cancel();
            debounceCts?.Dispose();
        }
    }

    private async Task RunDebouncedAsync(int value, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(500, cancellationToken);
            logger.WriteLine($"Verarbeitet nach Ruhezeit: {value}");
        }
        catch (OperationCanceledException)
        {
        }
    }

    public string Description =>
        """
        Demonstriert das Debounce-Muster.
 
        Mehrere Eingaben werden in kurzen Abständen erzeugt.
        Jede neue Eingabe bricht die zuvor geplante Verarbeitung ab
        und startet die Wartezeit erneut.

        Erst wenn für eine definierte Zeitspanne (500 ms) keine weitere
        Eingabe erfolgt, wird die letzte Eingabe verarbeitet.

        Beispiel:
        Eingaben im Abstand von 200 ms:
        1 -> 2 -> 3 -> 4 -> 5

        Da jede neue Eingabe die vorherige Verarbeitung abbricht,
        wird nach Ablauf der Ruhezeit nur die letzte Eingabe (5)
        tatsächlich verarbeitet.
        """;
}