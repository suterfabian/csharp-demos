using HeyAsync.Services;

namespace HeyAsync.Demos;

// SynchronizationContext = allgemeine Abstraktion über den UI-Kontext
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
            await Task.Delay(3000, cancellationToken);

            uiContext.Post(_ =>
            {
                logger.WriteLine($"UI-Update via SynchronizationContext. Thread: {Environment.CurrentManagedThreadId}");
            }, null);
        }, cancellationToken);
    }
}