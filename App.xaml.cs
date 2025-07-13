using System.Windows;
using RfemApp.ViewModels;
using RfemApplication.Services;
using RfemApplication.ViewModels;

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
            var mainWindow = new MainWindow();
            mainWindow.DataContext = new MainViewModel();
            mainWindow.Show();
        }

        private void ConfigureServices()
        {
            // Rejestracja serwisów w ServiceLocator
            ServiceLocator.Instance.RegisterSingleton<IDialogService>(new DialogService());
            ServiceLocator.Instance.RegisterSingleton<IRfemConnectionService>(new RfemConnectionService());
            ServiceLocator.Instance.RegisterSingleton<IEventAggregator>(new EventAggregator());
        }
    }
}