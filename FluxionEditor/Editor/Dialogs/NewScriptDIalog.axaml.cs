using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using FluxionEditor.Foundation.Utilities;
using System.IO;

namespace FluxionEditor;

public partial class NewScriptDIalog : Window
{
    /// <summary>The script name entered by the user (without extension).</summary>
    public string ScriptName { get; private set; } = string.Empty;

    /// <summary>The folder path where the script should be created.</summary>
    public string ScriptPath { get; private set; } = string.Empty;

    public NewScriptDIalog()
    {
        InitializeComponent();
    }

    // ── Live (keystroke) validation ──────────────────────────

    /// <summary>
    /// Validates only the name field on every keystroke.
    /// Lightweight — no disk I/O, no path check.
    /// </summary>
    private void OnScriptNameTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        var name = scriptNameTextBox.Text?.Trim() ?? string.Empty;
        if (name.Length == 0)
        {
            messageTextBlock.Text = string.Empty;
            return;
        }

        var result = ScriptValidator.ValidateName(name);
        messageTextBlock.Text = result.IsValid ? string.Empty : result.Message;
    }

    /// <summary>
    /// Validates only the path field on every keystroke.
    /// Does NOT check disk existence (too expensive per keystroke),
    /// only checks format validity.
    /// </summary>
    private void OnScriptPathTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        var path = scriptPathTextBox.Text?.Trim() ?? string.Empty;
        if (path.Length == 0)
        {
            // Don't clear a name-related error message
            if (string.IsNullOrEmpty(scriptNameTextBox.Text?.Trim()))
                messageTextBlock.Text = string.Empty;
            return;
        }

        // Lightweight: check format and rooted-ness, skip existence check
        var result = ScriptValidator.ValidatePath(path, checkExistence: false);

        // Only overwrite if the error is path-specific (don't hide a name error)
        if (!result.IsValid)
            messageTextBlock.Text = result.Message;
        else
            messageTextBlock.Text = string.Empty;
    }

    // ── Browse for script folder ──────────────────────────────

    private async void OnBrowseButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select folder for the new script",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            scriptPathTextBox.Text = folders[0].Path.LocalPath;
            messageTextBlock.Text = string.Empty;
        }
    }

    // ── OK ────────────────────────────────────────────────────

    private void OnOkButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var name = scriptNameTextBox.Text?.Trim() ?? string.Empty;
        var path = scriptPathTextBox.Text?.Trim() ?? string.Empty;

        // Full validation with disk checks
        var result = ScriptValidator.Validate(name, path, checkExistence: true);

        if (!result.IsValid)
        {
            messageTextBlock.Text = result.Message;
            return;
        }

        ScriptName = name;
        ScriptPath = path;
        Close(true);
    }

    // ── Cancel ────────────────────────────────────────────────

    private void OnCancelButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(false);
    }
}