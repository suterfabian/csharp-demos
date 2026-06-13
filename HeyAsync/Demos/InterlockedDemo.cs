using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class InterlockedDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 6;
    public string Title => "06 - Interlocked";

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
                Interlocked.Increment(ref counter);
            });
        }, cancellationToken);

        logger.WriteLine("Erwartet: 100000");
        logger.WriteLine($"Tatsächlich: {counter}");
        logger.WriteLine("Interlocked.Increment ist atomar.");
    }
}