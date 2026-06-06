using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class UiFreezeDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 18;
    public string Title => "18 - UI Freeze";

    public UiFreezeDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine("CPU-Arbeit gehört nicht direkt auf den UI-Thread.");

        int result = await Task.Run(() =>
        {
            int sum = 0;

            for (int i = 0; i < 50_000_000; i++)
            {
                sum += i % 10;
            }

            return sum;
        });

        _logger.WriteLine($"Berechnung fertig: {result}");
    }
}