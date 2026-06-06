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

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        Task<string> first = LoadAsync("A", 1000);
        Task<string> second = LoadAsync("B", 1500);
        Task<string> third = LoadAsync("C", 500);

        string[] results = await Task.WhenAll(first, second, third);

        foreach (string result in results)
        {
            _logger.WriteLine(result);
        }
    }

    private async Task<string> LoadAsync(string name, int delay)
    {
        await Task.Delay(delay);
        return $"Operation {name} fertig.";
    }
}