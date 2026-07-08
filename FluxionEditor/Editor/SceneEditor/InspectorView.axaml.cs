using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FluxionEditor;

public partial class InspectorView : UserControl
{
    public static InspectorView Instance { get; private set; } = null!;
    public InspectorView()
    {
        InitializeComponent();
        DataContext = null;
        Instance = this;
    }
}