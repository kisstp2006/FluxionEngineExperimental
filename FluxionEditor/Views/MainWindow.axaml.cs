using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using FluxionEditor.Foundation;
using System;

namespace FluxionEditor.Views
{
    public partial class MainWindow : Window
    {
        private bool _isProjectManagerDialogOpen;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;
        }

        private void OnMainWindowLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded; //We olnly need this once (TODO: Make it arg and project state aware)
            OpenProjectManagerDialog(0);

        }

        private void OnMainWindowClosing(object? sender, WindowClosingEventArgs e)
        {
            Closing -= OnMainWindowClosing;
            Project.Current?.Unload();
        }

        private async void OpenProjectManagerDialog(int wichPage=0)
        {
            if (_isProjectManagerDialogOpen)
                return;

            _isProjectManagerDialogOpen = true;

            try
            {
                ProjectManagerDialog projectManagerDialog = new ProjectManagerDialog();

                bool result = await projectManagerDialog.ShowDialog<bool>(this);

                if (!result || projectManagerDialog.DataContext == null)
                {
                    if (Application.Current?.ApplicationLifetime
                        is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.Shutdown();
                    }
                }
                else
                {
                    Project.Current?.Unload();
                    DataContext = projectManagerDialog.DataContext;
                }
            }
            finally
            {
                _isProjectManagerDialogOpen = false;
            }

            
           
        }
    }
}