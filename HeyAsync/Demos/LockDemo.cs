using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class LockDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;
    private readonly object _counterLock = new();

    public int Order => 5;
    public string Title => "05 - Lock";

    public LockDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        int counter = 0;

        await Task.Run(() =>
        {
            Parallel.For(0, 100_000, _ =>
            {
                lock (_counterLock)
                {
                    counter++;
                }
            });
        });

        _logger.WriteLine("Erwartet: 100000");
        _logger.WriteLine($"Tatsächlich: {counter}");
        _logger.WriteLine("Der lock schützt den kritischen Bereich.");
    }
}