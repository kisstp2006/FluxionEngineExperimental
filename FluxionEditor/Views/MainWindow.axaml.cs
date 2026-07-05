using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace FluxionEditor.Views
{
    public partial class MainWindow : Window
    {
        private bool _isProjectManagerDialogOpen;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
        }

        private void OnMainWindowLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded; //We olnly need this once (TODO: Make it arg and project state aware)
            OpenProjectManagerDialog(0);

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

                if (!result)
                {
                    if (Application.Current?.ApplicationLifetime
                        is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.Shutdown();
                    }
                }
                else
                {
                    // Project selected / created successfully
                }
            }
            finally
            {
                _isProjectManagerDialogOpen = false;
            }

            
           
        }
    }
}