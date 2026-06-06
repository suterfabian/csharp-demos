using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class InterlockedDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 6;
    public string Title => "06 - Interlocked";

    public InterlockedDemo(IUiLogger logger)
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
                Interlocked.Increment(ref counter);
            });
        });

        _logger.WriteLine("Erwartet: 100000");
        _logger.WriteLine($"Tatsächlich: {counter}");
        _logger.WriteLine("Interlocked.Increment ist atomar.");
    }
}