using HeyAsync.Models;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class TimeoutDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 15;
    public string Title => "15 - Timeout";
    public DemoType Type => DemoType.Async;

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
    
    public string Description =>
        """
        Demonstriert einen Timeout für eine asynchrone Operation.

        Ein interner CancellationTokenSource bricht nach einer Sekunde ab.
        Dieser wird mit dem externen CancellationToken kombiniert.

        Wird die Operation nicht rechtzeitig fertig, wird sie abgebrochen
        und als Timeout behandelt.
        """;
}