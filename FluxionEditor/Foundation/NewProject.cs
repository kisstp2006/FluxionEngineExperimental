using FluxionEditor.Foundation.Utilities;
using FluxionEditor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace FluxionEditor.Foundation
{
    /// <summary>
    /// Metadata for a project template (read from ProjectTemplates folder).
    /// </summary>
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }

        [DataMember]
        public string ProjectFile { get; set; }

        [DataMember]
        public List<string> Folders { get; set; }

        // ── Visual assets (loaded from disk, not serialized) ──

        public byte[] Icon { get; set; }
        public string IconFilePath { get; set; }

        public byte[] Screenshot { get; set; }
        public string ScreenshotFilePath { get; set; }

        public string ProjectFilePath { get; set; }
        public string TemplatePath { get;  set; }
    }

    /// <summary>
    /// ViewModel for the "Create Project" flow. Validates paths and
    /// creates the project directory structure on disk.
    /// </summary>
    class NewProject : ViewModelBase
    {
        private string _templatePaths => Path.GetFullPath(Path.Combine(MainWindow.FluxionPath, @"FluxionEditor\ProjectTemplates"));

        // Flag this during development
        private static bool _templatePathWarned;
        private void WarnTemplatePath()
        {
            if (_templatePathWarned) return;
            _templatePathWarned = true;
            Logger.Log(SeverityLevel.Warning, $"Resolved template path: {_templatePaths}");
        }

        // ── Project name ──

        private string _projectName = "Fluxion Game";
        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (_projectName != value)
                {
                    _projectName = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }

        // ── Project projectPath ──

        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\FluxionProject\";

        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        // ── Templates ──

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        // ── Validation ──

        private bool _isValidProjectPath;
        public bool IsValidProjectPath
        {
            get => _isValidProjectPath;
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
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        // ── Validation logic ──

        /// <summary>
        /// Checks whether the current project name + projectPath combination is valid
        /// and updates <see cref="IsValidProjectPath"/> and <see cref="ErrorMessage"/>.
        /// </summary>
        private bool ValidateProjectPath()
        {
            var path = ProjectPath;
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                path += Path.DirectorySeparatorChar;

            path += $@"{ProjectName}" + Path.DirectorySeparatorChar;

            IsValidProjectPath = false;

            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMessage = "Project name cannot be empty or white space.";
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMessage = "Project name contains invalid characters.";
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMessage = "Project projectPath cannot be empty or white space.";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMessage = "Project projectPath contains invalid characters.";
            }
            else if (Directory.Exists(path))
            {
                ErrorMessage = "Project projectPath already exists.";
            }
            else
            {
                IsValidProjectPath = true;
                ErrorMessage = string.Empty;
            }

            return IsValidProjectPath;
        }

        // ── Project creation ──

        /// <summary>
        /// Creates the project directory structure on disk based on the
        /// selected <paramref name="template"/>.
        /// </summary>
        /// <returns>The full project projectPath, or an empty string on failure.</returns>
        public string CreateProject(ProjectTemplate template)
        {
            Debug.Assert(template != null, "ProjectTemplate cannot be null.");

            ValidateProjectPath();
            if (!IsValidProjectPath)
            {
                Logger.Log(SeverityLevel.Warning, "CreateProject aborted: project projectPath is not valid.");
                return "";
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath))
                ProjectPath += Path.DirectorySeparatorChar;

            var path = $@"{ProjectPath}{ProjectName}" + Path.DirectorySeparatorChar;

            try
            {
                // Create main project directory
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // Create template-defined subfolders
                if (template.Folders != null)
                {
                    foreach (var folder in template.Folders)
                    {
                        var folderPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder));
                        Directory.CreateDirectory(folderPath);
                    }
                }

                // Create hidden .Fluxion folder for metadata
                var dirinfo = new DirectoryInfo(path + @".Fluxion\");
                if (!dirinfo.Exists)
                    dirinfo.Create();
                dirinfo.Attributes |= FileAttributes.Hidden;

                // Copy icon
                if (!string.IsNullOrEmpty(template.IconFilePath) && File.Exists(template.IconFilePath))
                {
                    File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirinfo.FullName, "icon.png")));
                }
                else
                {
                    Logger.Log(SeverityLevel.Warning, $"Skipping icon copy: file not found '{template.IconFilePath}'");
                }

                // Copy screenshot
                if (!string.IsNullOrEmpty(template.ScreenshotFilePath) && File.Exists(template.ScreenshotFilePath))
                {
                    File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirinfo.FullName, "screenshot.png")));
                }
                else
                {
                    Logger.Log(SeverityLevel.Warning, $"Skipping screenshot copy: file not found '{template.ScreenshotFilePath}'");
                }

                // Generate project file from template
                var projectXMLFile = File.ReadAllText(template.ProjectFilePath);
                projectXMLFile = string.Format(projectXMLFile, ProjectName, ProjectPath);

                var projectFilePath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(projectFilePath, projectXMLFile);

                CreateCodeProjectFile(template, path);


                return path;
            }
            catch (Exception ex)
            {
                Logger.Log(SeverityLevel.Error, $"Error creating project: {ex.Message}");
                MessageBox.Error($"Error creating project: {ex.Message}", "Error");
                return "";
            }
        }

        private void CreateCodeProjectFile(ProjectTemplate template, string projectPath)
        {
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "MSVCSolution")));
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "MSVCProject")));


            var engineAPIPath = Path.Combine(MainWindow.FluxionPath, @"Engine\EngineAPI");

            Debug.Assert(Directory.Exists(engineAPIPath));

            var _0 = ProjectName;
            var _1= "{"+Guid.NewGuid().ToString().ToUpper()+"}";
            var _2 = "{" + Guid.NewGuid().ToString().ToUpper() + "}";

            var _2a = engineAPIPath; //its the second in the file 

            var _3 = MainWindow.FluxionPath;


            var solution = File.ReadAllText(Path.Combine(template.TemplatePath, "MSVCSolution"));
            solution = String.Format(solution, _0,_1,_2);
            File.WriteAllText(Path.GetFullPath(Path.Combine(projectPath, $"{_0}.sln")), solution);

            // Create GameCode subfolder for the vcxproj (referenced by the .sln)
            var gameCodePath = Path.Combine(projectPath, "GameCode");
            Directory.CreateDirectory(gameCodePath);

            var project = File.ReadAllText(Path.Combine(template.TemplatePath, "MSVCProject"));
            project = String.Format(project, _0, _1, _2a, _3);
            File.WriteAllText(Path.GetFullPath(Path.Combine(projectPath, $@"GameCode\{_0}.vcxproj")), project);
        }

        // ── Constructor ──

        public NewProject()
        {
            WarnTemplatePath();
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);

            try
            {
                var templateFiles = Directory.GetFiles(_templatePaths, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any(), "No project templates found in the specified projectPath.");

                foreach (var file in templateFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    if (template == null) continue;

                    // Load icon
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "icon.png"));
                    if (File.Exists(template.IconFilePath))
                        template.Icon = File.ReadAllBytes(template.IconFilePath);

                    // Load screenshot
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "screenshot.png"));
                    if (File.Exists(template.ScreenshotFilePath))
                        template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);

                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));
                    template.TemplatePath = Path.GetDirectoryName(file);
                    _projectTemplates.Add(template);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(SeverityLevel.Error, $"Error loading project templates: {ex.Message}");
                MessageBox.Error($"Error loading project templates: {ex.Message}", "Error");
            }
        }
    }
}
