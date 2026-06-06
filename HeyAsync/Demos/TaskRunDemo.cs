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

    public async Task ExecuteAsync()
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine($"UI Thread vor Task.Run: {Environment.CurrentManagedThreadId}");

        int workerThreadId = await Task.Run(() =>
        {
            Thread.Sleep(1500);
            return Environment.CurrentManagedThreadId;
        });

        _logger.WriteLine($"Worker Thread in Task.Run: {workerThreadId}");
        _logger.WriteLine($"UI Thread nach await: {Environment.CurrentManagedThreadId}");
    }
}