using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class SimpleRetryDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;
    private int _unstableOperationCallCount;

    public int Order => 22;
    public string Title => "22 - Retry einfach";

    public SimpleRetryDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        _unstableOperationCallCount = 0;

        for (int attempt = 1; attempt <= 3; attempt++)
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
                _logger.WriteLine($"Versuch {attempt} fehlgeschlagen: {ex.Message}");
            }
        }

        _logger.WriteLine("Alle Versuche fehlgeschlagen.");
    }

    private async Task UnstableOperationAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(300, cancellationToken);

        _unstableOperationCallCount++;

        if (_unstableOperationCallCount < 3)
        {
            throw new InvalidOperationException("Transienter Fehler.");
        }
    }
}