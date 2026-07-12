using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using FluxionEditor.Views;
using System.IO;

namespace FluxionEditor;

public partial class EnginePathSelectDialog : Window
{
    public string FluxionPath { get; private set; }

    public EnginePathSelectDialog()
    {
        InitializeComponent();

    }

    private async void OnBrowseButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // StorageProvider is the cross-platform picker: it maps to the
        // native folder dialog on Windows, macOS and Linux.
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select the Fluxion Engine folder",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            pathTextBox.Text = folders[0].Path.LocalPath;
            messageTextBlock.Text = string.Empty;
        }
    }

    private void OnOkButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Text is null until the user types something (the watermark is not text)
        var path = pathTextBox.Text?.Trim() ?? string.Empty;

        messageTextBlock.Text = string.Empty;
        if (string.IsNullOrEmpty(path))
        {
            messageTextBlock.Text = "Path cant be empty";
        }
        else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
        {
            messageTextBlock.Text = "Path cant have invalid characters init";

        }
        else if (!Directory.Exists(Path.Combine(path, "Engine", "EngineAPI")))
        {
            messageTextBlock.Text = "Unable to find the engine";
        }

        if (string.IsNullOrEmpty(messageTextBlock.Text))
        {
            if (!Path.EndsInDirectorySeparator(path)) path += Path.DirectorySeparatorChar;
            FluxionPath = path;
            Close(true);
        }
    }

    private void OnCancelButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(false);
    }
}