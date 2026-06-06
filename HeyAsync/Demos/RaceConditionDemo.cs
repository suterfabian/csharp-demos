using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class RaceConditionDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 4;
    public string Title => "04 - Race Condition";

    public RaceConditionDemo(IUiLogger logger)
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
                counter++;
            });
        }, cancellationToken);

        _logger.WriteLine("Erwartet: 100000");
        _logger.WriteLine($"Tatsächlich: {counter}");
        _logger.WriteLine("counter++ ist nicht atomar.");
    }
}