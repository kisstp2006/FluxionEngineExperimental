using Avalonia.Controls;
using FluxionEditor.Foundation;

namespace FluxionEditor;

public partial class CreateProjectView : UserControl
{
    public CreateProjectView()
    {
        InitializeComponent();
    }

    private void CreateButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = this.DataContext as NewProject;
        var projectPath = vm?.CreateProject(TemplateList.SelectedItem as ProjectTemplate);

        Project? project = null;
        if (!string.IsNullOrEmpty(projectPath))
        {
            project = OpenProject.Open(new ProjectData { Name = vm!.ProjectName, Path = projectPath });
        }

        var window = TopLevel.GetTopLevel(this) as Window;
        window?.Close(project);
    }

    private void ExitButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        window?.Close(null);
    }
}