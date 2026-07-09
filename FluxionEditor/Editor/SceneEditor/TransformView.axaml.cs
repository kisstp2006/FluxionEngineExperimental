using Avalonia.Controls;
using Avalonia.Interactivity;
using FluxionEditor.Editor.CustomControls.Numeric;
using FluxionEditor.Foundation.Components;
using FluxionEditor.Foundation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace FluxionEditor;

/// <summary>
/// Inspector editor for the Transform component (multi-selection aware:
/// its DataContext is an <see cref="MSTransform"/>).
/// </summary>
public partial class TransformView : UserControl
{
    // ── Undo snapshots ────────────────────────────────────────

    private List<(Transform transform, Vector3 oldValue)>? _positionSnapshot;
    private List<(Transform transform, Vector3 oldValue)>? _rotationSnapshot;
    private List<(Transform transform, Vector3 oldValue)>? _scaleSnapshot;

    // ═══════════════════════════════════════════════════════════
    //  Init
    // ═══════════════════════════════════════════════════════════

    public TransformView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        WireInput("PositionInput", OnPosition_DragStarted, OnPosition_DragCompleted, OnPosition_ValueCommitted);
        WireInput("RotationInput", OnRotation_DragStarted, OnRotation_DragCompleted, OnRotation_ValueCommitted);
        WireInput("ScaleInput",    OnScale_DragStarted,    OnScale_DragCompleted,    OnScale_ValueCommitted);
    }

    private void WireInput(string name,
        EventHandler<RoutedEventArgs> onStart,
        EventHandler<RoutedEventArgs> onComplete,
        EventHandler<RoutedEventArgs> onCommitted)
    {
        if (this.FindControl<Vector3Input>(name) is { } input)
        {
            input.DragStarted     += onStart;
            input.DragCompleted   += onComplete;
            input.ValueCommitted  += onCommitted;
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  Shared undo helpers
    // ═══════════════════════════════════════════════════════════

    /// <summary>Snapshot all selected Transforms' current value.</summary>
    private static List<(Transform t, Vector3 v)> TakeSnapshot(
        MSTransform vm, Func<Transform, Vector3> getValue)
    {
        return vm.SelectedComponents.Select(t => (t, getValue(t))).ToList();
    }

    /// <summary>
    /// Compare snapshot with current values, push undo if changed,
    /// and update the snapshot for the next interaction.
    /// </summary>
    private void TryCommitUndo(
        string undoName,
        Func<Transform, Vector3> getValue,
        Action<Transform, Vector3> setValue,
        ref List<(Transform t, Vector3 v)>? snapshot)
    {
        if (DataContext is not MSTransform vm || snapshot is null) return;

        var oldValues = snapshot;
        var newValues = TakeSnapshot(vm, getValue);
        snapshot = newValues;   // already ready for the next action

        if (!oldValues.Any(s => getValue(s.t) != s.v)) return;

        var project = vm.SelectedComponents.FirstOrDefault()?.Owner?.ParentScene?.Project;
        if (project is null) return;

        var self = this;

        project.UndoRedo.Add(new UndoRedoCommand(
            undoName,
            execute: () =>
            {
                foreach (var (t, val) in newValues) setValue(t, val);
                if (self.DataContext is MSTransform vm) vm.Refresh();
            },
            undo: () =>
            {
                foreach (var (t, val) in oldValues) setValue(t, val);
                if (self.DataContext is MSTransform vm) vm.Refresh();
            }));
    }

    // ═══════════════════════════════════════════════════════════
    //  Position
    // ═══════════════════════════════════════════════════════════

    private void OnPosition_DragStarted(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MSTransform vm)
            _positionSnapshot = TakeSnapshot(vm, t => t.Position);
    }

    private void OnPosition_DragCompleted(object? sender, RoutedEventArgs e)
    {
        TryCommitUndo("Move", t => t.Position, (t, v) => t.Position = v, ref _positionSnapshot);
    }

    private void OnPosition_ValueCommitted(object? sender, RoutedEventArgs e)
    {
        TryCommitUndo("Move", t => t.Position, (t, v) => t.Position = v, ref _positionSnapshot);
    }

    // ═══════════════════════════════════════════════════════════
    //  Rotation
    // ═══════════════════════════════════════════════════════════

    private void OnRotation_DragStarted(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MSTransform vm)
            _rotationSnapshot = TakeSnapshot(vm, t => t.Rotation);
    }

    private void OnRotation_DragCompleted(object? sender, RoutedEventArgs e)
    {
        TryCommitUndo("Rotate", t => t.Rotation, (t, v) => t.Rotation = v, ref _rotationSnapshot);
    }

    private void OnRotation_ValueCommitted(object? sender, RoutedEventArgs e)
    {
        TryCommitUndo("Rotate", t => t.Rotation, (t, v) => t.Rotation = v, ref _rotationSnapshot);
    }

    // ═══════════════════════════════════════════════════════════
    //  Scale
    // ═══════════════════════════════════════════════════════════

    private void OnScale_DragStarted(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MSTransform vm)
            _scaleSnapshot = TakeSnapshot(vm, t => t.Scale);
    }

    private void OnScale_DragCompleted(object? sender, RoutedEventArgs e)
    {
        TryCommitUndo("Scale", t => t.Scale, (t, v) => t.Scale = v, ref _scaleSnapshot);
    }

    private void OnScale_ValueCommitted(object? sender, RoutedEventArgs e)
    {
        TryCommitUndo("Scale", t => t.Scale, (t, v) => t.Scale = v, ref _scaleSnapshot);
    }
}
