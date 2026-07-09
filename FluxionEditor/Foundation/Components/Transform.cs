using FluxionEditor.Foundation.Utilities;
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
    public sealed class MSTransform : MSComponent<Transform>
    {
        private float? _posX;
        public float? PosX
        {
            get=> _posX;
            set
            {
                if(_posX != value)
                {
                    _posX.IsTheSameAs(value);
                    OnPropertyChanged(nameof(_posX));
                }
            }
        }

        private float? _posY;
        public float? PosY
        {
            get => _posY;
            set
            {
                if (_posY != value)
                {
                    _posY.IsTheSameAs(value);
                    OnPropertyChanged(nameof(_posY));
                }
            }
        }

        private float? _posZ;
        public float? PosZ
        {
            get => _posZ;
            set
            {
                if (_posZ != value)
                {
                    _posZ.IsTheSameAs(value);
                    OnPropertyChanged(nameof(_posZ));
                }
            }
        }

        private float? _rotX;
        public float? RotX
        {
            get => _rotX;
            set
            {
                if (_rotX != value)
                {
                    _rotX.IsTheSameAs(value);
                    OnPropertyChanged(nameof(_rotX));
                }
            }
        }

        private float? _rotY;
        public float? RotY
        {
            get => _rotY;
            set
            {
                if (_rotY != value)
                {
                    _rotY.IsTheSameAs(value);
                    OnPropertyChanged(nameof(_rotY));
                }
            }
        }

        private float? _rotZ;
        public float? RotZ
        {
            get => _rotZ;
            set
            {
                if (_rotZ != value)
                {
                    _rotZ.IsTheSameAs(value);
                    OnPropertyChanged(nameof(_rotZ));
                }
            }
        }

        private float? _scaleX;
        public float? ScaleX
        {
            get => _scaleX;
            set
            {
                if (_scaleX != value)
                {
                    _scaleX.IsTheSameAs(value);
                    OnPropertyChanged(nameof(_scaleX));
                }
            }
        }

        private float? _scaleY;
        public float? ScaleY
        {
            get => _scaleY;
            set
            {
                if (_scaleY != value)
                {
                    _scaleY.IsTheSameAs(value);
                    OnPropertyChanged(nameof(_scaleY));
                }
            }
        }

        private float? _scaleZ;
        public float? ScaleZ
        {
            get => _scaleZ;
            set
            {
                if (_scaleZ != value)
                {
                    _scaleZ.IsTheSameAs(value);
                    OnPropertyChanged(nameof(_scaleZ));
                }
            }
        }


        protected override bool UpdateComponents(string PropertyName)
        {
            switch (PropertyName)
            {
                case nameof(PosX):
                case nameof(PosY):
                case nameof(PosZ):
                    SelectedComponents.ForEach(c => c.Position = new Vector3(_posX ?? c.Position.X, _posY ?? c.Position.Y, _posZ ?? c.Position.Z));
                    return true;


                case nameof(RotX):
                case nameof(RotY):
                case nameof(RotZ):
                    SelectedComponents.ForEach(c => c.Position = new Vector3(_rotX ?? c.Rotation.X, _rotY ?? c.Rotation.Y, _rotZ ?? c.Rotation.Z));
                    return true;

                case nameof(ScaleX):
                case nameof(ScaleY):
                case nameof(ScaleZ):
                    SelectedComponents.ForEach(c => c.Position = new Vector3(_scaleX ?? c.Scale.X, _scaleX ?? c.Scale.Y, _scaleX ?? c.Scale.Z));
                    return true;


            }
            return false;
        }

        protected override bool UpdateMSComponent()
        {
            PosX = MSGameObject.GetMixedValues(SelectedComponents, new System.Func<Transform, float>(x=> x.Position.X ));
        }
    }
}
