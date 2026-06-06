using System.Text;

namespace HeyAsync.Services;

public sealed class UiLogger : IUiLogger
{
    private readonly StringBuilder _builder = new();

    public string OutputText => _builder.ToString();

    public event EventHandler? OutputChanged;

    public void WriteLine(string text)
    {
        _builder.AppendLine($"{DateTime.Now:HH:mm:ss.fff} | {text}");
        OutputChanged?.Invoke(this, EventArgs.Empty);
    }

    public void WriteHeader(string text)
    {
        WriteLine("");
        WriteLine("==================================================");
        WriteLine(text);
        WriteLine("==================================================");
    }

    public void Clear()
    {
        _builder.Clear();
        OutputChanged?.Invoke(this, EventArgs.Empty);
    }
}