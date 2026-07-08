using Avalonia.Controls;
using FluxionEditor.Foundation;

namespace FluxionEditor;

/// <summary>
/// View for browsing and opening existing projects.
/// </summary>
public partial class OpenProjectView : UserControl
{
    public OpenProjectView()
    {
        InitializeComponent();
    }

    private void OnProjectDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        OpenSelectedProject();
    }

    private void OnOpenButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        OpenSelectedProject();
    }

    /// <summary>
    /// Opens the currently selected project and returns it to the parent window.
    /// </summary>
    private void OpenSelectedProject()
    {
        var project = OpenProject.Open(ProjectsListBox.SelectedItem as ProjectData);
        (TopLevel.GetTopLevel(this) as Window)?.Close(project);
    }
}