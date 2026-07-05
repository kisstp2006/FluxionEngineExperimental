using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace FluxionEditor.Foundation
{
    
    public  class Scene : ViewModelBase
    {
        private string _name;

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


        public Project Project { get; private set; }

        public Scene(Project project, string name)
        {
            Debug.Assert(project != null, "Project cannot be null");
            Name = name;
            Project = project;
        }
    }
}
