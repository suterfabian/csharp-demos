using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class AsyncLockDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public int Order => 21;
    public string Title => "21 - Async Lock";

    public AsyncLockDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        IEnumerable<Task> tasks = Enumerable.Range(1, 3)
            .Select(number => ProtectedOperationAsync(number, cancellationToken));

        await Task.WhenAll(tasks);
    }

    private async Task ProtectedOperationAsync(int number, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            _logger.WriteLine($"Task {number} betritt kritischen Bereich.");
            await Task.Delay(700, cancellationToken);
            _logger.WriteLine($"Task {number} verlässt kritischen Bereich.");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}