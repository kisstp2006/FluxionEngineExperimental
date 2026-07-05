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
using System.Collections.ObjectModel;
using System.Linq;


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

        public byte[] Icon { get; set; }
        public string IconFilePath { get; set; }

        public byte[] Screenshot { get; set; }
        public string ScreenshotFilePath { get; set; }

        public string ProjectFilePath { get; set; }



    }

    class NewProject : ViewModelBase
    {
        private readonly string _templatePaths = @"..\..\FluxionEditor\ProjectTemplates"; //TODO DONT HARDCODE THIS PATH, USE RELATIVE PATHS OR CONFIGURATION FILES

        private string _projectname = "Fluxion Game";
        public string ProjectName { 
            get { return _projectname; } 
            set { if (_projectname != value) 
                { 
                    _projectname = value; 
                    OnPropertyChanged(nameof(ProjectName));
                } 
            } 
        }


        private string _projectpath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\FluxionProject\";

        public string ProjectPath
        {
            get { return _projectpath; }
            set
            {
                if (_projectpath != value)
                {
                    _projectpath = value;
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }


        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                var templateFiles = Directory.GetFiles(_templatePaths, "template.xml", SearchOption.AllDirectories);

                Debug.Assert(templateFiles.Any(), "No project templates found in the specified path.");

                foreach (var Files in templateFiles)
                {
                   var template =  Serializer.FromFile<ProjectTemplate>(Files);
                    if (template == null) continue;
                    template.IconFilePath = System.IO.Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Files),"icon.png"));
                    if (!string.IsNullOrEmpty(template.IconFilePath) && File.Exists(template.IconFilePath))
                    {
                        template.Icon = File.ReadAllBytes(template.IconFilePath);
                    }
                    template.ScreenshotFilePath = System.IO.Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Files), "screenshot.png"));
                    if (!string.IsNullOrEmpty(template.ScreenshotFilePath) && File.Exists(template.ScreenshotFilePath))
                    {
                        template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    }

                    _projectTemplates.Add(template);
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
