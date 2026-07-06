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
        OpenProjectBtn.IsChecked = true;
        CreateProjectBtn.IsChecked = false;

        // Start with the first panel visible.
        ManagerContent.RenderTransform = new TranslateTransform(0, 0);
    }

    private void ToggleView(object? sender, RoutedEventArgs e)
    {
        switch (sender)
        {
            case Button btn when btn == OpenProjectBtn:
                // Select the Open Project tab and deselect the Create Project tab.
                OpenProjectBtn.IsChecked = true;
                CreateProjectBtn.IsChecked = false;

                // Move the content back to the first panel.
                ManagerContent.RenderTransform = new TranslateTransform(0, 0);
                break;

            case Button btn when btn == CreateProjectBtn:
                // Select the Create Project tab and deselect the Open Project tab.
                OpenProjectBtn.IsChecked = false;
                CreateProjectBtn.IsChecked = true;

                // Move the content to the second panel.
                // The previous issue was here: this movement should not depend
                // on the previous checked state of the other button.
                ManagerContent.RenderTransform = new TranslateTransform(-800, 0);
                break;

            default:
                // Ignore clicks from unknown controls.
                break;
        }
    }
}