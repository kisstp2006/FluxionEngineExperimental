using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluxionEditor.Foundation;
using FluxionEditor.Foundation.Components;

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

        vm.AddGameObjectCommand.Execute(new GameObject(vm) {Name ="GameObject" });
    }
}