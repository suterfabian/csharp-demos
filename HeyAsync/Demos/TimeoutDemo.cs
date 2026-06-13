using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class TimeoutDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 15;
    public string Title => "15 - Timeout";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        using CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(1));
        using var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await Task.Delay(3000, linkedCts.Token);
            logger.WriteLine("Operation erfolgreich.");
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            logger.WriteLine("Timeout erreicht.");
        }
    }
}