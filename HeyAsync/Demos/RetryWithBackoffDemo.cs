using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class RetryWithBackoffDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;
    private int _attempt;

    public int Order => 23;
    public string Title => "23 - Retry mit Backoff";

    public RetryWithBackoffDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        for (int attempt = 1; attempt <= 4; attempt++)
        {
            try
            {
                await UnstableOperationAsync();
                _logger.WriteLine("Operation erfolgreich.");
                return;
            }
            catch (Exception ex)
            {
                int delay = attempt * 500;
                _logger.WriteLine($"Versuch {attempt} fehlgeschlagen: {ex.Message}");
                _logger.WriteLine($"Warte {delay} ms.");

                await Task.Delay(delay);
            }
        }
    }

    private async Task UnstableOperationAsync()
    {
        await Task.Delay(200);
        _attempt++;

        if (_attempt < 3)
        {
            throw new InvalidOperationException("Temporärer Fehler.");
        }
    }
}