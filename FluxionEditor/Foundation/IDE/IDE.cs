using FluxionEditor.Foundation.Utilities;
using System;
using System.Diagnostics;
using System.IO;

namespace FluxionEditor.Foundation.IDE
{
    /// <summary>
    /// Opens the game code solution in an IDE. On Windows this is delegated
    /// to <see cref="VisualStudio"/> (COM automation); on other platforms —
    /// or when Visual Studio is unavailable — the solution is handed to the
    /// OS default handler (Rider, VS Code, MonoDevelop — whatever is
    /// associated with .sln files).
    /// </summary>
    internal static class IDE
    {
        public static void OpenIDE(string solution)
        {
            if (string.IsNullOrEmpty(solution) || !File.Exists(solution))
            {
                Logger.Log(SeverityLevel.Error, $"Unable to find the solution: {solution}");
                return;
            }

#if WINDOWS
            if (VisualStudio.Open(solution)) return;
#endif
            OpenWithDefaultHandler(solution);
        }

        /// <summary>
        /// Cross-platform fallback: lets the OS pick the application
        /// registered for solution files (xdg-open / open / ShellExecute).
        /// </summary>
        private static void OpenWithDefaultHandler(string solution)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = solution,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Logger.Log(SeverityLevel.Error, $"Unable to open the solution in an IDE: {ex.Message}");
            }
        }


        public static void CloseIDE(string solution)
        {
            if (string.IsNullOrEmpty(solution) || !File.Exists(solution))
            {
                Logger.Log(SeverityLevel.Error, $"Unable to find the solution: {solution}");
                return;
            }
#if WINDOWS
            if (VisualStudio.Close(solution)) return;
#endif
        }
    }
}
