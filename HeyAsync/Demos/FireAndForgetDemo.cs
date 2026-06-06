using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class FireAndForgetDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 16;
    public string Title => "16 - Fire and Forget";

    public FireAndForgetDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        _ = RunInBackgroundSafelyAsync(cancellationToken);

        _logger.WriteLine("Hintergrundaufgabe gestartet.");
        _logger.WriteLine("Fehler müssen intern behandelt werden.");

        return Task.CompletedTask;
    }

    private async Task RunInBackgroundSafelyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(1000, cancellationToken);
            _logger.WriteLine("Fire-and-forget Aufgabe fertig.");
        }
        catch (OperationCanceledException)
        {
            _logger.WriteLine("Fire-and-forget Aufgabe abgebrochen.");
        }
        catch (Exception ex)
        {
            _logger.WriteLine($"Fehler: {ex.Message}");
        }
    }
}