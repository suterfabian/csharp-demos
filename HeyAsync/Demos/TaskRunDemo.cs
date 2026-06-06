using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class TaskRunDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 2;
    public string Title => "02 - Task.Run";

    public TaskRunDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine($"UI Thread vor Task.Run: {Environment.CurrentManagedThreadId}");

        int workerThreadId = await Task.Run(async () =>
        {
            // Thread.Sleep(1500);
            await Task.Delay(1500, cancellationToken);
            return Environment.CurrentManagedThreadId;
        });

        _logger.WriteLine($"Worker Thread in Task.Run: {workerThreadId}");
        _logger.WriteLine($"UI Thread nach await: {Environment.CurrentManagedThreadId}");
    }
}