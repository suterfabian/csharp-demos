using System.Windows;
using HeyAsync.Services;

namespace HeyAsync;

public partial class MainWindow : Window
{
    private readonly object _counterLock = new();
    private readonly UiLogger _logger;

    public MainWindow()
    {
        InitializeComponent();

        _logger = new UiLogger(OutputTextBox);

        _logger.WriteLine("Async WPF Lab gestartet.");
        _logger.WriteLine($"UI Thread ID: {Environment.CurrentManagedThreadId}");
        _logger.WriteLine("");
    }

    private async void BasicAsyncButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("01 - Basic async/await");

        _logger.WriteLine("Start");
        _logger.WriteLine($"Vor await - Thread ID: {Environment.CurrentManagedThreadId}");

        await Task.Delay(2000);

        _logger.WriteLine($"Nach await - Thread ID: {Environment.CurrentManagedThreadId}");
        _logger.WriteLine("Ende");
    }

    private async void TaskRunButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("02 - Task.Run");

        _logger.WriteLine($"UI Thread vor Task.Run: {Environment.CurrentManagedThreadId}");

        int result = await Task.Run(() =>
        {
            Thread.Sleep(1500);
            return Environment.CurrentManagedThreadId;
        });

        _logger.WriteLine($"Worker Thread in Task.Run: {result}");
        _logger.WriteLine($"UI Thread nach await: {Environment.CurrentManagedThreadId}");
    }

    private void DispatcherButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("03 - Dispatcher");

        Task.Run(() =>
        {
            int backgroundThreadId = Environment.CurrentManagedThreadId;

            Dispatcher.Invoke(() =>
            {
                _logger.WriteLine($"Background Thread: {backgroundThreadId}");
                _logger.WriteLine($"Zurück auf UI Thread: {Environment.CurrentManagedThreadId}");
                _logger.WriteLine("UI-Ausgabe wurde über Dispatcher.Invoke geschrieben.");
            });
        });
    }

    private async void RaceConditionButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("04 - Race Condition");

        int counter = 0;

        await Task.Run(() =>
        {
            Parallel.For(0, 100_000, _ =>
            {
                counter++;
            });
        });

        _logger.WriteLine("Erwartet: 100000");
        _logger.WriteLine($"Tatsächlich: {counter}");
        _logger.WriteLine("Die Abweichung entsteht durch gleichzeitige Schreibzugriffe.");
    }

    private async void LockButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.WriteHeader("05 - Lock");

        int counter = 0;

        await Task.Run(() =>
        {
            Parallel.For(0, 100_000, _ =>
            {
                lock (_counterLock)
                {
                    counter++;
                }
            });
        });

        _logger.WriteLine("Erwartet: 100000");
        _logger.WriteLine($"Tatsächlich: {counter}");
        _logger.WriteLine("Der lock schützt den kritischen Bereich.");
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.Clear();
    }
}