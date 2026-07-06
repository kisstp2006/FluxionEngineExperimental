using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
        var window = this.VisualRoot as Window;

        var project = OpenProject.Open(projectsListBox.SelectedItem as ProjectData);

        bool dialogResult = false;
        if (project != null)
        {
            dialogResult = true;
            window.DataContext = project;
        }


        // Close the parent dialog and return true = project opened
        window?.Close(true);
    }
}