using HeyAsync.Services;

namespace HeyAsync.Demos;

public sealed class AsyncVoidProblemDemo(IUiLogger logger) : IAsyncDemo
{
    public int SortOrder => 11;
    public string Title => "11 - async void Problem";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.WriteHeader(Title);

        logger.WriteLine("async void sollte nur für UI-Events verwendet werden.");
        logger.WriteLine("Besser ist async Task, weil es awaitbar und testbar ist.");

        await SafeAsyncMethod(cancellationToken);

        logger.WriteLine("Diese Demo verwendet bewusst async Task.");
    }

    private async Task SafeAsyncMethod(CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);
        logger.WriteLine("SafeAsyncMethod wurde awaited.");
    }
}