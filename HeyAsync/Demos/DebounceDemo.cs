using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class DebounceDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 27;
    public string Title => "27 - Debounce";

    public DebounceDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        CancellationTokenSource? debounceCts = null;

        for (int i = 1; i <= 5; i++)
        {
            debounceCts?.Cancel();
            debounceCts = new CancellationTokenSource();

            _logger.WriteLine($"Eingabe {i}");

            _ = RunDebouncedAsync(i, debounceCts.Token);

            await Task.Delay(200);
        }

        await Task.Delay(1000);
    }

    private async Task RunDebouncedAsync(int value, CancellationToken token)
    {
        try
        {
            await Task.Delay(500, token);
            _logger.WriteLine($"Verarbeitet nach Ruhezeit: {value}");
        }
        catch (OperationCanceledException)
        {
        }
    }
}