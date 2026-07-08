using Avalonia;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.IO;

using FluxionEditor.Foundation.Utilities;

namespace FluxionEditor.Foundation.Themes
{
    /// <summary>The selectable editor themes. Each maps to a theme variant in EditorColors.axaml.</summary>
    public enum EditorTheme
    {
        Dark,
        Light
    }

    /// <summary>
    /// Central theme service: switches between editor themes at runtime,
    /// persists the choice so the editor starts with the last used theme,
    /// and is bindable so UI can be attached later, e.g.:
    ///
    ///   &lt;ComboBox ItemsSource="{Binding Themes, Source={x:Static themes:ThemeManager.Instance}}"
    ///             SelectedItem="{Binding CurrentTheme, Source={x:Static themes:ThemeManager.Instance}}"/&gt;
    /// </summary>
    public sealed class ThemeManager : ViewModelBase
    {
        /// <summary>Theme used when no saved preference exists yet.</summary>
        public const EditorTheme DefaultTheme = EditorTheme.Dark;

        private static readonly string _settingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FluxionEditor", "editor.theme");

        public static ThemeManager Instance { get; } = new ThemeManager();

        /// <summary>All selectable themes — item source for theme picker UI.</summary>
        public IReadOnlyList<EditorTheme> Themes { get; } = Enum.GetValues<EditorTheme>();

        private EditorTheme _currentTheme = DefaultTheme;

        /// <summary>
        /// The active theme. Setting it applies the matching theme variant
        /// immediately and persists it as the startup theme.
        /// </summary>
        public EditorTheme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    Apply(value);
                    Save(value);
                    OnPropertyChanged(nameof(CurrentTheme));
                }
            }
        }

        private ThemeManager()
        {
        }

        /// <summary>
        /// Loads the saved startup theme (or the default) and applies it.
        /// Call once during application startup.
        /// </summary>
        public void Initialize()
        {
            _currentTheme = Load();
            Apply(_currentTheme);
            OnPropertyChanged(nameof(CurrentTheme));
        }

        private static void Apply(EditorTheme theme)
        {
            if (Application.Current is { } app)
            {
                app.RequestedThemeVariant = theme == EditorTheme.Light
                    ? ThemeVariant.Light
                    : ThemeVariant.Dark;
            }
        }

        private static EditorTheme Load()
        {
            try
            {
                if (File.Exists(_settingsFilePath) &&
                    Enum.TryParse(File.ReadAllText(_settingsFilePath).Trim(), out EditorTheme saved))
                {
                    return saved;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(SeverityLevel.Warning, $"Could not load theme setting: {ex.Message}");
            }
            return DefaultTheme;
        }

        private static void Save(EditorTheme theme)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath)!);
                File.WriteAllText(_settingsFilePath, theme.ToString());
            }
            catch (Exception ex)
            {
                Logger.Log(SeverityLevel.Warning, $"Could not save theme setting: {ex.Message}");
            }
        }
    }
}
