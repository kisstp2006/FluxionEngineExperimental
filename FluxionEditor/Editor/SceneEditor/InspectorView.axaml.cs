using Avalonia.Controls;

namespace FluxionEditor;

/// <summary>
/// Displays the currently selected GameObject's properties (name, enabled,
/// components) for editing. Singleton — <see cref="Instance"/> is set once on creation.
/// </summary>
public partial class InspectorView : UserControl
{
    /// <summary>Singleton accessor for external code to set the DataContext.</summary>
    public static InspectorView Instance { get; private set; } = null!;

    public InspectorView()
    {
        InitializeComponent();
        DataContext = null;
        Instance = this;
    }
}