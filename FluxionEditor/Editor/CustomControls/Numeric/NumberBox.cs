using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Globalization;
using System.Windows.Input;

namespace FluxionEditor.Editor.CustomControls.Numeric;

/// <summary>
/// A Unity-style numeric input control that supports mouse-drag scrubbing
/// and click-to-type editing.
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
    private StackPanel? _dragTickRow;
    private TextBlock? _dragDeltaText;
    private TextBlock? _dragValueText;

    // ── Drag state ───────────────────────────────────────────

    private double _originalValue;
    private double _mouseXStart;
    private bool _captured;
    private bool _isDrag;

    /// <summary>Maximum tick labels shown in the drag-scrub overlay at once.</summary>
    private const int MaxDragTicks = 7;

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
    //  IsDragging  property — drives the drag-scrub overlay popup.
    //  Read-only from the outside; only this class calls SetValue.
    // ═══════════════════════════════════════════════════════════

    public static readonly StyledProperty<bool> IsDraggingProperty =
        AvaloniaProperty.Register<NumberBox, bool>(nameof(IsDragging));

    public bool IsDragging => GetValue(IsDraggingProperty);

    // ═══════════════════════════════════════════════════════════
    //  OnApplyTemplate  (WPF:  GetTemplateChild)
    // ═══════════════════════════════════════════════════════════

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _textBlock     = e.NameScope.Find<TextBlock>("PART_textBlock");
        _textBox       = e.NameScope.Find<TextBox>("PART_textBox");
        _dragTickRow   = e.NameScope.Find<StackPanel>("PART_TickRow");
        _dragDeltaText = e.NameScope.Find<TextBlock>("PART_DeltaText");
        _dragValueText = e.NameScope.Find<TextBlock>("PART_ValueText");

        // Wire mouse drag on the TextBlock (exact WPF event names)
        if (_textBlock is not null)
        {
            _textBlock.PointerPressed     += OnTextBlock_Mouse_LBD;
            _textBlock.PointerReleased    += OnTextBlock_Mouse_LBU;
            _textBlock.PointerMoved       += OnTextBlock_Mouse_Move;
            _textBlock.PointerCaptureLost += OnTextBlock_PointerCaptureLost;
        }

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

        // Invariant culture: on non-US locales (e.g. Hungarian, which uses a
        // comma decimal separator), CurrentCulture parsing/formatting would
        // silently disagree with the "." format the rest of the app expects.
        double.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out _originalValue);

        e.Pointer.Capture(sender as InputElement);
        _captured = true;
        _isDrag = false;
        _mouseXStart = e.GetPosition(this).X;
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
        e.Handled = true;
        SetValue(IsDraggingProperty, false);

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
            if (!_isDrag)
            {
                _isDrag = true;
                SetValue(IsDraggingProperty, true);
            }

            var newValue = _originalValue + d;
            Value = newValue.ToString("0.####", CultureInfo.InvariantCulture);
            UpdateDragOverlay(newValue, d);
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  UpdateDragOverlay — fills the floating snap-ruler popup that
    //  appears above the field while dragging: a row of nearby whole-
    //  number ticks (the one closest to the current value highlighted)
    //  plus the raw pixel delta and the precise resulting value.
    // ═══════════════════════════════════════════════════════════

    private void UpdateDragOverlay(double currentValue, double pixelDelta)
    {
        if (_dragTickRow is null || _dragDeltaText is null || _dragValueText is null) return;

        int lo = (int)Math.Floor(Math.Min(_originalValue, currentValue));
        int hi = (int)Math.Ceiling(Math.Max(_originalValue, currentValue));

        // Once the drag spans more than MaxDragTicks whole numbers, stop
        // growing and instead center a fixed-size window on the current
        // value so the ruler doesn't run off the screen on a long drag.
        if (hi - lo + 1 > MaxDragTicks)
        {
            int center = (int)Math.Round(currentValue);
            lo = center - MaxDragTicks / 2;
            hi = lo + MaxDragTicks - 1;
        }

        int nearest = (int)Math.Round(currentValue);

        _dragTickRow.Children.Clear();
        for (int i = lo; i <= hi; i++)
        {
            var tick = new TextBlock
            {
                Text = i.ToString(CultureInfo.InvariantCulture),
                Classes = { "DragTick" }
            };
            if (i == nearest)
                tick.Classes.Add("Current");

            _dragTickRow.Children.Add(tick);
        }

        _dragDeltaText.Text = (pixelDelta >= 0 ? "+" : "") + pixelDelta.ToString("0.##", CultureInfo.InvariantCulture);
        _dragValueText.Text = currentValue.ToString("0.#####", CultureInfo.InvariantCulture);
    }

    // ═══════════════════════════════════════════════════════════
    //  OnTextBlock_PointerCaptureLost — defensive reset
    // ═══════════════════════════════════════════════════════════
    //  Capture can be stolen or dropped by the platform (e.g. Alt-Tab,
    //  a modal popup opening) without a matching PointerReleased ever
    //  firing. Without this, _captured/_isDrag would stay stuck true
    //  and the control would misbehave on the next click.

    private void OnTextBlock_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _captured = false;
        _isDrag = false;
        SetValue(IsDraggingProperty, false);
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
