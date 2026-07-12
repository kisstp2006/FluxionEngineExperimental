using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluxionEditor.Foundation.Themes;
using FluxionEditor.ViewModels;
using FluxionEditor.Views;
using System;
using System.IO;

namespace FluxionEditor
{
    /// <summary>
    /// Avalonia application root. Bootstraps the main window and ViewModel.
    /// </summary>
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Resolves the engine path, then creates the <see cref="MainWindow"/>.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            // Apply the saved startup theme before any window is created.
            ThemeManager.Instance.Initialize();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var enginePath = Environment.GetEnvironmentVariable("FLUXION_ENGINE", EnvironmentVariableTarget.User);

                if (enginePath != null && Directory.Exists(Path.Combine(enginePath, @"Engine\EngineAPI")))
                {
                    MainWindow.FluxionPath = enginePath;

                    // DataContext is set to the opened Project by MainWindow
                    // after the project manager dialog closes.
                    desktop.MainWindow = new MainWindow();
                }
                else
                {
                    // No valid engine path yet: the path dialog runs alone —
                    // the main window (and the project manager it opens) is
                    // only created once a valid path was confirmed. Explicit
                    // shutdown keeps the app alive in the gap between the
                    // dialog closing and the main window appearing.
                    desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                    var dlg = new EnginePathSelectDialog();
                    dlg.Closed += (_, _) =>
                    {
                        if (string.IsNullOrEmpty(dlg.FluxionPath))
                        {
                            // User cancelled — exit the app
                            desktop.Shutdown();
                            return;
                        }

                        MainWindow.FluxionPath = dlg.FluxionPath;
                        Environment.SetEnvironmentVariable("FLUXION_ENGINE",
                            dlg.FluxionPath.ToUpper(), EnvironmentVariableTarget.User);

                        var mainWindow = new MainWindow();
                        desktop.MainWindow = mainWindow;
                        desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                        mainWindow.Show();
                    };
                    dlg.Show();
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}