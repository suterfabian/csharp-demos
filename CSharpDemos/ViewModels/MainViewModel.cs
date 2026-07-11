using System.Collections.ObjectModel;
using CSharpDemos.Demos;
using CSharpDemos.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CSharpDemos.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IUiLogger _logger;

    private CancellationTokenSource? _currentCancellationTokenSource;

    public ObservableCollection<AsyncDemoButtonViewModel> DemoButtons { get; }

    [ObservableProperty]
    public partial string OutputText { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _isDemoRunning;

    public MainViewModel(IEnumerable<IAsyncDemo> demos, IUiLogger logger)
    {
        _logger = logger;

        DemoButtons = new ObservableCollection<AsyncDemoButtonViewModel>(
            demos
                .OrderBy(demo => demo.SortOrder)
                .Select(demo => new AsyncDemoButtonViewModel(
                    demo.Title,
                    new AsyncRelayCommand(() => RunDemoAsync(demo), CanRunDemo),
                    demo.Description)));

        _logger.OutputChanged += OnOutputChanged;
    }

    private bool CanRunDemo()
    {
        return !IsDemoRunning;
    }

    private async Task RunDemoAsync(IAsyncDemo demo)
    {
        IsDemoRunning = true;
        RefreshDemoButtons();

        _currentCancellationTokenSource = new CancellationTokenSource();

        try
        {
            await demo.ExecuteAsync(_currentCancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            _logger.WriteLine("Demo wurde abgebrochen.");
        }
        catch (Exception ex)
        {
            _logger.WriteLine($"Fehler: {ex.Message}");
        }
        finally
        {
            _currentCancellationTokenSource.Dispose();
            _currentCancellationTokenSource = null;

            IsDemoRunning = false;
            RefreshDemoButtons();
        }
    }

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        _currentCancellationTokenSource?.Cancel();
    }

    private bool CanCancel()
    {
        return IsDemoRunning;
    }

    [RelayCommand]
    private void Clear()
    {
        _logger.Clear();
    }

    private void OnOutputChanged(object? sender, EventArgs e)
    {
        OutputText = _logger.OutputText;
    }

    private void RefreshDemoButtons()
    {
        foreach (var button in DemoButtons)
        {
            if (button.Command is IRelayCommand relayCommand)
            {
                relayCommand.NotifyCanExecuteChanged();
            }

            if (button.Command is IAsyncRelayCommand asyncRelayCommand)
            {
                asyncRelayCommand.NotifyCanExecuteChanged();
            }
        }
    }
}