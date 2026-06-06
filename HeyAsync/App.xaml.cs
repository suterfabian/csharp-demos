using System.Windows;
using HeyAsync.Demos;
using HeyAsync.Services;
using HeyAsync.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace HeyAsync;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ServiceCollection services = new();

        services.AddSingleton<IUiLogger, UiLogger>();

        services.AddTransient<IAsyncDemo, BasicAsyncDemo>();
        services.AddTransient<IAsyncDemo, TaskRunDemo>();
        services.AddTransient<IAsyncDemo, DispatcherDemo>();
        services.AddTransient<IAsyncDemo, RaceConditionDemo>();
        services.AddTransient<IAsyncDemo, LockDemo>();
        services.AddTransient<IAsyncDemo, InterlockedDemo>();
        services.AddTransient<IAsyncDemo, SemaphoreSlimDemo>();
        services.AddTransient<IAsyncDemo, CancellationTokenDemo>();
        services.AddTransient<IAsyncDemo, ProgressDemo>();
        services.AddTransient<IAsyncDemo, ConfigureAwaitDemo>();
        services.AddTransient<IAsyncDemo, AsyncVoidProblemDemo>();
        services.AddTransient<IAsyncDemo, ExceptionHandlingDemo>();
        services.AddTransient<IAsyncDemo, WhenAllDemo>();
        services.AddTransient<IAsyncDemo, WhenAnyDemo>();
        services.AddTransient<IAsyncDemo, TimeoutDemo>();
        services.AddTransient<IAsyncDemo, FireAndForgetDemo>();
        services.AddTransient<IAsyncDemo, DeadlockExampleDemo>();
        services.AddTransient<IAsyncDemo, UiFreezeDemo>();
        services.AddTransient<IAsyncDemo, ProducerConsumerDemo>();
        services.AddTransient<IAsyncDemo, ChannelDemo>();
        services.AddTransient<IAsyncDemo, AsyncLockDemo>();
        services.AddTransient<IAsyncDemo, SimpleRetryDemo>();
        services.AddTransient<IAsyncDemo, RetryWithBackoffDemo>();
        services.AddTransient<IAsyncDemo, PeriodicTimerDemo>();
        services.AddTransient<IAsyncDemo, TaskCompletionSourceDemo>();
        services.AddTransient<IAsyncDemo, AsyncStreamDemo>();
        services.AddTransient<IAsyncDemo, DebounceDemo>();
        services.AddTransient<IAsyncDemo, ThrottleDemo>();
        services.AddTransient<IAsyncDemo, QueueWorkerDemo>();
        services.AddTransient<IAsyncDemo, BleCommandSimulationDemo>();
        services.AddTransient<IAsyncDemo, FirmwareUploadSimulationDemo>();
        services.AddTransient<IAsyncDemo, FirmwareUploadTransientRetryDemo>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
