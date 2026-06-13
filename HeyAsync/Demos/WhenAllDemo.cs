using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class WhenAllDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 13;
    public string Title => "13 - Task.WhenAll";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var first = LoadAsync("A", 1000, cancellationToken);
        var second = LoadAsync("B", 1500, cancellationToken);
        var third = LoadAsync("C", 500, cancellationToken);

        var results = await Task.WhenAll(first, second, third);

        foreach (var result in results)
        {
            logger.WriteLine(result);
        }
    }

    private static async Task<string> LoadAsync(string name, int delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken);
        return $"Operation {name} fertig.";
    }
}