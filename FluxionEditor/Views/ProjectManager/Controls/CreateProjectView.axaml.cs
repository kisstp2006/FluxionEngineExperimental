using Avalonia.Controls;
using FluxionEditor.Foundation;

namespace FluxionEditor;

/// <summary>
/// View for creating a new project. Validates the project path and
/// returns the newly created <see cref="Project"/> to the parent window.
/// </summary>
public partial class CreateProjectView : UserControl
{
    public CreateProjectView()
    {
        InitializeComponent();
    }

    private void CreateButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = DataContext as NewProject;
        var projectPath = vm?.CreateProject(TemplateList.SelectedItem as ProjectTemplate);

        Project? project = null;
        if (!string.IsNullOrEmpty(projectPath))
        {
            project = OpenProject.Open(new ProjectData
            {
                Name = vm!.ProjectName,
                Path = projectPath
            });
        }

        (TopLevel.GetTopLevel(this) as Window)?.Close(project);
    }

    private void ExitButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        (TopLevel.GetTopLevel(this) as Window)?.Close(null);
    }
}