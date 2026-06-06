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

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        TaskCompletionSource<string> tcs = new();

        _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            tcs.SetResult("Signal empfangen.");
        });

        _logger.WriteLine("Warte auf externes Signal...");

        string result = await tcs.Task;

        _logger.WriteLine(result);
    }
}