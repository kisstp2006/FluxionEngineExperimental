using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace FluxionEditor.Foundation.Components
{
    /// <summary>
    /// An entity in a <see cref="Scene"/>. Can hold multiple <see cref="Component"/> instances.
    /// </summary>
    [DataContract]
    public class GameObject : ViewModelBase
    {
        // ── Identity ──

        private string _name = string.Empty;

        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        // ── Parent scene ──

        [DataMember]
        public Scene ParentScene { get; private set; } = null!;

        // ── Components ──

        [DataMember(Name = nameof(Components))]
        private ObservableCollection<Component> _components = new ObservableCollection<Component>();

        public ReadOnlyObservableCollection<Component> Components { get; private set; } = null!;

        // ── Constructors ──

        /// <summary>Parameterless constructor required by DataContractSerializer.</summary>
        private GameObject()
        {
        }

        public GameObject(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            Components = new ReadOnlyObservableCollection<Component>(_components);
        }

        // ── Deserialization ──

        /// <summary>Re-wraps the component collection after loading.</summary>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Ensure the backing collection is never null (field initializer may not run during deserialization)
            _components ??= new ObservableCollection<Component>();
            Components = new ReadOnlyObservableCollection<Component>(_components);
        }
    }
}
