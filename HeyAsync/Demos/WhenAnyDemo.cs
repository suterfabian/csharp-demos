using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class WhenAnyDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 14;
    public string Title => "14 - Task.WhenAny";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        var fast = LoadAsync("Schnell", 500, cancellationToken);
        var slow = LoadAsync("Langsam", 2000, cancellationToken);

        var finished = await Task.WhenAny(fast, slow);

        logger.WriteLine(await finished);
        logger.WriteLine("WhenAny liefert den zuerst fertigen Task.");
    }

    private static async Task<string> LoadAsync(string name, int delay, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken);
        return $"{name} ist fertig.";
    }
}