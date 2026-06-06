using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class SemaphoreSlimDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 7;
    public string Title => "07 - SemaphoreSlim";

    public SemaphoreSlimDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        using SemaphoreSlim semaphore = new(2);

        List<Task> tasks = Enumerable.Range(1, 5)
            .Select(async number =>
            {
                await semaphore.WaitAsync();

                try
                {
                    _logger.WriteLine($"Task {number} startet.");
                    await Task.Delay(1000);
                    _logger.WriteLine($"Task {number} fertig.");
                }
                finally
                {
                    semaphore.Release();
                }
            })
            .ToList();

        await Task.WhenAll(tasks);

        _logger.WriteLine("Maximal 2 Tasks liefen gleichzeitig.");
    }
}