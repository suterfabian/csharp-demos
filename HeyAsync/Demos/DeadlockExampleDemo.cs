using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class DeadlockExampleDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 17;
    public string Title => "17 - Deadlock Beispiel";

    public DeadlockExampleDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine("Gefährlich wäre in WPF z.B.: SomeAsync().Result");
        _logger.WriteLine("Das kann den UI-Thread blockieren.");

        string result = await SomeAsync();

        _logger.WriteLine($"Richtig mit await: {result}");
    }

    private static async Task<string> SomeAsync()
    {
        await Task.Delay(500);
        return "fertig";
    }
}