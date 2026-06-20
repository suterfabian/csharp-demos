using CSharpDemos.Models;
using CSharpDemos.Services;

namespace CSharpDemos.Demos;

public sealed class PeriodicTimerDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 24;
    public string Title => "24 - PeriodicTimer";
    public DemoType Type => DemoType.Async;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(500));

        for (var i = 1; i <= 5; i++)
        {
            await timer.WaitForNextTickAsync(cancellationToken);
            logger.WriteLine($"Tick {i}");
        }

        logger.WriteLine("Timer beendet.");
    }

    public string Description =>
        """
        Demonstriert einen asynchronen PeriodicTimer.

        Der Timer erzeugt in festen Zeitabständen einen Tick.
        Mit WaitForNextTickAsync wird asynchron auf den nächsten Tick gewartet.

        Die Schleife läuft, bis die gewünschte Anzahl Ticks erreicht ist
        oder der CancellationToken abgebrochen wird.
        
        PeriodicTimer erzeugt wiederkehrende Ticks in einem festen Intervall.
        
        -----------------------------------------------------------------------
        
        WaitForNextTickAsync(...)
            wartet asynchron auf den nächsten Tick,
            ohne den UI-Thread zu blockieren.
        
        Der CancellationToken sorgt dafür,
            dass das Warten sauber abgebrochen werden kann.
        
        using sorgt dafür,
            dass der Timer nach der Demo wieder freigegeben wird.
        """;
}

/*  Periodisches Polling
    using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

    while (await timer.WaitForNextTickAsync(cancellationToken))
    {
        await CheckConnectionAsync(cancellationToken);
    }
*/