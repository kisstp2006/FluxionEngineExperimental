using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluxionEditor.Foundation.Utilities;
using FluxionEditor.ViewModels;
using FluxionEditor.Views;

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
        /// Creates the <see cref="MainWindow"/> and assigns its initial DataContext.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            // Apply the saved startup theme before any window is created.
            ThemeManager.Instance.Initialize();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // DataContext is set to the opened Project by MainWindow
                // after the project manager dialog closes.
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}