using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluxionEditor.Foundation.Utilities;

namespace FluxionEditor;

public partial class LogView : UserControl
{
    public LogView()
    {
        InitializeComponent();
    }

    private void On_Clear_Button_CLick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Logger.Clear();
    }
}