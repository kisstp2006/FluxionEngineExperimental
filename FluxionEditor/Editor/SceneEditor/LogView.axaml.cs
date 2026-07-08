using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluxionEditor.Foundation.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FluxionEditor;

public partial class LogView : UserControl
{
    private readonly ObservableCollection<MessageLog> _displayedMessages = new();

    public LogView()
    {
        InitializeComponent();

        LogItemsControl.ItemsSource = _displayedMessages;
        RebuildDisplayedMessages();

        Logger.FiltersChanged += OnFiltersChanged;
        ((INotifyCollectionChanged)Logger.Messages).CollectionChanged += (_, _) => RebuildDisplayedMessages();
    }

    private void OnClearButtonClick(object? sender, RoutedEventArgs e)
    {
        Logger.Clear();
    }

    /// <summary>
    /// Updates the log filter bitmask based on which checkboxes are checked.
    /// Info=0x01, Warning=0x02, Error=0x04.
    /// </summary>
    private void OnFilterCheckBoxClick(object? sender, RoutedEventArgs e)
    {
        int mask = 0;
        if (InfoCheckBox.IsChecked == true)    mask |= (int)SeverityLevel.Info;
        if (WarningCheckBox.IsChecked == true) mask |= (int)SeverityLevel.Warning;
        if (ErrorCheckBox.IsChecked == true)   mask |= (int)SeverityLevel.Error;

        Logger.MessageFilter = mask;
    }

    private void OnFiltersChanged()
    {
        RebuildDisplayedMessages();
    }

    private void RebuildDisplayedMessages()
    {
        _displayedMessages.Clear();
        foreach (var msg in Logger.FilteredMessages)
            _displayedMessages.Add(msg);
    }
}