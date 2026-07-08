using System.Numerics;
using System.Runtime.Serialization;

namespace FluxionEditor.Foundation.Components
{
    /// <summary>
    /// Transform component attached to every <see cref="GameObject"/>.
    /// Stores position, rotation, and scale in 3D space.
    /// </summary>
    [DataContract]
    public class Transform : Component
    {
        // ── Constructors ────────────────────────────────────────────

        /// <summary>Parameterless constructor required by DataContractSerializer.</summary>
        public Transform()
        {
        }

        /// <summary>Creates a Transform attached to the given <paramref name="owner"/>.</summary>
        public Transform(GameObject owner) : base(owner)
        {
        }

        // ── Position ────────────────────────────────────────────────

        private Vector3 _position;

        [DataMember]
        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        // ── Rotation ────────────────────────────────────────────────

        private Vector3 _rotation;

        [DataMember]
        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }

        // ── Scale ───────────────────────────────────────────────────

        private Vector3 _scale;

        [DataMember]
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }
        }
    }
}
