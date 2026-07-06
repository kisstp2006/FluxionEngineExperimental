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

    private void Open_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        OpenSelectedProject();
    }

    private void OpenSelectedProject()
    {
        
        var project = OpenProject.Open(projectsListBox.SelectedItem as ProjectData);

        bool dialogResult = false;
        if (project != null)
        {
            dialogResult = true;
        }


        // Close the parent dialog and return true = project created
        if (this.VisualRoot is Window window)
            window.Close(true);
    }
}