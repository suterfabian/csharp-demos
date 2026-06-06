using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class LockDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;
    private readonly object _syncRoot = new();

    public int Order => 5;
    public string Title => "05 - Lock";

    public LockDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        int counter = 0;

        await Task.Run(() =>
        {
            Parallel.For(0, 100_000, new ParallelOptions
            {
                CancellationToken = cancellationToken
            }, _ =>
            {
                lock (_syncRoot)
                {
                    counter++;
                }
            });
        }, cancellationToken);

        _logger.WriteLine("Erwartet: 100000");
        _logger.WriteLine($"Tatsächlich: {counter}");
        _logger.WriteLine("lock schützt den kritischen Bereich.");
    }
}