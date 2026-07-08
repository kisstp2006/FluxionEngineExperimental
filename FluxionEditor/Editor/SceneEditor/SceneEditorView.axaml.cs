using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FluxionEditor.Editor;

public partial class SceneEditorView : UserControl
{
    public SceneEditorView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>Auto-focus the editor on load so key bindings work immediately.</summary>
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Focus();
    }
}