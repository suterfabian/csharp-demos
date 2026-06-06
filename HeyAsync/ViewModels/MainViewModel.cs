using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeyAsync.Demos;
using HeyAsync.Services;

namespace HeyAsync.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IUiLogger _logger;

    public ObservableCollection<AsyncDemoButtonViewModel> DemoButtons { get; }

    [ObservableProperty]
    private string _outputText = string.Empty;

    public MainViewModel(IEnumerable<IAsyncDemo> demos, IUiLogger logger)
    {
        _logger = logger;

        DemoButtons = new ObservableCollection<AsyncDemoButtonViewModel>(
            demos
                .OrderBy(demo => demo.Order)
                .Select(demo => new AsyncDemoButtonViewModel(
                    demo.Title,
                    new AsyncRelayCommand(demo.ExecuteAsync))));

        _logger.OutputChanged += OnOutputChanged;

        _logger.WriteLine("Async WPF Lab gestartet.");
        _logger.WriteLine($"UI Thread ID: {Environment.CurrentManagedThreadId}");
        _logger.WriteLine("");
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
}