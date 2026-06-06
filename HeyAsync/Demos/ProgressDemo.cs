using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class ProgressDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 9;
    public string Title => "09 - Progress";

    public ProgressDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        IProgress<int> progress = new Progress<int>(value =>
        {
            _logger.WriteLine($"Fortschritt: {value}%");
        });

        await Task.Run(async () =>
        {
            for (int i = 0; i <= 100; i += 20)
            {
                cancellationToken.ThrowIfCancellationRequested();

                progress.Report(i);
                await Task.Delay(300, cancellationToken);
            }
        }, cancellationToken);

        _logger.WriteLine("Fertig.");
    }
}