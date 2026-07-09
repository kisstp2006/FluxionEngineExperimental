using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using System;

namespace FluxionEditor.Editor.CustomControls.Numeric;

public partial class Vector2Input : UserControl
{
    public static readonly StyledProperty<string?> HeaderProperty =
        AvaloniaProperty.Register<Vector2Input, string?>(nameof(Header));

    public static readonly StyledProperty<string> XProperty =
        AvaloniaProperty.Register<Vector2Input, string>(nameof(X), "0", defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> YProperty =
        AvaloniaProperty.Register<Vector2Input, string>(nameof(Y), "0", defaultBindingMode: BindingMode.TwoWay);

    public string? Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public string X { get => GetValue(XProperty); set => SetValue(XProperty, value); }
    public string Y { get => GetValue(YProperty); set => SetValue(YProperty, value); }

    /// <summary>Fires when a drag-scrub starts on any child NumberBox.</summary>
    public event EventHandler<RoutedEventArgs> DragStarted
    {
        add => AddHandler(NumberBox.DragStartedEvent, value);
        remove => RemoveHandler(NumberBox.DragStartedEvent, value);
    }

    /// <summary>Fires during drag-scrub with delta info.</summary>
    public event EventHandler<DragDeltaEventArgs> DragDelta
    {
        add => AddHandler(NumberBox.DragDeltaEvent, value);
        remove => RemoveHandler(NumberBox.DragDeltaEvent, value);
    }

    /// <summary>Fires when a drag-scrub ends on any child NumberBox.</summary>
    public event EventHandler<RoutedEventArgs> DragCompleted
    {
        add => AddHandler(NumberBox.DragCompletedEvent, value);
        remove => RemoveHandler(NumberBox.DragCompletedEvent, value);
    }

    public Vector2Input() => InitializeComponent();
}
