using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class RaceConditionDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 4;
    public string Title => "04 - Race Condition";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var counter = 0;

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

        logger.WriteLine("Erwartet: 100000");
        logger.WriteLine($"Tatsächlich: {counter}");
        logger.WriteLine("counter++ ist nicht atomar.");
    }
}