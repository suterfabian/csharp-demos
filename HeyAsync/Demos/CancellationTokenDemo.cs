using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class CancellationTokenDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 8;
    public string Title => "08 - CancellationToken";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using CancellationTokenSource demoTimeout = new(TimeSpan.FromSeconds(5));
        using var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, demoTimeout.Token);

        for (var i = 1; i <= 10; i++)
        {
            linkedCts.Token.ThrowIfCancellationRequested();

            logger.WriteLine($"Schritt {i}");
            await Task.Delay(1000, linkedCts.Token);
        }
    }
}