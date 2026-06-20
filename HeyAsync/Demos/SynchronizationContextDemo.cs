using HeyAsync.Services;

namespace HeyAsync.Demos;

/// <summary>
/// Demonstriert die Synchronisation zurück auf den ursprünglichen
/// SynchronizationContext.
///
/// Die Arbeit läuft auf einem Hintergrund-Thread.
/// Mit SynchronizationContext.Post(...) wird anschliessend
/// sicher in den UI-Kontext zurückgewechselt, um UI-Code
/// auszuführen.
///
/// Im Gegensatz zum Dispatcher ist SynchronizationContext
/// nicht an eine bestimmte UI-Technologie gebunden.
/// </summary>
public sealed class SynchronizationContextDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 34;
    public string Title => "34 - Synchronisation mit SynchronizationContext";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var uiContext = SynchronizationContext.Current;

        if (uiContext is null)
        {
            logger.WriteLine("Kein SynchronizationContext vorhanden.");
            return;
        }

        await Task.Run(async () =>
        {
            await Task.Delay(1000, cancellationToken);

            uiContext.Post(_ =>
            {
                logger.WriteLine($"UI-Update via SynchronizationContext. Thread: {Environment.CurrentManagedThreadId}");
            }, null);
        }, cancellationToken);
    }
}