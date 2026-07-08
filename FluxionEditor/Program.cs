using Avalonia;
using System;

namespace FluxionEditor
{
    internal sealed class Program
    {
        /// <summary>
        /// Application entry point. Do not use Avalonia APIs or
        /// SynchronizationContext before AppMain is called.
        /// </summary>
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        /// <summary>Avalonia configuration — also used by the visual designer.</summary>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
#if DEBUG
                .WithDeveloperTools()
#endif
                .WithInterFont()
                .LogToTrace();
    }
}
