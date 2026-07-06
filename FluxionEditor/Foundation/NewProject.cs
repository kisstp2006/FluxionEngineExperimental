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
                    ValidateProjectPath();
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

        private bool _isValidProjectPath;
        public bool IsValidProjectPath
        {
            get { return _isValidProjectPath; }
            set
            {
                if (_isValidProjectPath != value)
                {
                    _isValidProjectPath = value;
                    OnPropertyChanged(nameof(IsValidProjectPath));
                }
            }
        }


        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }


        private bool ValidateProjectPath()
        {
             
            var path = ProjectPath;
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString())) path += Path.DirectorySeparatorChar;

            path += $@"{ProjectName}" + Path.DirectorySeparatorChar;

            IsValidProjectPath = false;

            if(string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMessage = "Project name cannot be empty or white space.";
                Debug.WriteLine($"Project validation failed: {ErrorMessage}");
                
            }
            else if(ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMessage = "Project name cannot be incorrect character(s).";
                Debug.WriteLine($"Project validation failed: {ErrorMessage}");
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMessage = "Project path cannot be empty or white space.";
                Debug.WriteLine($"Project validation failed: {ErrorMessage}");
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMessage = "Project path cannot be incorrect character(s).";
                Debug.WriteLine($"Project validation failed: {ErrorMessage}");
            }
            else if (Directory.Exists(path))
            {
                ErrorMessage = "Project path already exists.";
                Debug.WriteLine($"Project validation failed: {ErrorMessage}");
            }
            else
            {
                IsValidProjectPath = true;
                ErrorMessage = string.Empty;
            }

            return IsValidProjectPath;
        }


        public string CreateProject(ProjectTemplate template)
        {
            Debug.Assert(template != null, "ProjectTemplate cannot be null.");
            ValidateProjectPath();
            if (!IsValidProjectPath)
            {
                Debug.WriteLine("CreateProject aborted: project path is not valid.");
                return "";
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath)) 
               ProjectPath += Path.DirectorySeparatorChar;
            
            var path = $@"{ProjectPath}{ProjectName}" + Path.DirectorySeparatorChar;

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (template.Folders != null)
                {
                    foreach (var folder in template.Folders)
                    {
                        var folderPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder));
                        Directory.CreateDirectory(folderPath);
                    }
                }

                    var dirinfo = new DirectoryInfo(path + @".Fluxion\");
                    dirinfo.Attributes |= FileAttributes.Hidden;

                    if (!string.IsNullOrEmpty(template.IconFilePath) && File.Exists(template.IconFilePath))
                    {
                        File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirinfo.FullName, "icon.png")));
                    }
                    else
                    {
                        Debug.WriteLine($"Skipping icon copy: file not found '{template.IconFilePath}'");
                    }

                    if (!string.IsNullOrEmpty(template.ScreenshotFilePath) && File.Exists(template.ScreenshotFilePath))
                    {
                        File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirinfo.FullName, "screenshot.png")));
                    }
                    else
                    {
                        Debug.WriteLine($"Skipping screenshot copy: file not found '{template.ScreenshotFilePath}'");
                    }

                    
                var projectXMLFile = File.ReadAllText(template.ProjectFilePath);
                projectXMLFile = string.Format(projectXMLFile, ProjectName, ProjectPath);

                var projectPath = Path.GetFullPath(Path.Combine(path,$"{ProjectName}{Project.Extension }"));
                File.WriteAllText(projectPath, projectXMLFile);


                return path;
                

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating project: {ex.Message}");
                MessageBox.Error($"Error creating project: {ex.Message}", "Error");
                return "";
            }
        }


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
                    else
                    {
                        Debug.WriteLine($"Icon not found for template '{template.ProjectType}': {template.IconFilePath}");
                    }
                    template.ScreenshotFilePath = System.IO.Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Files), "screenshot.png"));
                    if (!string.IsNullOrEmpty(template.ScreenshotFilePath) && File.Exists(template.ScreenshotFilePath))
                    {
                        template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    }
                    else
                    {
                        Debug.WriteLine($"Screenshot not found for template '{template.ProjectType}': {template.ScreenshotFilePath}");
                    }
                    template.ProjectFilePath = System.IO.Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Files), template.ProjectFile));

                    Debug.WriteLine($"Loaded template: {template.ProjectType}");
                    _projectTemplates.Add(template);
                }
                ValidateProjectPath();
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
