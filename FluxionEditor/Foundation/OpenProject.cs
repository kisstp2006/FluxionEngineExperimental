using FluxionEditor.Foundation.Utilities;
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
    /// Lightweight metadata about a saved project (shown in the Open Project list).
    /// </summary>
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string Name { get; set; } = string.Empty;

        [DataMember]
        public string Path { get; set; } = string.Empty;

        [DataMember]
        public DateTime LastModified { get; set; }

        /// <summary>Full path to the .fluxion project file.</summary>
        public string FullPath => $@"{Path}{Name}{Project.Extension}";

        // ── Thumbnails (loaded from disk, not serialized) ──

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
    }

    /// <summary>
    /// Wrapper for serializing the list of known projects to disk.
    /// </summary>
    [DataContract]
    public class ProjectDataCollection
    {
        [DataMember(Name = "_projects")]
        public List<ProjectData> Projects { get; set; } = new List<ProjectData>();
    }

    /// <summary>
    /// Manages the list of recently opened projects and handles
    /// project loading, thumbnail extraction, and persistence.
    /// </summary>
    public class OpenProject
    {
        private static readonly string _applicationDataPath =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\FluxionEditor\";

        private static readonly string _projectDataFilePath;

        private static readonly ObservableCollection<ProjectData> _projectData = new ObservableCollection<ProjectData>();

        /// <summary>Read-only list of known projects for UI binding.</summary>
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        // ── Static constructor ──

        static OpenProject()
        {
            try
            {
                if (!Directory.Exists(_applicationDataPath))
                    Directory.CreateDirectory(_applicationDataPath);

                _projectDataFilePath = Path.Combine(_applicationDataPath, "projectData.xml");
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projectData);

                ReadProjectData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // ── Persistence ──

        private static void ReadProjectData()
        {
            try
            {
                if (!File.Exists(_projectDataFilePath))
                    return;

                var projects = Serializer
                    .FromFile<ProjectDataCollection>(_projectDataFilePath)
                    .Projects
                    .OrderByDescending(x => x.LastModified);

                _projectData.Clear();
                foreach (var project in projects)
                {
                    LoadProjectMedia(project);
                    _projectData.Add(project);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading project data: {ex.Message}");
            }
        }

        private static void WriteProjectData()
        {
            var projects = _projectData.OrderBy(x => x.LastModified).ToList();
            Serializer.ToFile(new ProjectDataCollection { Projects = projects }, _projectDataFilePath);
        }

        // ── Loading ──

        /// <summary>
        /// Opens the project described by <paramref name="projectData"/>.
        /// Updates the last-modified timestamp and persists the project list.
        /// </summary>
        public static Project Open(ProjectData projectData)
        {
            ReadProjectData();

            var existing = _projectData.FirstOrDefault(x => x.FullPath == projectData.FullPath);
            if (existing != null)
            {
                existing.LastModified = DateTime.Now;
            }
            else
            {
                existing = projectData;
                existing.LastModified = DateTime.Now;
                LoadProjectMedia(existing);
                _projectData.Add(existing);
            }

            WriteProjectData();
            return Project.Load(projectData.FullPath);
        }

        // ── Media ──

        private static void LoadProjectMedia(ProjectData project)
        {
            var iconPath = Path.Combine(project.Path, ".Fluxion", "icon.png");
            var screenshotPath = Path.Combine(project.Path, ".Fluxion", "screenshot.png");

            if (File.Exists(iconPath))
                project.Icon = File.ReadAllBytes(iconPath);

            if (File.Exists(screenshotPath))
                project.Screenshot = File.ReadAllBytes(screenshotPath);
        }
    }
}
