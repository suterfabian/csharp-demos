using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class CancellationTokenDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 8;
    public string Title => "08 - CancellationToken";

    public CancellationTokenDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        using CancellationTokenSource cts = new();
        cts.CancelAfter(2500);

        try
        {
            for (int i = 1; i <= 10; i++)
            {
                cts.Token.ThrowIfCancellationRequested();

                _logger.WriteLine($"Schritt {i}");
                await Task.Delay(700, cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.WriteLine("Vorgang wurde abgebrochen.");
        }
    }
}