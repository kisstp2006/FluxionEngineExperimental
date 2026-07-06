using FluxionEditor.Foundation.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FluxionEditor.Foundation
{
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string Name { get; set; } = string.Empty;
        [DataMember]
        public string Path { get; set; } = string.Empty;
        [DataMember]
        public DateTime LastModified { get; set; }

        public string FullPath => $@"{Path}{Name}{Project.Extension}";

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
    }

    [DataContract]
    public class ProjectDataCollection 
    {
        [DataMember]
        public List<ProjectData> _projects = new List<ProjectData>();
    }



    public class OpenProject
    {
        private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\FluxionEditor\";
        private static readonly string _projectDataFilePath;

        private static readonly ObservableCollection<ProjectData> _projectData = new ObservableCollection<ProjectData>();
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        private static void ReadProjectData()
        {
            try
            {
                if (File.Exists(_projectDataFilePath)) 
                { 
                    var projects = Serializer.FromFile<ProjectDataCollection>(_projectDataFilePath)._projects.OrderByDescending(x=> x.LastModified);
                    _projectData.Clear();

                    foreach (var project in projects)
                    {
                        LoadProjectMedia(project);
                        _projectData.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading project data: {ex.Message}");
                // Handle the exception, e.g., log it or show a message to the user
            }
        }

        private static void WriteProjectData()
        {
            var projects = _projectData.OrderBy(x  => x.LastModified).ToList();
            Serializer.ToFile(new ProjectDataCollection() { _projects=projects}, _projectDataFilePath);
        }

        public static Project Open(ProjectData projectData)
        {
            ReadProjectData();

            var projectFound = _projectData.FirstOrDefault(x => x.FullPath == projectData.FullPath);
            if (projectFound != null)
            {
                projectFound.LastModified = DateTime.Now;
            }
            else
            {
                projectFound = projectData;
                projectFound.LastModified = DateTime.Now;
                LoadProjectMedia(projectFound);
                _projectData.Add(projectFound);
            }

            WriteProjectData();

            return Project.Load(projectData.FullPath);
        }

        private static void LoadProjectMedia(ProjectData project)
        {
            var iconPath = Path.Combine(project.Path, ".Fluxion", "icon.png");
            var screenshotPath = Path.Combine(project.Path, ".Fluxion", "screenshot.png");

            if (File.Exists(iconPath))
                project.Icon = File.ReadAllBytes(iconPath);
            if (File.Exists(screenshotPath))
                project.Screenshot = File.ReadAllBytes(screenshotPath);
        }



        static OpenProject()
        {
            // Static constructor logic here
            try
            {
                if(!Directory.Exists(_applicationDataPath))
                {
                    Directory.CreateDirectory(_applicationDataPath);
                }

                _projectDataFilePath = Path.Combine(_applicationDataPath, "projectData.xml");
                Projects // Initialize the read-only collection
                    = new ReadOnlyObservableCollection<ProjectData>(_projectData);

                ReadProjectData();
            }
            catch (Exception ex) { 
                Debug.WriteLine(ex.Message);
            
            
            
            
            
            }
        }
    }
}
