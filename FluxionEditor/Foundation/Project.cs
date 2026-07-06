using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace FluxionEditor.Foundation
{
    [DataContract (Name ="Game")]
    public  class Project : ViewModelBase
    {

        public static string Extension => ".fluxion";
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Path { get; set; }

        public string FullPath => System.IO.Path.Combine(Path, Name + Extension);
        [DataMember (Name ="Scenes")]
        private ObservableCollection<Scene> _scenes  = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; } 


        public Project(string name, string path)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "Project name cannot be null or empty.");
            Debug.Assert(!string.IsNullOrEmpty(path), "Project path cannot be null or empty.");
            Name = name;
            Path = path;
        

            _scenes.Add(new Scene(this, "Main Scene"));
        }
    }
}
