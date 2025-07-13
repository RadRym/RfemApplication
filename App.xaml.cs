using System.Windows;
using RfemApplication.Services;
using RfemApp.ViewModels; // Poprawny namespace dla MainViewModel

namespace RfemApplication
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Konfiguracja serwisów
            ConfigureServices();

            // Utworzenie głównego okna z ViewModelem
            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            mainWindow.Show();
        }

        private void ConfigureServices()
        {
            // Rejestracja serwisów w ServiceLocator
            ServiceLocator.Instance.RegisterSingleton<IDialogService>(new DialogService());
            ServiceLocator.Instance.RegisterSingleton<IRfemConnectionService>(new RfemConnectionService());
            ServiceLocator.Instance.RegisterSingleton<IEventAggregator>(new EventAggregator());
            ServiceLocator.Instance.RegisterSingleton<IRfemServerManager>(new RfemServerManager());
        }
    }
}