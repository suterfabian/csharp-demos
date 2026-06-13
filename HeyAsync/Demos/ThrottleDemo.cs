using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ThrottleDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int SortOrder => 28;
    public string Title => "28 - Throttle";

    public ThrottleDemo(IUiLogger logger)
    {
        _logger = logger;
    }


    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        DateTime lastExecution = DateTime.MinValue;

        for (int i = 1; i <= 10; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DateTime now = DateTime.Now;

            if (now - lastExecution >= TimeSpan.FromMilliseconds(700))
            {
                lastExecution = now;
                _logger.WriteLine($"Ausgeführt: Event {i}");
            }
            else
            {
                _logger.WriteLine($"Übersprungen: Event {i}");
            }

            await Task.Delay(200, cancellationToken);
        }
    }
}