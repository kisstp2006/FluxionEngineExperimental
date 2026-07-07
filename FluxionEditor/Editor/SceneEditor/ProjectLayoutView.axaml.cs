using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluxionEditor.Foundation;
using FluxionEditor.Foundation.Components;
using FluxionEditor.Foundation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluxionEditor;

public partial class ProjectLayoutView : UserControl
{
    public ProjectLayoutView()
    {
        InitializeComponent();
    }

    private void On_AddGameObject_Button_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var btn = sender as Button;
        var vm = btn?.DataContext as Scene;

        vm.AddGameObjectCommand.Execute(new GameObject(vm) { Name = "GameObject" });
    }

    private void OnGameObject_list_Selection_Changed(object? sender, SelectionChangedEventArgs e)
    {
        
        var listBox = sender as ListBox;
        var newSelectedGameObject = listBox?.SelectedItems.Cast<GameObject>().ToList();
        var previousSelectedGameObject = newSelectedGameObject
            ?.Except(e.AddedItems.Cast<GameObject>())
            .Concat(e.RemovedItems.Cast<GameObject>())
            .ToList();

        Project.Current?.UndoRedo.Add(new UndoRedoCommand(
            $"Select GameObject",
            execute: () => RestoreSelection(listBox!, newSelectedGameObject),
            undo: () => RestoreSelection(listBox!, previousSelectedGameObject)));


        MSGameObject mSGameObject = null;
        if (newSelectedGameObject.Any())
        {
            mSGameObject = new MSGameObject(newSelectedGameObject);
        }
        InspectorView.Instance.DataContext = mSGameObject;
    }

    /// <summary>
    /// Safely restores a multi-selection on the ListBox.
    /// Skips items that are no longer present in the collection.
    /// </summary>
    private static void RestoreSelection(ListBox listBox, List<GameObject>? items)
    {
        listBox.SelectedItems.Clear();
        if (items == null) return;

        foreach (var item in items)
        {
            try { listBox.SelectedItems.Add(item); }
            catch (ArgumentOutOfRangeException) { /* item was removed from the scene */ }
        }
    }
}