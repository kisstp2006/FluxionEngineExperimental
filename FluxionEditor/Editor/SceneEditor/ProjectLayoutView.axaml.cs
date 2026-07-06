using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluxionEditor.Foundation;

namespace FluxionEditor;

public partial class ProjectLayoutView : UserControl
{
    public ProjectLayoutView()
    {
        InitializeComponent();
    }

    private void Add_New_Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = DataContext as Project;
        vm?.AddScene.Execute("New Scene " + vm.Scenes.Count);
    }
}