using Avalonia.Controls.Primitives;
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
        public int gameObjectId = ID.INVALID_ID;
        public int GameObjectId
        {
            get => gameObjectId;
            set
            {
                if (gameObjectId != value)
                {
                    gameObjectId = value;
                    OnPropertyChanged(nameof(GameObjectId));
                }


            }
        }

        public bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if (isActive)
                    {
                        GameObjectId = EngineAPI.CreateGameObject(this);
                        Debug.Assert(ID.IsValid(GameObjectId));
                    }
                    else if (ID.IsValid(GameObjectId))
                    {
                        EngineAPI.RemoveGameObject(this);
                        GameObjectId = ID.INVALID_ID;
                    }
                    OnPropertyChanged(nameof(IsActive));
                    
                }


            }
        }



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

        public Component GetComponent(Type type) => Components.FirstOrDefault(c=>c.GetType() == type);
        public T GetComponent<T>() where T : Component => GetComponent(typeof(T))as T;

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
            // The constructor doesn't run during deserialization, so the field
            // initializer may not either — guard against a null collection.
            _components ??= new ObservableCollection<Component>();

            // Every game object must have a Transform. New objects get one from
            // the constructor; deserialized (or older/partial) data may be missing
            // it, so ensure it exists exactly once here.
            if (!_components.Any(c => c is Transform))
                _components.Add(new Transform(this));

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

        // ── Mixed value helpers ──
        // Return the value shared by every item, or null when the selection
        // disagrees. A null item (e.g. a game object without the queried
        // component) also counts as "mixed".

        private void MakeComponentList()
        {
            _components.Clear();
            var firstGameObject=selectedGameObjects.FirstOrDefault();
            if(firstGameObject == null)
            {
                return;
            }

            foreach (var component in firstGameObject.Components) 
            { 
                var type = component.GetType();

                if (!selectedGameObjects.Skip(1).Any(gameObject=>gameObject.GetComponent(type)==null))
                {
                    // Guard against adding the same component type twice
                    Debug.Assert(Components.FirstOrDefault(x=>x.GetType() == type) == null);
                    _components.Add(component.GetMultiSelectionComponent(this));
                }
            }
        }


        public static float? GetMixedValues<T>(IReadOnlyList<T> objects, Func<T, float> getProperty)
        {
            if (objects is null) throw new ArgumentNullException(nameof(objects));
            if (getProperty is null) throw new ArgumentNullException(nameof(getProperty));
            if (objects.Count == 0 || objects[0] is null) return null;

            var value = getProperty(objects[0]);
            for (int i = 1; i < objects.Count; i++)
            {
                var item = objects[i];
                if (item is null || !value.IsTheSameAs(getProperty(item)))
                    return null;
            }
            return value;
        }

        public static bool? GetMixedValues<T>(IReadOnlyList<T> objects, Func<T, bool> getProperty)
        {
            if (objects is null) throw new ArgumentNullException(nameof(objects));
            if (getProperty is null) throw new ArgumentNullException(nameof(getProperty));
            if (objects.Count == 0 || objects[0] is null) return null;

            var value = getProperty(objects[0]);
            for (int i = 1; i < objects.Count; i++)
            {
                var item = objects[i];
                if (item is null || value != getProperty(item))
                    return null;
            }
            return value;
        }

        public static string? GetMixedValues<T>(IReadOnlyList<T> objects, Func<T, string> getProperty)
        {
            if (objects is null) throw new ArgumentNullException(nameof(objects));
            if (getProperty is null) throw new ArgumentNullException(nameof(getProperty));
            if (objects.Count == 0 || objects[0] is null) return null;

            var value = getProperty(objects[0]);
            for (int i = 1; i < objects.Count; i++)
            {
                var item = objects[i];
                if (item is null || value != getProperty(item))
                    return null;
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
            MakeComponentList();
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
