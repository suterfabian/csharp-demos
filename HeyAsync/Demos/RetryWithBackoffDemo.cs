using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class RetryWithBackoffDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;
    private int _unstableOperationCallCount;

    public int Order => 23;
    public string Title => "23 - Retry mit Backoff";

    public RetryWithBackoffDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        _unstableOperationCallCount = 0;

        for (int attempt = 1; attempt <= 4; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await UnstableOperationAsync(cancellationToken);
                _logger.WriteLine("Operation erfolgreich.");
                return;
            }
            catch (InvalidOperationException ex)
            {
                int delay = attempt * 500;

                _logger.WriteLine($"Versuch {attempt} fehlgeschlagen: {ex.Message}");
                _logger.WriteLine($"Warte {delay} ms.");

                await Task.Delay(delay, cancellationToken);
            }
        }

        _logger.WriteLine("Alle Versuche fehlgeschlagen.");
    }

    private async Task UnstableOperationAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(200, cancellationToken);

        _unstableOperationCallCount++;

        if (_unstableOperationCallCount < 3)
        {
            throw new InvalidOperationException("Temporärer Fehler.");
        }
    }
}