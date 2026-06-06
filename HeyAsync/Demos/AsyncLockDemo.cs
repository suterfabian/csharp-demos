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

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        List<Task> tasks = Enumerable.Range(1, 3)
            .Select(ProtectedOperationAsync)
            .ToList();

        await Task.WhenAll(tasks);
    }

    private async Task ProtectedOperationAsync(int number)
    {
        await _semaphore.WaitAsync();

        try
        {
            _logger.WriteLine($"Task {number} betritt kritischen Bereich.");
            await Task.Delay(700);
            _logger.WriteLine($"Task {number} verlässt kritischen Bereich.");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}