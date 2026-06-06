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

    public Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        _ = RunInBackgroundSafelyAsync();

        _logger.WriteLine("Hintergrundaufgabe wurde gestartet.");
        _logger.WriteLine("Fehler müssen intern behandelt werden.");

        return Task.CompletedTask;
    }

    private async Task RunInBackgroundSafelyAsync()
    {
        try
        {
            await Task.Delay(1000);
            _logger.WriteLine("Fire-and-forget Aufgabe fertig.");
        }
        catch (Exception ex)
        {
            _logger.WriteLine($"Fehler: {ex.Message}");
        }
    }
}