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

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        Task<string> fast = LoadAsync("Schnell", 500, cancellationToken);
        Task<string> slow = LoadAsync("Langsam", 2000, cancellationToken);

        Task<string> finished = await Task.WhenAny(fast, slow);

        _logger.WriteLine(await finished);
        _logger.WriteLine("WhenAny liefert den zuerst fertigen Task.");
    }

    private static async Task<string> LoadAsync(string name, int delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken);
        return $"{name} ist fertig.";
    }
}