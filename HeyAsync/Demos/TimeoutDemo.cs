using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class TimeoutDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 15;
    public string Title => "15 - Timeout";

    public TimeoutDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        using CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(1));
        using CancellationTokenSource linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await Task.Delay(3000, linkedCts.Token);
            _logger.WriteLine("Operation erfolgreich.");
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.WriteLine("Timeout erreicht.");
        }
    }
}