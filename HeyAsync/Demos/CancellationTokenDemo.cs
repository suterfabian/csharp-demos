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

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        using CancellationTokenSource demoTimeout = new(TimeSpan.FromSeconds(3));
        using CancellationTokenSource linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, demoTimeout.Token);

        for (int i = 1; i <= 10; i++)
        {
            linkedCts.Token.ThrowIfCancellationRequested();

            _logger.WriteLine($"Schritt {i}");
            await Task.Delay(700, linkedCts.Token);
        }
    }
}