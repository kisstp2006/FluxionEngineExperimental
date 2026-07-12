using System;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EngineWrapper
{
    // ── Descriptors ─────────────────────────────────────────────────
    // Plain-data mirrors of the C++ structs in EngineExport/EngineAPI.cpp.
    // Layout must match the native side field for field (3 x Vector3 = 9 floats).

    [StructLayout(LayoutKind.Sequential)]
    public class TransformDescriptor
    {
        public Vector3 Position;
        public Vector3 Rotation; // Euler angles in radians
        public Vector3 Scale = Vector3.One;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class GameObjectDescriptor
    {
        public TransformDescriptor Transform = new TransformDescriptor();
    }

    // ── Native entry points ─────────────────────────────────────────

    /// <summary>
    /// Thin P/Invoke layer over the native EngineExport library. Knows nothing
    /// about editor types — callers convert their data into descriptors.
    /// </summary>
    public static class EngineAPI
    {
        // Logical (extension-less) name. A per-platform resolver below maps it
        // to the actual file: EngineExport.dll / libEngineExport.so / .dylib.
        private const string _libraryName = "EngineExport";

        static EngineAPI()
        {
            NativeLibrary.SetDllImportResolver(typeof(EngineAPI).Assembly, Resolve);
        }

        /// <summary>Maps the logical library name to the OS-specific file name.</summary>
        private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            // Only handle our library; anything else falls back to the default loader.
            if (libraryName != _libraryName)
                return IntPtr.Zero;

            string fileName;
            if (OperatingSystem.IsWindows())
                fileName = "EngineExport.dll";
            else if (OperatingSystem.IsMacOS())
                fileName = "libEngineExport.dylib";
            else
                fileName = "libEngineExport.so"; // Linux and other Unix-likes

            return NativeLibrary.Load(fileName, assembly, searchPath);
        }

        /// <summary>Creates a game object in the engine and returns its id.</summary>
        [DllImport(_libraryName)]
        public static extern int CreateGameObject(GameObjectDescriptor desc);

        /// <summary>Removes the game object with the given engine id.</summary>
        [DllImport(_libraryName)]
        public static extern void RemoveGameObject(int id);
    }
}
