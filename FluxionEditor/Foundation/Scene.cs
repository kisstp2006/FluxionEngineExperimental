using Avalonia.Controls;
using FluxionEditor.Foundation.Components;
using FluxionEditor.Foundation.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace FluxionEditor.Foundation
{
    [DataContract]
    public  class Scene : ViewModelBase
    {
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

        /// <summary>Not serialized — re-linked by <see cref="Project"/> after deserialization.</summary>
        public Project? Project { get; internal set; }
        
        public bool _isActive;
        
        [DataMember]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }


        /// <summary>Parameterless constructor required by DataContractSerializer.</summary>
        private Scene()
        {
        }
        [DataMember(Name = nameof(GameObjects))]
        private readonly ObservableCollection<GameObject> _gameObjects = new ObservableCollection<GameObject>();
        public ReadOnlyObservableCollection<GameObject> GameObjects { get; private set; }

        public ICommand AddGameObjectCommand { get; private set; }
        public ICommand RemoveGameObjectCommand { get; private set; }



        private void AddGameObjectInternal(GameObject gameObject)
        {
            Debug.Assert(!_gameObjects.Contains(gameObject));  
            _gameObjects.Add(gameObject);
        }

        private void RemoveGameObjectInternal(GameObject gameObject)
        {
            Debug.Assert(_gameObjects.Contains(gameObject));
            _gameObjects.Remove(gameObject);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_gameObjects != null)
            {
                GameObjects = new ReadOnlyObservableCollection<GameObject>(_gameObjects);
                OnPropertyChanged(nameof(GameObjects));
            }

            AddGameObjectCommand = new RelayCommand<GameObject>(x =>
            {
                if (x == null) return;
                AddGameObjectInternal(x);
                var gameObjectIndex = _gameObjects.Count - 1;

                Project?.UndoRedo.Add(new UndoRedoCommand(
                    $"Add {x.Name} to {Name}",
                    execute: () => _gameObjects.Insert(gameObjectIndex, x),
                    undo: () => RemoveGameObjectInternal(x)
                ));
            });

            RemoveGameObjectCommand = new RelayCommand<GameObject>(x =>
            {
                if (x == null) return;
                var gameObjectIndex = _gameObjects.IndexOf(x);
                RemoveGameObjectInternal(x);

                Project?.UndoRedo.Add(new UndoRedoCommand(
                    $"Remove {x.Name} from {Name}",
                    execute: () => RemoveGameObjectInternal(x),
                    undo: () => _gameObjects.Insert(gameObjectIndex, x)
                ));
            }, x => x != null);
        }



        public Scene(Project project, string name)
        {
            Debug.Assert(project != null, "Project cannot be null");
            Name = name;
            Project = project;
            OnDeserialized(new StreamingContext());
        }
    }
}
