using Avalonia;
using FluxionEditor.Foundation.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace FluxionEditor.Foundation
{
    /// <summary>
    /// Represents a game project. Serialized to disk as the main project file.
    /// Contains scenes, undo/redo history, and top-level editor commands.
    /// </summary>
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        // ── File metadata ──────────────────────────────────────────

        public static string Extension => ".fluxion";

        [DataMember]
        public string Name { get; set; } = "Fluxion Project";

        [DataMember]
        public string Path { get; set; }

        /// <summary>Full file path including name and extension.</summary>
        public string FullPath => $@"{Path}{Name}\{Name}{Extension}";

        // ── Scenes ──────────────────────────────────────────────────

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        private Scene _activeScene;
        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                if (_activeScene != value)
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }

        // ── Undo / Redo ─────────────────────────────────────────────

        private UndoRedo? _undoRedo;

        /// <summary>
        /// Lazily-initialized undo/redo manager.
        /// Not serialized — recreated on load.
        /// </summary>
        public UndoRedo UndoRedo => _undoRedo ??= new UndoRedo();

        // ── Commands ────────────────────────────────────────────────

        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveSceneCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        // ── Static helpers ──────────────────────────────────────────

        /// <summary>Returns the currently loaded project, if any.</summary>
        public static Project Current => Application.Current.DataContext as Project;

        // ── Scene management ────────────────────────────────────────

        private void AddSceneInternal(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }

        private void RemoveSceneInternal(Scene scene)
        {
            Debug.Assert(scene != null, "Scene cannot be null.");
            Debug.Assert(_scenes.Contains(scene), "Scene does not belong to this project.");
            _scenes.Remove(scene);
        }

        // ── Persistence ─────────────────────────────────────────────

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public void Unload()
        {
            UndoRedo.Reset();
            // TODO: clean up resources, close open file handles
        }

        /// <summary>Saves the project to its <see cref="FullPath"/> on disk.</summary>
        public static void Save(Project project)
        {
            Debug.WriteLine($"Saving project {project.Name} to {project.FullPath}");
            Serializer.ToFile(project, project.FullPath);
        }

        // ── Deserialization callback ────────────────────────────────

        /// <summary>
        /// Called by <see cref="DataContractSerializer"/> after loading,
        /// and manually from the constructor. Re-wraps collections and
        /// wires up all editor commands with undo support.
        /// </summary>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Ensure the backing collection is never null (field initializer may not run during deserialization)
            _scenes ??= new ObservableCollection<Scene>();

            // Re-wrap scene collection for read-only public access
            Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
            OnPropertyChanged(nameof(Scenes));

            // Re-link parent project on each scene (not serialized)
            foreach (var scene in _scenes)
                scene.Project = this;

            ActiveScene = Scenes?.FirstOrDefault(x => x.IsActive);

            // ── Add / Remove Scene commands (with undo support) ──

            AddSceneCommand = new RelayCommand<object>(_ =>
            {
                AddSceneInternal($"New Scene {Scenes?.Count ?? 0}");
                var newScene = _scenes.Last();
                var sceneIndex = _scenes.Count - 1;

                UndoRedo.Add(new UndoRedoCommand(
                    $"Add new scene {newScene.Name}",
                    execute: () => AddSceneInternal(newScene.Name),
                    undo: () => RemoveSceneInternal(newScene)
                ));
            });

            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                if (x == null) return;
                var sceneIndex = _scenes.IndexOf(x);
                RemoveSceneInternal(x);

                UndoRedo.Add(new UndoRedoCommand(
                    $"Remove scene {x.Name}",
                    execute: () => RemoveSceneInternal(x),
                    undo: () => _scenes.Insert(sceneIndex, x)
                ));
            }, x => x != null && !x.IsActive);

            // ── Undo / Redo commands (must reference each other) ──

            RelayCommand<object> undoCmd = null!;
            RelayCommand<object> redoCmd = null!;

            undoCmd = new RelayCommand<object>(_ =>
            {
                UndoRedo.Undo();
                undoCmd.NotifyCanExecuteChanged();
                redoCmd.NotifyCanExecuteChanged();
            }, _ => UndoRedo.UndoStack.Count > 0);

            redoCmd = new RelayCommand<object>(_ =>
            {
                UndoRedo.Redo();
                undoCmd.NotifyCanExecuteChanged();
                redoCmd.NotifyCanExecuteChanged();
            }, _ => UndoRedo.RedoStack.Count > 0);

            UndoCommand = undoCmd;
            RedoCommand = redoCmd;

            // ── Save command ──

            SaveCommand = new RelayCommand<object>(_ => Project.Save(this), _ => true);
        }

        // ── Constructor ─────────────────────────────────────────────

        public Project(string name, string path)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "Project name cannot be null or empty.");
            Debug.Assert(!string.IsNullOrEmpty(path), "Project path cannot be null or empty.");
            Name = name;
            Path = path;

            // Default commands (overwritten by OnDeserialized for full undo support)
            AddSceneCommand = new RelayCommand<string?>(AddSceneInternal);
            RemoveSceneCommand = new RelayCommand<Scene>(RemoveSceneInternal);

            OnDeserialized(new StreamingContext());
        }
    }
}