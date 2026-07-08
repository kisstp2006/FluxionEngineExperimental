using System.Numerics;
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
    /// Thin P/Invoke layer over EngineExport.dll. Knows nothing about
    /// editor types — callers convert their data into descriptors.
    /// </summary>
    public static class EngineAPI
    {
        private const string _dllName = "EngineExport.dll";

        /// <summary>Creates a game object in the engine and returns its id.</summary>
        [DllImport(_dllName)]
        public static extern int CreateGameObject(GameObjectDescriptor desc);

        /// <summary>Removes the game object with the given engine id.</summary>
        [DllImport(_dllName)]
        public static extern void RemoveGameObject(int id);
    }
}
