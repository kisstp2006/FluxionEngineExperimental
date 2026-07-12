using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using FluxionEditor.Foundation;

namespace FluxionEditor.Views
{
    /// <summary>
    /// Main application window. Handles project open/close flow and
    /// synchronizes <see cref="Application.Current"/> DataContext.
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isProjectManagerDialogOpen;

        /// <summary>Engine install root. Resolved by <see cref="App"/> before this window is created.</summary>
        public static string FluxionPath { get; set; } = "C:\\Users\\tigames\\Documents\\FluxionExperimental\\FluxionEngine";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;
        }

        // ── Lifecycle ──────────────────────────────────────────

        private void OnMainWindowLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded; // Run once
            OpenProjectManagerDialog();
        }

        private void OnMainWindowClosing(object? sender, WindowClosingEventArgs e)
        {
            Closing -= OnMainWindowClosing;
            Project.Current?.Unload();
        }

        // ── Project open flow ───────────────────────────────────

        /// <summary>
        /// Shows the project manager dialog. If the user cancels,
        /// the application shuts down.
        /// </summary>
        private async void OpenProjectManagerDialog()
        {
            if (_isProjectManagerDialogOpen)
                return;

            _isProjectManagerDialogOpen = true;

            try
            {
                var dialog = new ProjectManagerDialog();
                var project = await dialog.ShowDialog<Project?>(this);

                if (project == null)
                {
                    // User cancelled — exit the app
                    if (Application.Current?.ApplicationLifetime
                        is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.Shutdown();
                    }
                }
                else
                {
                    Project.Current?.Unload();
                    DataContext = project;
                    Application.Current!.DataContext = project;
                }
            }
            finally
            {
                _isProjectManagerDialogOpen = false;
            }
        }
    }
}