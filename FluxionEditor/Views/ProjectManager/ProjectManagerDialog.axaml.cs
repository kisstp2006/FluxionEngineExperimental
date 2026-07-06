using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace FluxionEditor;

/// <summary>
/// Dialog that lets the user choose between opening an existing project
/// or creating a new one. Uses a sliding panel for the two views.
/// </summary>
public partial class ProjectManagerDialog : Window
{
    public ProjectManagerDialog()
    {
        InitializeComponent();

        // Default to the "Open Project" view
        OpenProjectBtn.IsChecked = true;
        CreateProjectBtn.IsChecked = false;
        ManagerContent.RenderTransform = new TranslateTransform(0, 0);
    }

    /// <summary>
    /// Switches between the "Open Project" and "Create Project" panels
    /// by sliding the content horizontally.
    /// </summary>
    private void ToggleView(object? sender, RoutedEventArgs e)
    {
        switch (sender)
        {
            case Button btn when btn == OpenProjectBtn:
                OpenProjectBtn.IsChecked = true;
                CreateProjectBtn.IsChecked = false;
                ManagerContent.RenderTransform = new TranslateTransform(0, 0);
                break;

            case Button btn when btn == CreateProjectBtn:
                OpenProjectBtn.IsChecked = false;
                CreateProjectBtn.IsChecked = true;
                ManagerContent.RenderTransform = new TranslateTransform(-800, 0);
                break;
        }
    }
}