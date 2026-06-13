using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class BasicAsyncDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 1;
    public string Title => "01 - Basic async/await";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        logger.WriteLine("Start");
        logger.WriteLine($"Vor await - Thread ID: {Environment.CurrentManagedThreadId}");

        await Task.Delay(2000, cancellationToken);

        logger.WriteLine($"Nach await - Thread ID: {Environment.CurrentManagedThreadId}");
        logger.WriteLine("Ende");
    }
}