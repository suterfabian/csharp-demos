using System.Windows.Input;

namespace HeyAsync.ViewModels;

public sealed class AsyncDemoButtonViewModel
{
    public string Title { get; }
    public ICommand Command { get; }

    public AsyncDemoButtonViewModel(string title, ICommand command)
    {
        Title = title;
        Command = command;
    }
}