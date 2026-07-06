using Avalonia.Controls;
using FluxionEditor.Foundation;

namespace FluxionEditor;

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

    private void Open_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        OpenSelectedProject();
    }

    private void OpenSelectedProject()
    {
        var project = OpenProject.Open(projectsListBox.SelectedItem as ProjectData);
        var window = TopLevel.GetTopLevel(this) as Window;
        window?.Close(project);
    }
}