using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class SimpleRetryDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;
    private int _attempt;

    public int Order => 22;
    public string Title => "22 - Retry einfach";

    public SimpleRetryDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        for (int attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                await UnstableOperationAsync();
                _logger.WriteLine("Operation erfolgreich.");
                return;
            }
            catch (Exception ex)
            {
                _logger.WriteLine($"Versuch {attempt} fehlgeschlagen: {ex.Message}");
            }
        }

        _logger.WriteLine("Alle Versuche fehlgeschlagen.");
    }

    private async Task UnstableOperationAsync()
    {
        await Task.Delay(300);
        _attempt++;

        if (_attempt < 3)
        {
            throw new InvalidOperationException("Transienter Fehler.");
        }
    }
}