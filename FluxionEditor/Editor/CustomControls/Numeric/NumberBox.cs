using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Windows.Input;

namespace FluxionEditor.Editor.CustomControls.Numeric;

/// <summary>
/// Event args carrying the drag delta value.
/// </summary>
public class DragDeltaEventArgs : RoutedEventArgs
{
    public double Delta { get; }
    public double NewValue { get; }

    public DragDeltaEventArgs(RoutedEvent routedEvent, double delta, double newValue)
        : base(routedEvent)
    {
        Delta = delta;
        NewValue = newValue;
    }
}

/// <summary>
/// A Unity-style numeric input control that supports mouse-drag scrubbing
/// and click-to-type editing.
/// 
/// <para>Routed events:</para>
/// <list type="bullet">
///   <item><b>DragStarted</b>  — pointer pressed on the control</item>
///   <item><b>DragDelta</b>    — pointer moved while dragging</item>
///   <item><b>DragCompleted</b> — pointer released</item>
/// </list>
/// 
/// <para>Template parts:</para>
/// <list type="bullet">
///   <item><b>PART_textBlock</b> — <see cref="TextBlock"/> for display mode</item>
///   <item><b>PART_textBox</b>  — <see cref="TextBox"/> for edit mode</item>
/// </list>
/// </summary>
public class NumberBox : TemplatedControl
{
    // ── Template parts ───────────────────────────────────────

    private TextBlock? _textBlock;
    private TextBox? _textBox;

    // ── Drag state ───────────────────────────────────────────

    private double _originalValue;
    private double _mouseXStart;
    private bool _captured;
    private bool _isDrag;

    // ═══════════════════════════════════════════════════════════
    //  RoutedEvents
    // ═══════════════════════════════════════════════════════════

    public static readonly RoutedEvent<RoutedEventArgs> DragStartedEvent =
        RoutedEvent.Register<NumberBox, RoutedEventArgs>(
            nameof(DragStarted), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<DragDeltaEventArgs> DragDeltaEvent =
        RoutedEvent.Register<NumberBox, DragDeltaEventArgs>(
            nameof(DragDelta), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> DragCompletedEvent =
        RoutedEvent.Register<NumberBox, RoutedEventArgs>(
            nameof(DragCompleted), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs> DragStarted
    {
        add => AddHandler(DragStartedEvent, value);
        remove => RemoveHandler(DragStartedEvent, value);
    }

    public event EventHandler<DragDeltaEventArgs> DragDelta
    {
        add => AddHandler(DragDeltaEvent, value);
        remove => RemoveHandler(DragDeltaEvent, value);
    }

    public event EventHandler<RoutedEventArgs> DragCompleted
    {
        add => AddHandler(DragCompletedEvent, value);
        remove => RemoveHandler(DragCompletedEvent, value);
    }

    // ═══════════════════════════════════════════════════════════
    //  Value  property  (matches WPF  DependencyProperty)
    // ═══════════════════════════════════════════════════════════

    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<NumberBox, string>(
            nameof(Value),
            defaultValue: "0",
            defaultBindingMode: BindingMode.TwoWay);

    // ═══════════════════════════════════════════════════════════
    //  Label  property — small text shown above the input
    // ═══════════════════════════════════════════════════════════

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<NumberBox, string?>(nameof(Label));

    // ═══════════════════════════════════════════════════════════
    //  IsDragging  property — true while the user drag-scrubs
    // ═══════════════════════════════════════════════════════════

    public bool IsDragging
    {
        get => GetValue(IsDraggingProperty);
        set => SetValue(IsDraggingProperty, value);
    }

    public static readonly StyledProperty<bool> IsDraggingProperty =
        AvaloniaProperty.Register<NumberBox, bool>(nameof(IsDragging));

    // ═══════════════════════════════════════════════════════════
    //  OnApplyTemplate  (WPF:  GetTemplateChild)
    // ═══════════════════════════════════════════════════════════

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _textBlock = e.NameScope.Find<TextBlock>("PART_textBlock");
        _textBox   = e.NameScope.Find<TextBox>("PART_textBox");

        // Wire mouse drag on the whole control surface, not just the tiny TextBlock.
        // This gives us a proper hitbox across the entire NumberBox.
        PointerPressed  += OnTextBlock_Mouse_LBD;
        PointerReleased += OnTextBlock_Mouse_LBU;
        PointerMoved    += OnTextBlock_Mouse_Move;

        // Wire keyboard + focus on the edit TextBox
        if (_textBox is not null)
        {
            _textBox.KeyDown   += OnTextBoxRename_KeyDown;
            _textBox.LostFocus += OnTextBox_LostFocus;
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  OnTextBlock_Mouse_LBD  — start drag / click
    // ═══════════════════════════════════════════════════════════

    private void OnTextBlock_Mouse_LBD(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed) return;

        double.TryParse(Value, out _originalValue);

        e.Pointer.Capture(this);
        _captured = true;
        _isDrag = false;
        IsDragging = true;
        _mouseXStart = e.GetPosition(this).X;

        RaiseEvent(new RoutedEventArgs(DragStartedEvent));
        e.Handled = true;
    }

    // ═══════════════════════════════════════════════════════════
    //  OnTextBlock_Mouse_LBU  — end drag; if value changed → edit
    // ═══════════════════════════════════════════════════════════

    private void OnTextBlock_Mouse_LBU(object? sender, PointerReleasedEventArgs e)
    {
        if (!_captured) return;

        e.Pointer.Capture(null);
        _captured = false;
        IsDragging = false;

        RaiseEvent(new RoutedEventArgs(DragCompletedEvent));
        e.Handled = true;

        // Plain click (no drag) → open TextBox for direct typing.
        // Drag → keep the value and stay in display mode.
        if (!_isDrag && _textBox is not null)
        {
            _textBlock!.IsVisible = false;
            _textBox.IsVisible    = true;
            _textBox.Text         = Value;
            _textBox.Focus();
            _textBox.SelectAll();
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  OnTextBlock_Mouse_Move  — drag-scrub the value
    // ═══════════════════════════════════════════════════════════

    private void OnTextBlock_Mouse_Move(object? sender, PointerEventArgs e)
    {
        if (!_captured) return;

        var mouseX = e.GetPosition(this).X;
        var d = mouseX - _mouseXStart;

        // WPF uses SystemParameters.MinimumHorizontalDragDistance
        const double dragThreshold = 2.0;

        if (Math.Abs(d) > dragThreshold)
        {
            _isDrag = true;
            var newValue = _originalValue + d;
            Value = newValue.ToString("0.####");

            RaiseEvent(new DragDeltaEventArgs(DragDeltaEvent, d, newValue));
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  OnTextBoxRename_KeyDown  — Enter  commit  /  Escape  revert
    // ═══════════════════════════════════════════════════════════

    private void OnTextBoxRename_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        if (e.Key == Key.Enter)
        {
            // WPF pattern: if Tag holds an ICommand, execute it;
            // otherwise just push the value back to the source.
            if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
            {
                command.Execute(textBox.Text);
            }
            else
            {
                Value = textBox.Text ?? Value;
            }

            HideTextBox();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            // Revert to the binding-source value
            textBox.Text = Value;
            HideTextBox();
            e.Handled = true;
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  OnTextBox_LostFocus  — auto-commit when clicking away
    // ═══════════════════════════════════════════════════════════

    private void OnTextBox_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && textBox.IsVisible)
        {
            Value = textBox.Text ?? Value;
            HideTextBox();
        }
    }

    private void HideTextBox()
    {
        if (_textBox is null || _textBlock is null) return;
        _textBox.IsVisible   = false;
        _textBlock.IsVisible = true;
    }
}
