using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class AsyncLockDemo(IUiLogger logger) : IAsyncDemo
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public int SortOrder => 21;
    public string Title => "21 - Async Lock";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var tasks = Enumerable.Range(1, 5)
            .Select(number => ProtectedOperationAsync(number, cancellationToken));

        await Task.WhenAll(tasks);
    }

    private async Task ProtectedOperationAsync(int number, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            logger.WriteLine($"Task {number} betritt kritischen Bereich.");
            await Task.Delay(700, cancellationToken);
            logger.WriteLine($"Task {number} verlässt kritischen Bereich.");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}