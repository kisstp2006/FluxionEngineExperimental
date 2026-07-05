using FluxionEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using FluxionEditor.Foundation.Utilities;


namespace FluxionEditor.Foundation
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }

        [DataMember]
        public string ProjectFile { get; set; }

        // TODO: public string Description { get; set; }

        [DataMember]
        public List<string> Folders { get; set; }
    }

    class NewProject : ViewModelBase
    {
        private readonly string _templatePaths = @"..\..\FluxionEditor\ProjectTemplates"; //TODO DONT HARDCODE THIS PATH, USE RELATIVE PATHS OR CONFIGURATION FILES

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
        public NewProject()
        {
            try
            {
                var templateFiles = Directory.GetFiles(_templatePaths, "template.xml", SearchOption.AllDirectories);

                Debug.Assert(templateFiles.Length <1, "No project templates found in the specified path.");

                foreach (var Files in templateFiles)
                {
                    var template = new ProjectTemplate()
                    {
                        ProjectType = "Empty Project",
                        ProjectFile = "project.fluxion",
                        Folders = new List<string> {".Fluxion", "Assets", "Scripts", "Scenes" }

                    };

                    Serializer.ToFile(template, Files);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading project templates: {ex.Message}");
                MessageBox.Error($"Error loading project templates: {ex.Message}", "Error");

                //TODO: log error
            }

        }

    }

}
