using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class TaskCompletionSourceDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 25;
    public string Title => "25 - TaskCompletionSource";

    public TaskCompletionSourceDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        TaskCompletionSource<string> tcs = new();

        await using CancellationTokenRegistration registration =
            cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

        _ = Task.Run(async () =>
        {
            await Task.Delay(1000, cancellationToken);
            tcs.TrySetResult("Signal empfangen.");
        }, cancellationToken);

        _logger.WriteLine("Warte auf externes Signal...");

        string result = await tcs.Task;

        _logger.WriteLine(result);
    }
}