using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class PeriodicTimerDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 24;
    public string Title => "24 - PeriodicTimer";

    public PeriodicTimerDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(500));

        for (int i = 1; i <= 5; i++)
        {
            await timer.WaitForNextTickAsync();
            _logger.WriteLine($"Tick {i}");
        }

        _logger.WriteLine("Timer beendet.");
    }
}