using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class AsyncVoidProblemDemo : IAsyncDemo
{
    private readonly IUiLogger _logger;

    public int Order => 11;
    public string Title => "11 - async void Problem";

    public AsyncVoidProblemDemo(IUiLogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.WriteHeader(Title);

        _logger.WriteLine("async void sollte nur für UI-Events verwendet werden.");
        _logger.WriteLine("Besser ist async Task, weil es awaitbar und testbar ist.");

        await SafeAsyncMethod(cancellationToken);

        _logger.WriteLine("Diese Demo verwendet bewusst async Task.");
    }

    private async Task SafeAsyncMethod(CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);
        _logger.WriteLine("SafeAsyncMethod wurde awaited.");
    }
}