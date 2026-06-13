using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class TaskRunDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 2;
    public string Title => "02 - Task.Run";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        logger.WriteLine($"UI Thread vor Task.Run: {Environment.CurrentManagedThreadId}");

        var workerThreadId = await Task.Run(async () =>
        {
            await Task.Delay(1500, cancellationToken);
            return Environment.CurrentManagedThreadId;
        }, cancellationToken);

        logger.WriteLine($"Worker Thread in Task.Run: {workerThreadId}");
        logger.WriteLine($"UI Thread nach await: {Environment.CurrentManagedThreadId}");
    }
}