using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace FluxionEditor;

public partial class ProjectManagerDialog : Window
{
    public ProjectManagerDialog()
    {
        InitializeComponent();

        // Set the default selected view to "Open Project".
        openProjectBtn.IsChecked = true;
        createProjectBtn.IsChecked = false;

        // Start with the first panel visible.
        managerContent.RenderTransform = new TranslateTransform(0, 0);
    }

    private void ToggleView(object? sender, RoutedEventArgs e)
    {
        switch (sender)
        {
            case Button btn when btn == openProjectBtn:
                // Select the Open Project tab and deselect the Create Project tab.
                openProjectBtn.IsChecked = true;
                createProjectBtn.IsChecked = false;

                // Move the content back to the first panel.
                managerContent.RenderTransform = new TranslateTransform(0, 0);
                break;

            case Button btn when btn == createProjectBtn:
                // Select the Create Project tab and deselect the Open Project tab.
                openProjectBtn.IsChecked = false;
                createProjectBtn.IsChecked = true;

                // Move the content to the second panel.
                // The previous issue was here: this movement should not depend
                // on the previous checked state of the other button.
                managerContent.RenderTransform = new TranslateTransform(-800, 0);
                break;

            default:
                // Ignore clicks from unknown controls.
                break;
        }
    }
}