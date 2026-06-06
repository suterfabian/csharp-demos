using System.Threading.Channels;
using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class QueueWorkerDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 29;
    public string Title => "29 - Queue Worker";

    public QueueWorkerDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        Channel<string> queue = Channel.CreateUnbounded<string>();

        Task worker = Task.Run(async () =>
        {
            await foreach (string command in queue.Reader.ReadAllAsync(cancellationToken))
            {
                _logger.WriteLine($"Verarbeite: {command}");
                await Task.Delay(400, cancellationToken);
            }
        }, cancellationToken);

        for (int i = 1; i <= 5; i++)
        {
            await queue.Writer.WriteAsync($"Command {i}", cancellationToken);
            _logger.WriteLine($"Eingereiht: Command {i}");
        }

        queue.Writer.Complete();

        await worker;

        _logger.WriteLine("Queue leer.");
    }
}