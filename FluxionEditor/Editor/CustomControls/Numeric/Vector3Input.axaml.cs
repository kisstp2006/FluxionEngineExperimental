using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace FluxionEditor.Editor.CustomControls.Numeric;

public partial class Vector3Input : UserControl
{
    public static readonly StyledProperty<string?> HeaderProperty =
        AvaloniaProperty.Register<Vector3Input, string?>(nameof(Header));

    public static readonly StyledProperty<string> XProperty =
        AvaloniaProperty.Register<Vector3Input, string>(nameof(X), "0", defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> YProperty =
        AvaloniaProperty.Register<Vector3Input, string>(nameof(Y), "0", defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> ZProperty =
        AvaloniaProperty.Register<Vector3Input, string>(nameof(Z), "0", defaultBindingMode: BindingMode.TwoWay);

    public string? Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public string X { get => GetValue(XProperty); set => SetValue(XProperty, value); }
    public string Y { get => GetValue(YProperty); set => SetValue(YProperty, value); }
    public string Z { get => GetValue(ZProperty); set => SetValue(ZProperty, value); }

    public Vector3Input() => InitializeComponent();
}
