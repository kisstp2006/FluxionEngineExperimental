using Avalonia;
using Avalonia.Controls;

namespace FluxionEditor.Editor.CustomControls;

/// <summary>
/// Reusable collapsible container for inspector component editors.
/// The header band always stretches to the full available width;
/// the XAML children become the collapsible content.
///
/// Usage:
///   &lt;controls:ComponentView Header="Transform"&gt;
///       ...component inputs...
///   &lt;/controls:ComponentView&gt;
/// </summary>
public class ComponentView : ContentControl
{
    /// <summary>Title shown in the collapsible header band.</summary>
    public static readonly StyledProperty<string?> HeaderProperty =
        AvaloniaProperty.Register<ComponentView, string?>(nameof(Header));

    public string? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>Whether the content is currently expanded.</summary>
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<ComponentView, bool>(nameof(IsExpanded), defaultValue: true);

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
}
