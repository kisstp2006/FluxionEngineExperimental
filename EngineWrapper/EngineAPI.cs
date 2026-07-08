using EngineWrapper.EngineAPIStructs;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace EngineWrapper.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class Transform
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = Vector3.One;

    }


    [StructLayout(LayoutKind.Sequential)]
    internal class GameEntityDescriptor
    {
        public Transform transform= new Transform();
    }
}

namespace EngineWrapper
{
    static class EngineAPI
    {
        private const string _dllName= "EngineExport.dll";

        [DllImport(_dllName)]
        private static extern int CreateGameObject(GameEntityDescriptor desc);
        public static int CreateGameObject(GameObject gameObject)
        {
            GameEntityDescriptor desc = new GameEntityDescriptor();

            {
                var c = gameObject.GetComponent<GameEntityDescriptor>();
            }
        }
    }

    
}
