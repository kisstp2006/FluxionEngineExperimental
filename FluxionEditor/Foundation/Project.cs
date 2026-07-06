using Avalonia;
using FluxionEditor.Foundation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;

namespace FluxionEditor.Foundation
{
    [DataContract (Name ="Game")]
    public  class Project : ViewModelBase
    {

        public static string Extension => ".fluxion";
        [DataMember]
        public string Name { get; set; } = "Fluxion Project";
        [DataMember]
        public string Path { get; set; }

        public string FullPath => System.IO.Path.Combine(Path, Name + Extension);
        [DataMember (Name ="Scenes")]
        private ObservableCollection<Scene> _scenes  = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        public static UndoRedo UndoRedo { get; } = new UndoRedo();

        private Scene activeScene;
        public Scene ActiveScene
        {
            get => activeScene;

            set
            {
                if (activeScene != value)
                {
                    activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }


        public static Project Current => Application.Current.DataContext as Project;

        private void AddSceneInternal(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }

        public ICommand AddScene {  get; private set; }
        public ICommand RemoveScene { get; private set; }


        public ICommand Undo { get; private set; }
        public ICommand Redo { get; private set; }


        private void RemoveSceneInternal(Scene scene)
        {
            Debug.Assert(scene != null, "Scene cannot be null.");
            Debug.Assert(_scenes.Contains(scene), "Scene does not belong to this project.");
            _scenes.Remove(scene);
        }

        public static Project Load (string file) 
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public void Unload()
        {

        }

        public static void Save(Project project)
        {
            
           Serializer.ToFile(project, project.FullPath);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if(_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }

            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

            AddScene = new RelayCommand<object>(x =>
            {
                AddSceneInternal($"New Scene {Scenes.Count}");
                var newScene = _scenes.Last();
                var sceneIndex = _scenes.Count()-1;
                UndoRedo.Add(new UndoRedoCommand(
                    $"Add new scene {newScene.Name}",
                    execute: () => AddSceneInternal(newScene.Name),
                    undo: () => RemoveSceneInternal(newScene)
                ));
            });

            RemoveScene = new RelayCommand<Scene>(x =>
            {
                var sceneIndex = _scenes.IndexOf(x);
                RemoveSceneInternal(x);

                UndoRedo.Add(new UndoRedoCommand(
                    $"Remove scene {x.Name}",
                    execute: () => RemoveSceneInternal(x),
                    undo: () => _scenes.Insert(sceneIndex, x)
                ));
            },x=>!x.IsActive);


            RelayCommand<object> undoCmd = null!;
            RelayCommand<object> redoCmd = null!;

            undoCmd = new RelayCommand<object>(x =>
            {
                UndoRedo.Undo();
                undoCmd.NotifyCanExecuteChanged();
                redoCmd.NotifyCanExecuteChanged();
            }, x => UndoRedo.UndoStack.Count > 0);

            redoCmd = new RelayCommand<object>(x =>
            {
                UndoRedo.Redo();
                undoCmd.NotifyCanExecuteChanged();
                redoCmd.NotifyCanExecuteChanged();
            }, x => UndoRedo.RedoStack.Count > 0);

            Undo = undoCmd;
            Redo = redoCmd;

        }

        public Project(string name, string path)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "Project name cannot be null or empty.");
            Debug.Assert(!string.IsNullOrEmpty(path), "Project path cannot be null or empty.");
            Name = name;
            Path = path;

            AddScene = new RelayCommand<string?>(AddSceneInternal);
            RemoveScene = new RelayCommand<Scene>(RemoveSceneInternal);

            OnDeserialized(new StreamingContext());
        }
    }
}