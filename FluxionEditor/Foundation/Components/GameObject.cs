using FluxionEditor.Foundation.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace FluxionEditor.Foundation.Components
{
    /// <summary>
    /// An entity in a <see cref="Scene"/>. Can hold multiple <see cref="Component"/> instances.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Transform))] // Include new component types here for serialization
    public class GameObject : ViewModelBase
    {
        // ── Identity ──

        private bool _isEnabled = true;

        [DataMember]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

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

        // ── Commands ──

        public ICommand RenameCommand { get; private set; }
        public ICommand CancelEditCommand { get; private set; }
        public ICommand IsEnabledCommand { get; private set; }

        // ── Constructors ──

        /// <summary>Parameterless constructor required by DataContractSerializer.</summary>
        private GameObject()
        {
        }

        public GameObject(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
        }

        // ── Deserialization ──

        /// <summary>Re-wraps the component collection after loading.</summary>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Components = new ReadOnlyObservableCollection<Component>(_components);
            OnPropertyChanged(nameof(Components));

            RenameCommand = new RelayCommand<string>(x =>
            {
                var oldName = Name;
                Name = x;
                ParentScene.Project?.UndoRedo.Add(new UndoRedoCommand(
                    $"Rename {oldName} to {x}",
                    nameof(Name), this, oldName, x));
            }, x => x != _name);

            // Escape simply resets the TextBox by forcing the OneWay binding to refresh
            CancelEditCommand = new RelayCommand(() =>
                OnPropertyChanged(nameof(Name)));

            IsEnabledCommand = new RelayCommand<bool>(x =>
            {
                var oldValue = IsEnabled;
                IsEnabled = x;
                ParentScene.Project?.UndoRedo.Add(new UndoRedoCommand(
                    $"{(x ? "Enable" : "Disable")} {Name}",
                    nameof(IsEnabled), this, oldValue, x));
            });


            // Escape simply resets the TextBox by forcing the OneWay binding to refresh
            CancelEditCommand = new RelayCommand(() =>
                OnPropertyChanged(nameof(Name)));
        }
    }
}
