using System.Runtime.CompilerServices;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class AsyncStreamDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 26;
    public string Title => "26 - Async Stream";

    public AsyncStreamDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        await foreach (int value in GenerateValuesAsync(cancellationToken))
        {
            _logger.WriteLine($"Wert: {value}");
        }
    }

    private static async IAsyncEnumerable<int> GenerateValuesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (int i = 1; i <= 5; i++)
        {
            await Task.Delay(400, cancellationToken);
            yield return i;
        }
    }
}