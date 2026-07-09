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

        WireInput("PositionInput", OnPosition_DragStarted, OnPosition_DragCompleted);
        WireInput("RotationInput", OnRotation_DragStarted, OnRotation_DragCompleted);
        WireInput("ScaleInput",    OnScale_DragStarted,    OnScale_DragCompleted);
    }

    private void WireInput(string name,
        EventHandler<RoutedEventArgs> onStart,
        EventHandler<RoutedEventArgs> onComplete)
    {
        if (this.FindControl<Vector3Input>(name) is { } input)
        {
            input.DragStarted   += onStart;
            input.DragCompleted += onComplete;
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
    /// Compare snapshot with current values and, if anything changed,
    /// push an <see cref="UndoRedoCommand"/> to the project.
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
        snapshot = null;

        bool anyChanged = oldValues.Any(s => getValue(s.t) != s.v);
        if (!anyChanged) return;

        var project = vm.SelectedComponents.FirstOrDefault()?.Owner?.ParentScene?.Project;
        if (project is null) return;

        project.UndoRedo.Add(new UndoRedoCommand(
            undoName,
            execute: () => { foreach (var (t, v) in newValues) setValue(t, v); },
            undo:    () => { foreach (var (t, v) in oldValues) setValue(t, v); }));
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
}
