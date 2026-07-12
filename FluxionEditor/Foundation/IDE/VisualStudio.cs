#if WINDOWS
using FluxionEditor.Foundation.Utilities;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace FluxionEditor.Foundation.IDE
{
    /// <summary>
    /// Windows-only wrapper around the Visual Studio COM automation API
    /// (EnvDTE). The whole file is compiled out on other platforms — callers
    /// must guard usage with #if WINDOWS.
    /// </summary>
    internal static class VisualStudio
    {
        // Supported Visual Studio versions, newest preferred:
        //   18.0 = Visual Studio 2026, 17.0 = Visual Studio 2022.
        // The version-independent ProgID is kept as a last resort so a
        // future VS still works before it is added here explicitly.
        private static readonly string[] ProgIds =
        {
            "VisualStudio.DTE.18.0", // Visual Studio 2026
            "VisualStudio.DTE.17.0", // Visual Studio 2022
            "VisualStudio.DTE",      // newest registered version
        };

        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        /// <summary>
        /// Opens the solution in Visual Studio: reuses a running instance
        /// that already has it open, otherwise starts a new one.
        /// </summary>
        /// <returns>false if Visual Studio is unavailable — the caller
        /// should fall back to another way of opening the solution.</returns>
        public static bool Open(string solution)
        {
            try
            {
                var dte = FindRunningInstance(solution);

                if (dte == null)
                {
                    var dteType = FindInstalledVersion();
                    if (dteType == null) return false; // no Visual Studio installed

                    dte = Activator.CreateInstance(dteType) as EnvDTE80.DTE2;
                    dte?.Solution.Open(solution);
                }

                if (dte == null) return false;

                dte.MainWindow.Visible = true;
                dte.MainWindow.Activate();
                dte.UserControl = true; // keep VS alive after we release the COM object
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(SeverityLevel.Error, $"Unable to open Visual Studio: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Returns the DTE type of the newest supported Visual Studio
        /// installed on this machine, or null if there is none.
        /// </summary>
        private static Type? FindInstalledVersion()
        {
            foreach (var progId in ProgIds)
            {
                var type = Type.GetTypeFromProgID(progId, throwOnError: false);
                if (type != null) return type;
            }
            return null;
        }

        /// <summary>
        /// Scans the COM Running Object Table for a Visual Studio instance
        /// that already has this solution open.
        /// </summary>
        private static EnvDTE80.DTE2? FindRunningInstance(string solution)
        {
            IRunningObjectTable? rot = null;
            IEnumMoniker? monikers = null;
            IBindCtx? bindCtx = null;

            try
            {
                if (GetRunningObjectTable(0, out rot) != 0 || CreateBindCtx(0, out bindCtx) != 0)
                    return null;

                rot.EnumRunning(out monikers);
                monikers.Reset();
                var moniker = new IMoniker[1];

                while (monikers.Next(1, moniker, IntPtr.Zero) == 0)
                {
                    try
                    {
                        moniker[0].GetDisplayName(bindCtx, null, out var displayName);
                        if (!displayName.StartsWith("!VisualStudio.DTE")) continue;

                        if (rot.GetObject(moniker[0], out var obj) != 0) continue;
                        if (obj is not EnvDTE80.DTE2 dte) continue;

                        // The instance may still be initializing and reject
                        // COM calls — skip it in that case.
                        try
                        {
                            if (string.Equals(dte.Solution.FullName, solution,
                                    StringComparison.OrdinalIgnoreCase))
                                return dte;
                        }
                        catch (COMException) { }
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(moniker[0]);
                    }
                }
            }
            finally
            {
                if (monikers != null) Marshal.ReleaseComObject(monikers);
                if (bindCtx != null) Marshal.ReleaseComObject(bindCtx);
                if (rot != null) Marshal.ReleaseComObject(rot);
            }

            return null;
        }
    }
}
#endif
