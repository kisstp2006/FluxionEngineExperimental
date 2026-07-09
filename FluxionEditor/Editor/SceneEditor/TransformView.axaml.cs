using Avalonia.Controls;

namespace FluxionEditor;

/// <summary>
/// Inspector editor for the Transform component (multi-selection aware:
/// its DataContext is an <see cref="Foundation.Components.MSTransform"/>).
/// </summary>
public partial class TransformView : UserControl
{
    public TransformView()
    {
        InitializeComponent();
    }
}
