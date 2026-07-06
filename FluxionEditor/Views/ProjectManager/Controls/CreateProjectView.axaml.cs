using Avalonia.Controls;
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
        var vm = this.DataContext as NewProject;
        var projectPath = vm?.CreateProject(template_list.SelectedItem as ProjectTemplate);

        Project? project = null;
        if (!string.IsNullOrEmpty(projectPath))
        {
            project = OpenProject.Open(new ProjectData { Name = vm!.ProjectName, Path = projectPath });
        }

        var window = TopLevel.GetTopLevel(this) as Window;
        window?.Close(project);
    }

    private void Exit_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        window?.Close(null);
    }
}