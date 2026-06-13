using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class FireAndForgetDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 16;
    public string Title => "16 - Fire and Forget";

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        // kein await!!!
        _ = RunInBackgroundSafelyAsync(cancellationToken);

        logger.WriteLine("Hintergrundaufgabe gestartet.");
        logger.WriteLine("Fehler müssen intern behandelt werden.");

        return Task.CompletedTask;
    }

    private async Task RunInBackgroundSafelyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(4000, cancellationToken);
            logger.WriteLine("Fire-and-forget Aufgabe fertig.");
        }
        catch (OperationCanceledException)
        {
            logger.WriteLine("Fire-and-forget Aufgabe abgebrochen.");
        }
        catch (Exception ex)
        {
            logger.WriteLine($"Fehler: {ex.Message}");
        }
    }
}