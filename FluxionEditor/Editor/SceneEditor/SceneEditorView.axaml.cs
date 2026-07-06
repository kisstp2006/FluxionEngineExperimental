using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace FluxionEditor.Editor;

public partial class SceneEditorView : UserControl
{
    public SceneEditorView()
    {
        InitializeComponent();
        Loaded += OnSceneEditorViewLoaded;
    }

    private void OnSceneEditorViewLoaded(object? sender, RoutedEventArgs e)
    {
        Loaded -= OnSceneEditorViewLoaded;
        Focus();
    }
}