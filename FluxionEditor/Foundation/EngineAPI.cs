using EngineWrapper;
using FluxionEditor.Foundation.Components;
using System.Diagnostics;

namespace FluxionEditor.Foundation
{
    /// <summary>
    /// Editor-side adapter over <see cref="EngineWrapper.EngineAPI"/>.
    /// Converts editor components into plain wrapper descriptors, so the
    /// wrapper never needs to reference editor types (which would create
    /// a circular project reference).
    /// </summary>
    internal static class EngineAPI
    {
        /// <summary>
        /// Creates the engine-side counterpart of an editor game object
        /// and returns its engine id.
        /// </summary>
        public static int CreateGameObject(GameObject gameObject)
        {
            Debug.Assert(gameObject != null);
            var descriptor = new GameObjectDescriptor();

            // Transform component
            {
                var transform = gameObject.GetComponent<Transform>();
                Debug.Assert(transform != null, "Every game object must have a Transform");

                descriptor.Transform.Position = transform.Position;
                descriptor.Transform.Rotation = transform.Rotation;
                descriptor.Transform.Scale = transform.Scale;
            }

            return EngineWrapper.EngineAPI.CreateGameObject(descriptor);
        }

        /// <summary>Removes the engine-side counterpart of a game object.</summary>
        public static void RemoveGameObject(GameObject gameObject)
        {
            Debug.Assert(gameObject != null);
            EngineWrapper.EngineAPI.RemoveGameObject(gameObject.GameObjectId);
        }
    }
}
