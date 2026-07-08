using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using FluxionEditor.Foundation;
using FluxionEditor.Foundation.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FluxionEditor;

/// <summary>
/// Dialog that lets the user choose between opening an existing project
/// or creating a new one. Uses a sliding panel for the two views.
/// </summary>
public partial class ProjectManagerDialog : Window
{
    /// <summary>Width of one page inside the sliding strip.</summary>
    private const double PageWidth = 800;

    private static readonly TimeSpan SlideDuration = TimeSpan.FromMilliseconds(300);

    public ProjectManagerDialog()
    {
        InitializeComponent();

        Loaded += OnLoaded;

        // Default to the "Open Project" view
        OpenProjectBtn.IsChecked = true;
        CreateProjectBtn.IsChecked = false;
        ManagerContent.RenderTransform = new TranslateTransform(0, 0);

        // The create view starts off-screen and fully faded out;
        // it fades in as it slides in.
        createProjectView.Opacity = 0.0;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        if (!OpenProject.Projects.Any())
        {
            ToggleView(CreateProjectBtn, new RoutedEventArgs());
        }
    }

    /// <summary>
    /// Switches between the "Open Project" and "Create Project" panels:
    /// slides the content strip horizontally while cross-fading the
    /// outgoing and incoming views.
    /// </summary>
    private async void ToggleView(object? sender, RoutedEventArgs e)
    {
        var toCreate = ReferenceEquals(sender, CreateProjectBtn);
        OpenProjectBtn.IsChecked = !toCreate;
        CreateProjectBtn.IsChecked = toCreate;

        var easing = new CubicEaseInOut();
        var incoming = toCreate ? (Control)createProjectView : openProjectView;
        var outgoing = toCreate ? (Control)openProjectView : createProjectView;

        // Choreography: the outgoing page must vanish EARLY (while still
        // mostly on screen), then the incoming one fades up as it arrives —
        // otherwise the fade finishes only after the page already slid out
        // of view and the whole effect reads as a plain slide.
        var fadeOutDuration = TimeSpan.FromMilliseconds(110);
        var fadeInDelay     = TimeSpan.FromMilliseconds(90);
        var fadeInDuration  = SlideDuration - fadeInDelay;

        await Task.WhenAll(
            Animator.SlideToAsync(ManagerContent, toCreate ? -PageWidth : 0, SlideDuration, easing),
            Animator.FadeToAsync(outgoing, 0.0, fadeOutDuration, new CubicEaseOut()),
            Animator.FadeToAsync(incoming, 1.0, fadeInDuration, new CubicEaseOut(), fadeInDelay));
    }
}
