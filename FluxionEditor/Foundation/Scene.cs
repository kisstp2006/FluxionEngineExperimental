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
    /// <summary>
    /// A scene within a <see cref="Project"/>. Contains a collection of
    /// <see cref="GameObject"/> instances and commands to manage them.
    /// </summary>
    [DataContract]
    public class Scene : ViewModelBase
    {
        // ── Identity ────────────────────────────────────────────────

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

        // ── Parent project ──────────────────────────────────────────

        /// <summary>
        /// Not serialized — re-linked by <see cref="Project"/>
        /// after deserialization.
        /// </summary>
        internal Project? Project { get; set; }

        // ── Active state ────────────────────────────────────────────

        private bool _isActive;

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

        // ── GameObjects ─────────────────────────────────────────────

        [DataMember(Name = nameof(GameObjects))]
        private ObservableCollection<GameObject> _gameObjects = new ObservableCollection<GameObject>();
        public ReadOnlyObservableCollection<GameObject> GameObjects { get; private set; }

        public ICommand AddGameObjectCommand { get; private set; }
        public ICommand RemoveGameObjectCommand { get; private set; }

        // ── Internal helpers ────────────────────────────────────────

        private void AddGameObjectInternal(GameObject gameObject,int index= -1)
        {
            Debug.Assert(!_gameObjects.Contains(gameObject));
            gameObject.IsActive=IsActive;
            if (index == -1) 
            {     
                _gameObjects.Add(gameObject);
            }
            else
            {
                _gameObjects.Insert(index, gameObject);
            }
        }

        private void RemoveGameObjectInternal(GameObject gameObject)
        {
            Debug.Assert(_gameObjects.Contains(gameObject));
            gameObject.isActive=false;
            _gameObjects.Remove(gameObject);
        }

        // ── Deserialization callback ────────────────────────────────

        /// <summary>
        /// Re-wraps the GameObject collection and wires up
        /// add/remove commands with undo support.
        /// </summary>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Ensure the backing collection is never null (field initializer may not run during deserialization)
            _gameObjects ??= new ObservableCollection<GameObject>();

            // Re-wrap for read-only public access
            GameObjects = new ReadOnlyObservableCollection<GameObject>(_gameObjects);
            OnPropertyChanged(nameof(GameObjects));

            foreach (GameObject gameObject in GameObjects) 
            { 
                gameObject.isActive = IsActive;
            
            }



            // ── Add GameObject command ──

            AddGameObjectCommand = new RelayCommand<object>(_ =>
            {
                var newObj = new GameObject(this) { Name = $"GameObject {_gameObjects.Count}" };
                AddGameObjectInternal(newObj);
                var gameObjectIndex = _gameObjects.Count - 1;

                Project?.UndoRedo.Add(new UndoRedoCommand(
                    $"Add {newObj.Name} to {Name}",
                    execute: () => AddGameObjectInternal(newObj, gameObjectIndex),
                    undo: () => RemoveGameObjectInternal(newObj)
                ));
            });

            // ── Remove GameObject command ──

            RemoveGameObjectCommand = new RelayCommand<GameObject>(x =>
            {
                if (x == null) return;
                var gameObjectIndex = _gameObjects.IndexOf(x);
                RemoveGameObjectInternal(x);

                Project?.UndoRedo.Add(new UndoRedoCommand(
                    $"Remove {x.Name} from {Name}",
                    execute: () => RemoveGameObjectInternal(x),
                    undo: () => AddGameObjectInternal(x, gameObjectIndex)
                ));
            }, x => x != null);
        }

        // ── Constructors ────────────────────────────────────────────

        /// <summary>Parameterless constructor required by DataContractSerializer.</summary>
        private Scene()
        {
        }

        internal Scene(Project project, string name)
        {
            Debug.Assert(project != null, "Project cannot be null");
            Name = name;
            Project = project;
            OnDeserialized(new StreamingContext());
        }
    }
}
