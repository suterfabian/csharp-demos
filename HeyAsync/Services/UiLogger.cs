using System.Windows.Controls;

namespace HeyAsync.Services;

public sealed class UiLogger
{
    private readonly TextBox _outputTextBox;

    public UiLogger(TextBox outputTextBox)
    {
        _outputTextBox = outputTextBox;
    }

    public void WriteLine(string text)
    {
        _outputTextBox.AppendText($"{DateTime.Now:HH:mm:ss.fff} | {text}{Environment.NewLine}");
        _outputTextBox.ScrollToEnd();
    }

    public void WriteHeader(string text)
    {
        // WriteLine("");
        AddEmptyLine();
        WriteLine("==================================================");
        WriteLine(text);
        WriteLine("==================================================");
    }

    public void AddEmptyLine()
    {
        _outputTextBox.AppendText(""); 
    }

    public void Clear()
    {
        _outputTextBox.Clear();
    }
}