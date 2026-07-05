using FluxionEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace FluxionEditor.Foundation
{
    class ProjectProperty : ViewModelBase
    {
        private string _projectname = "Fluxion Game";
        public string Name { 
            get { return _projectname; } 
            set { if (_projectname != value) 
                { 
                    _projectname = value; 
                    OnPropertyChanged(nameof(Name));
                } 
            } 
        }


        private string _projectpath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\FluxionProject\";

        public string Path
        {
            get { return _projectpath; }
            set
            {
                if (_projectpath != value)
                {
                    _projectpath = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }
        
    }
}
