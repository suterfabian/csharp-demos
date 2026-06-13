using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class DebounceDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int SortOrder => 27;
    public string Title => "27 - Debounce";

    public DebounceDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        using CancellationTokenSource localCts = new();
        using CancellationTokenSource linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, localCts.Token);

        CancellationTokenSource? debounceCts = null;

        try
        {
            for (int i = 1; i <= 5; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                debounceCts?.Cancel();
                debounceCts?.Dispose();

                debounceCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                _logger.WriteLine($"Eingabe {i}");

                _ = RunDebouncedAsync(i, debounceCts.Token);

                await Task.Delay(200, cancellationToken);
            }

            await Task.Delay(1000, cancellationToken);
        }
        finally
        {
            debounceCts?.Cancel();
            debounceCts?.Dispose();
        }
    }

    private async Task RunDebouncedAsync(int value, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(500, cancellationToken);
            _logger.WriteLine($"Verarbeitet nach Ruhezeit: {value}");
        }
        catch (OperationCanceledException)
        {
        }
    }
}