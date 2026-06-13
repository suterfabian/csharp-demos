using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class RetryWithBackoffDemo(IUiLogger logger) : IAsyncDemo
{
    private int _unstableOperationCallCount;

    public int SortOrder => 23;
    public string Title => "23 - Retry mit Backoff";

    // Backoff = Verzögerung vor dem nächsten Retry.
    // Die Verzögerung wird typischerweise mit jedem Versuch erhöht.
    //
    // Jitter = zusätzlicher Zufallsanteil zur Verzögerung.
    // Dadurch retryen parallele Prozesse nicht exakt gleichzeitig.
    //
    // Linear Backoff:          var delay = attempt * 500;
    // Exponential Backoff:     var delay = Math.Pow(2, attempt) * 100;
    //
    // Backoff + Jitter:
    // Die Basis-Verzögerung wird um einen kleinen Zufallswert erweitert.
    //
    // var baseDelay = Math.Pow(2, attempt) * 100;
    // var delay = baseDelay + Random.Shared.Next(0, 500);
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        _unstableOperationCallCount = 0;

        for (var attempt = 1; attempt <= 8; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await UnstableOperationAsync(cancellationToken);
                logger.WriteLine("Operation erfolgreich.");
                return;
            }
            catch (InvalidOperationException ex)
            {
                var delay = attempt * 500;

                logger.WriteLine($"Versuch {attempt} fehlgeschlagen: {ex.Message}");
                logger.WriteLine($"Warte {delay} ms.");

                await Task.Delay(delay, cancellationToken);
            }
        }

        logger.WriteLine("Alle Versuche fehlgeschlagen.");
    }

    private async Task UnstableOperationAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(200, cancellationToken);

        _unstableOperationCallCount++;

        if (_unstableOperationCallCount < 6)
        {
            throw new InvalidOperationException("Temporärer Fehler.");
        }
    }
}