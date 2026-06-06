using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class WhenAnyDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 14;
    public string Title => "14 - Task.WhenAny";

    public WhenAnyDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        Task<string> fast = LoadAsync("Schnell", 500);
        Task<string> slow = LoadAsync("Langsam", 2000);

        Task<string> finished = await Task.WhenAny(fast, slow);

        _logger.WriteLine(await finished);
        _logger.WriteLine("WhenAny liefert den zuerst fertigen Task.");
    }

    private async Task<string> LoadAsync(string name, int delay)
    {
        await Task.Delay(delay);
        return $"{name} ist fertig.";
    }
}