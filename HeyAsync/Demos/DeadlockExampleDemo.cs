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

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine("Problematisch in WPF wäre z.B.: SomeAsync().Result");
        _logger.WriteLine("Das kann den UI-Thread blockieren.");

        string result = await SomeAsync(cancellationToken);

        _logger.WriteLine($"Richtig mit await: {result}");
    }

    private static async Task<string> SomeAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);
        return "fertig";
    }
}