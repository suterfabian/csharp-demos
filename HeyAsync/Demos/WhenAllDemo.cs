using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class WhenAllDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 13;
    public string Title => "13 - Task.WhenAll";

    public WhenAllDemo(IUiLogger logger)
    {
        _logger = logger;
    }


    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        Task<string> first = LoadAsync("A", 1000, cancellationToken);
        Task<string> second = LoadAsync("B", 1500, cancellationToken);
        Task<string> third = LoadAsync("C", 500, cancellationToken);

        string[] results = await Task.WhenAll(first, second, third);

        foreach (string result in results)
        {
            _logger.WriteLine(result);
        }
    }

    private static async Task<string> LoadAsync(string name, int delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken);
        return $"Operation {name} fertig.";
    }
}