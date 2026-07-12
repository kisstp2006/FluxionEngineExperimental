using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using FluxionEditor.Foundation;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace FluxionEditor.Views
{
    /// <summary>
    /// Main application window. Handles project open/close flow and
    /// synchronizes <see cref="Application.Current"/> DataContext.
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isProjectManagerDialogOpen;

        public static string FluxionPath { get; private set; } = "C:\\Users\\tigames\\Documents\\FluxionExperimental\\FluxionEngine";

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
            GetFluxionEnginePath();
            OpenProjectManagerDialog();
        }

        private async void GetFluxionEnginePath()
        {
            var enginePath = Environment.GetEnvironmentVariable("FLUXION_ENGINE",EnvironmentVariableTarget.User);

            if (enginePath == null || !Directory.Exists(Path.Combine(enginePath, @"Engine\EngineAPI"))) 
            {
                var dlg = new EnginePathSelectDialog();

                if (await dlg.ShowDialog<bool>(this) == true)
                {
                    FluxionPath = dlg.FluxionPath;
                    Environment.SetEnvironmentVariable("FLUXION_ENGINE", FluxionPath.ToUpper(),EnvironmentVariableTarget.User);
                }
                else
                {
                    // User cancelled — exit the app
                    if (Application.Current?.ApplicationLifetime
                        is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.Shutdown();
                    }
                }

            }
            else 
            {
                FluxionPath = enginePath;
            
            }
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