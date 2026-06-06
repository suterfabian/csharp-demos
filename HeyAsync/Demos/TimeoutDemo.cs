using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class TimeoutDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 15;
    public string Title => "15 - Timeout";

    public TimeoutDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        Task operation = Task.Delay(3000);
        Task timeout = Task.Delay(1000);

        Task finished = await Task.WhenAny(operation, timeout);

        if (finished == timeout)
        {
            _logger.WriteLine("Timeout erreicht.");
            return;
        }

        _logger.WriteLine("Operation erfolgreich.");
    }
}