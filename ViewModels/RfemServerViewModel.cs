using System;
using System.Threading.Tasks;
using System.Windows.Input;
using RfemApplication.Services;
using RfemApplication.ViewModels.Base;
using RfemApplication.ViewModels.Commands;

namespace RfemApplication.ViewModels
{
    public class RfemServerViewModel : ViewModelBase
    {
        private readonly IRfemServerManager _serverManager;
        private readonly IDialogService _dialogService;

        private RfemServerConfig _serverConfig;
        private bool _isServerRunning;
        private bool _isStarting;
        private string _serverStatus = "Nie uruchomiony";
        private string _lastError;

        public RfemServerViewModel(IRfemServerManager serverManager, IDialogService dialogService)
        {
            _serverManager = serverManager ?? throw new ArgumentNullException(nameof(serverManager));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Automatyczne wykrywanie konfiguracji RFEM
            ServerConfig = RfemServerConfig.AutoDetect();

            InitializeCommands();
            UpdateServerStatus();
        }

        #region Properties

        public RfemServerConfig ServerConfig
        {
            get => _serverConfig;
            set => SetProperty(ref _serverConfig, value);
        }

        public bool IsServerRunning
        {
            get => _isServerRunning;
            set => SetProperty(ref _isServerRunning, value);
        }

        public bool IsStarting
        {
            get => _isStarting;
            set => SetProperty(ref _isStarting, value);
        }

        public string ServerStatus
        {
            get => _serverStatus;
            set => SetProperty(ref _serverStatus, value);
        }

        public string LastError
        {
            get => _lastError;
            set => SetProperty(ref _lastError, value);
        }

        // Właściwości do bindowania konfiguracji
        public string Email
        {
            get => ServerConfig?.Email ?? "";
            set
            {
                if (ServerConfig != null)
                {
                    ServerConfig.Email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get => ServerConfig?.Password ?? "";
            set
            {
                if (ServerConfig != null)
                {
                    ServerConfig.Password = value;
                    OnPropertyChanged();
                }
            }
        }

        public string License
        {
            get => ServerConfig?.License ?? "";
            set
            {
                if (ServerConfig != null)
                {
                    ServerConfig.License = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SoapPort
        {
            get => ServerConfig?.SoapPort ?? 8081;
            set
            {
                if (ServerConfig != null)
                {
                    ServerConfig.SoapPort = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ServerExecutablePath
        {
            get => ServerConfig?.ServerExecutablePath ?? "";
            set
            {
                if (ServerConfig != null)
                {
                    ServerConfig.ServerExecutablePath = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool UseGuiMode
        {
            get => ServerConfig?.UseGuiMode ?? false;
            set
            {
                if (ServerConfig != null)
                {
                    ServerConfig.UseGuiMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ServerModeDescription));
                }
            }
        }

        public string ServerModeDescription =>
            UseGuiMode ? "RFEM z interfejsem + WebService" : "RFEM Server (bez interfejsu)";

        // Właściwości dla UI
        public bool CanStartServer => !IsServerRunning && !IsStarting;
        public bool CanStopServer => IsServerRunning && !IsStarting;

        #endregion

        #region Commands

        public ICommand StartServerCommand { get; private set; }
        public ICommand StopServerCommand { get; private set; }
        public ICommand BrowseServerPathCommand { get; private set; }
        public ICommand RefreshStatusCommand { get; private set; }

        private void InitializeCommands()
        {
            StartServerCommand = new AsyncRelayCommand(StartServerAsync, () => CanStartServer);
            StopServerCommand = new AsyncRelayCommand(StopServerAsync, () => CanStopServer);
            BrowseServerPathCommand = new RelayCommand(BrowseServerPath);
            RefreshStatusCommand = new RelayCommand(UpdateServerStatus);
        }

        #endregion

        #region Command Methods

        private async Task StartServerAsync()
        {
            try
            {
                IsStarting = true;
                LastError = null;
                ServerStatus = "Uruchamianie...";

                // Walidacja konfiguracji
                if (string.IsNullOrEmpty(ServerConfig.ServerExecutablePath))
                {
                    _dialogService.ShowErrorDialog(
                        "Nie znaleziono ścieżki do RFEM6Server.exe.\nSprawdź ścieżkę instalacji RFEM.",
                        "Błąd konfiguracji");
                    return;
                }

                // Uruchom serwer
                var result = await _serverManager.StartServerAsync(ServerConfig);

                if (result.IsSuccess)
                {
                    IsServerRunning = true;
                    ServerStatus = result.Message;

                    _dialogService.ShowInformationDialog(
                        $"{result.Message}\n\nTeraz możesz połączyć się z RFEM przez WebService API.",
                        "RFEM Server uruchomiony");
                }
                else
                {
                    LastError = result.Message;
                    ServerStatus = "Błąd uruchomienia";

                    _dialogService.ShowErrorDialog(
                        $"Nie udało się uruchomić RFEM Server:\n\n{result.Message}\n\n" +
                        "Sprawdź:\n" +
                        "• Czy RFEM jest zainstalowany\n" +
                        "• Czy podano prawidłowe dane logowania\n" +
                        "• Czy port 8081 nie jest zajęty",
                        "Błąd uruchomienia serwera");
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                ServerStatus = "Błąd";
                _dialogService.ShowErrorDialog($"Nieoczekiwany błąd: {ex.Message}", "Błąd");
            }
            finally
            {
                IsStarting = false;
                UpdateCanExecuteStates();
            }
        }

        private async Task StopServerAsync()
        {
            try
            {
                IsStarting = true;
                ServerStatus = "Zatrzymywanie...";

                var stopped = await _serverManager.StopServerAsync();

                if (stopped)
                {
                    IsServerRunning = false;
                    ServerStatus = "Zatrzymany";
                    LastError = null;

                    _dialogService.ShowInformationDialog(
                        "RFEM Server został zatrzymany.",
                        "Serwer zatrzymany");
                }
                else
                {
                    ServerStatus = "Błąd zatrzymania";
                    _dialogService.ShowErrorDialog(
                        "Nie udało się zatrzymać RFEM Server.",
                        "Błąd");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Błąd zatrzymywania: {ex.Message}", "Błąd");
            }
            finally
            {
                IsStarting = false;
                UpdateCanExecuteStates();
            }
        }

        private void BrowseServerPath()
        {
            var filePath = _dialogService.ShowOpenFileDialog(
                "Pliki wykonywalne (*.exe)|*.exe",
                "Wybierz RFEM6Server.exe");

            if (!string.IsNullOrEmpty(filePath))
            {
                ServerExecutablePath = filePath;
            }
        }

        private void UpdateServerStatus()
        {
            IsServerRunning = _serverManager.IsServerRunning();
            ServerStatus = _serverManager.GetServerStatus();
            UpdateCanExecuteStates();
        }

        #endregion

        #region Private Methods

        private void UpdateCanExecuteStates()
        {
            OnPropertyChanged(nameof(CanStartServer));
            OnPropertyChanged(nameof(CanStopServer));

            // Wymuszenie odświeżenia komend
            ((AsyncRelayCommand)StartServerCommand).RaiseCanExecuteChanged();
            ((AsyncRelayCommand)StopServerCommand).RaiseCanExecuteChanged();
        }

        #endregion
    }
}