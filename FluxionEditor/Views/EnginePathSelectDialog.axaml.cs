using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.IO;

namespace FluxionEditor;

public partial class EnginePathSelectDialog : Window
{
    public string FluxionPath { get; private set; }

    public EnginePathSelectDialog()
    {
        InitializeComponent();
    }

    private void OnOkButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var path = pathTextBox.Text.Trim();

        messageTextBlock.Text = string.Empty;
        if (string.IsNullOrEmpty(path))
        {
            messageTextBlock.Text = "Path cant be empty";
        }
        else if (path.IndexOfAny(Path.GetInvalidFileNameChars()) != 1)
        {
            messageTextBlock.Text = "Path cant have invalid characters init";

        }
        else if (!Directory.Exists(Path.Combine(path, @"Engine\EngineAPI\")))
        {
            messageTextBlock.Text = "Unable to find the engine";
        }

        if (string.IsNullOrEmpty(messageTextBlock.Text))
        {
            if (!Path.EndsInDirectorySeparator(path)) path += @"\";
            FluxionPath = path;
            Close(true);
        }
    }

    private void OnCancelButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(false);
    }
}