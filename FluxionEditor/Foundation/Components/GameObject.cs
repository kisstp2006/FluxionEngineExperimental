using FluxionEditor.Foundation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        }
    }

    abstract class MSObject : ViewModelBase
    {
        // ── Identity ──
        private bool _enableUpdates = true;
        private bool? _isEnabled = true;

        [DataMember]
        public bool? IsEnabled
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

        private string? _name = string.Empty;

        [DataMember]
        public string? Name
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

        private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();
        public ReadOnlyObservableCollection<IMSComponent> Components { get; }

        public List<GameObject> selectedGameObjects { get; }

        // ── Commands ──

        public ICommand RenameCommand { get; private set; }
        public ICommand CancelEditCommand { get; private set; }
        public ICommand IsEnabledCommand { get; private set; }


        protected virtual bool UpdateGameObjects(string propertyName)
        {
            switch (propertyName) { 
                case nameof(IsEnabled): selectedGameObjects.ForEach(x =>x.IsEnabled = IsEnabled.Value); return true;
                case nameof(Name): selectedGameObjects.ForEach(x =>x.Name = Name); return true;
            }
            return false;
        }

        public static float? GetMixedValues(List<GameObject> gameObjects, Func<GameObject,float> getProperty)
        {
            var value = getProperty(gameObjects.First());
            foreach (var gameObject in gameObjects.Skip(1)) {
                if (!value.IsTheSameAs(getProperty(gameObject)))
                {
                    return null;
                }
            }
            return value;
        }
        public static bool? GetMixedValues(List<GameObject> gameObjects, Func<GameObject, bool> getProperty)
        {
            var value = getProperty(gameObjects.First());
            foreach (var gameObject in gameObjects.Skip(1))
            {
                if (value != getProperty(gameObject))
                {
                    return null;
                }
            }
            return value;
        }

        public static string? GetMixedValues(List<GameObject> gameObjects, Func<GameObject, string> getProperty)
        {
            var value = getProperty(gameObjects.First());
            foreach (var gameObject in gameObjects.Skip(1))
            {
                if (value != getProperty(gameObject))
                {
                    return null;
                }
            }
            return value;
        }

        protected virtual bool UpdateMSGameObjects()
        {
            IsEnabled = GetMixedValues(selectedGameObjects,new Func<GameObject, bool>(x => x.IsEnabled));

            Name = GetMixedValues(selectedGameObjects, new Func<GameObject, string>(x => x.Name));


            return true;
        }

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSGameObjects();
            _enableUpdates=true;
        }

        public MSObject(List<GameObject> gameObjects)
        {
            Debug.Assert(gameObjects != null);
            Debug.Assert(gameObjects.Count > 0);

            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            selectedGameObjects = gameObjects;

            PropertyChanged += (s, e) =>
            {
                if(_enableUpdates) UpdateGameObjects(e.PropertyName);
            };

            RenameCommand = new RelayCommand<string>(x =>
            {
                var oldNames = selectedGameObjects.Select(g => (g, g.Name)).ToList();
                Name = x; // propagates to all selected objects via UpdateGameObjects

                Project.Current?.UndoRedo.Add(new UndoRedoCommand(
                    $"Rename selection to {x}",
                    execute: () => { oldNames.ForEach(p => p.g.Name = x); Refresh(); },
                    undo: () => { oldNames.ForEach(p => p.g.Name = p.Name); Refresh(); }));
            }, x => x != _name);

            // Escape simply resets the TextBox by forcing the OneWay binding to refresh
            CancelEditCommand = new RelayCommand(() =>
                OnPropertyChanged(nameof(Name)));

            IsEnabledCommand = new RelayCommand<bool>(x =>
            {
                var oldValues = selectedGameObjects.Select(g => (g, g.IsEnabled)).ToList();
                IsEnabled = x; // propagates to all selected objects via UpdateGameObjects

                Project.Current?.UndoRedo.Add(new UndoRedoCommand(
                    $"{(x ? "Enable" : "Disable")} selection",
                    execute: () => { oldValues.ForEach(p => p.g.IsEnabled = x); Refresh(); },
                    undo: () => { oldValues.ForEach(p => p.g.IsEnabled = p.IsEnabled); Refresh(); }));
            });
        }
    }
    class MSGameObject : MSObject
    {
        public MSGameObject(List<GameObject> gameObjects) : base(gameObjects)
        {
            Refresh();
        }
    }
}
