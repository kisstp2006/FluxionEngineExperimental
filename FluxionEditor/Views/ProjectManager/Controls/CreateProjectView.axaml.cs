using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluxionEditor.Foundation;

namespace FluxionEditor;

public partial class CreateProjectView : UserControl
{
    public CreateProjectView()
    {
        InitializeComponent();
    }

    private void Create_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        var vm = this.DataContext as NewProject;
        var projectPath = vm?.CreateProject(template_list.SelectedItem as ProjectTemplate);

        bool dialogResult = false;
        if (!string.IsNullOrEmpty(projectPath))
        {
            dialogResult = true;
            var project = OpenProject.Open(new ProjectData { Name = vm.ProjectName, Path = projectPath });
            window.DataContext = project;
        } 


        // Close the parent dialog and return true = project created
        window?.Close(true);
    }

    private void Exit_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Close the parent dialog and return false = cancelled
        if (this.VisualRoot is Window window)
            window.Close(false);
    }
}