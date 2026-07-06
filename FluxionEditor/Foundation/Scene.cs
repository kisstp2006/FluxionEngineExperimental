using System;
using System.Diagnostics;
using System.Runtime.Serialization;

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

        /// <summary>Parameterless constructor required by DataContractSerializer.</summary>
        private Scene()
        {
        }

        public Scene(Project project, string name)
        {
            Debug.Assert(project != null, "Project cannot be null");
            Name = name;
            Project = project;
        }
    }
}
