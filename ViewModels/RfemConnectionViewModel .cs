using System;
using System.Threading.Tasks;
using System.Windows.Input;
using RfemApplication.Services;
using RfemApplication.ViewModels.Base;
using RfemApplication.ViewModels.Commands;

namespace RfemApplication.ViewModels
{
    public class RfemConnectionViewModel : ViewModelBase
    {
        private readonly IRfemConnectionService _rfemService;
        private readonly IDialogService _dialogService;

        private string _serverUrl = "http://localhost:8081";
        private bool _isConnecting;
        private bool _isConnected;
        private string _connectionStatus = "Rozłączony";
        private string _lastError;
        private RfemApplicationInfo _applicationInfo;

        public RfemConnectionViewModel(IRfemConnectionService rfemService, IDialogService dialogService)
        {
            _rfemService = rfemService ?? throw new ArgumentNullException(nameof(rfemService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            InitializeCommands();
            UpdateConnectionStatus();
        }

        #region Properties

        public string ServerUrl
        {
            get => _serverUrl;
            set => SetProperty(ref _serverUrl, value);
        }

        public bool IsConnecting
        {
            get => _isConnecting;
            set => SetProperty(ref _isConnecting, value);
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }

        public string LastError
        {
            get => _lastError;
            set => SetProperty(ref _lastError, value);
        }

        public RfemApplicationInfo ApplicationInfo
        {
            get => _applicationInfo;
            set => SetProperty(ref _applicationInfo, value);
        }

        public bool CanConnect => !IsConnecting && !string.IsNullOrWhiteSpace(ServerUrl);
        public bool CanDisconnect => !IsConnecting && IsConnected;

        #endregion

        #region Commands

        public ICommand TestConnectionCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public ICommand DisconnectCommand { get; private set; }
        public ICommand ShowApplicationInfoCommand { get; private set; }

        private void InitializeCommands()
        {
            TestConnectionCommand = new AsyncRelayCommand(TestConnectionAsync, () => CanConnect);
            ConnectCommand = new AsyncRelayCommand(ConnectAsync, () => CanConnect);
            DisconnectCommand = new RelayCommand(Disconnect, () => CanDisconnect);
            ShowApplicationInfoCommand = new RelayCommand(ShowApplicationInfo, () => IsConnected && ApplicationInfo != null);
        }

        #endregion

        #region Command Methods

        private async Task TestConnectionAsync()
        {
            try
            {
                IsConnecting = true;
                ConnectionStatus = "Testowanie połączenia...";
                LastError = null;

                var result = await _rfemService.TestConnectionAsync(ServerUrl);

                if (result.IsSuccess)
                {
                    ConnectionStatus = "Test połączenia udany";
                    _dialogService.ShowInformationDialog(
                        $"Połączenie z RFEM udane!\n\n{result.ApplicationInfo}",
                        "Test połączenia");
                }
                else
                {
                    ConnectionStatus = "Test połączenia nieudany";
                    LastError = result.Message;
                    _dialogService.ShowErrorDialog(result.Message, "Błąd połączenia");
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = "Błąd podczas testowania";
                LastError = ex.Message;
                _dialogService.ShowErrorDialog($"Nieoczekiwany błąd: {ex.Message}", "Błąd");
            }
            finally
            {
                IsConnecting = false;
                UpdateCanExecuteStates();
            }
        }

        private async Task ConnectAsync()
        {
            try
            {
                IsConnecting = true;
                ConnectionStatus = "Łączenie...";
                LastError = null;

                var result = await _rfemService.ConnectAsync(ServerUrl);

                if (result.IsSuccess)
                {
                    IsConnected = true;
                    ApplicationInfo = result.ApplicationInfo;
                    ConnectionStatus = $"Połączony z {result.ApplicationInfo.ProductName}";

                    _dialogService.ShowInformationDialog(
                        $"Pomyślnie połączono z RFEM!\n\n{result.ApplicationInfo}",
                        "Połączenie nawiązane");
                }
                else
                {
                    IsConnected = false;
                    ApplicationInfo = null;
                    ConnectionStatus = "Błąd połączenia";
                    LastError = result.Message;

                    _dialogService.ShowErrorDialog(result.Message, "Błąd połączenia");
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
                ApplicationInfo = null;
                ConnectionStatus = "Błąd podczas łączenia";
                LastError = ex.Message;

                _dialogService.ShowErrorDialog($"Nieoczekiwany błąd: {ex.Message}", "Błąd");
            }
            finally
            {
                IsConnecting = false;
                UpdateCanExecuteStates();
            }
        }

        private void Disconnect()
        {
            try
            {
                _rfemService.Disconnect();
                IsConnected = false;
                ApplicationInfo = null;
                ConnectionStatus = "Rozłączony";
                LastError = null;

                _dialogService.ShowInformationDialog("Rozłączono z RFEM", "Rozłączenie");
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                _dialogService.ShowErrorDialog($"Błąd podczas rozłączania: {ex.Message}", "Błąd");
            }
            finally
            {
                UpdateCanExecuteStates();
            }
        }

        private void ShowApplicationInfo()
        {
            if (ApplicationInfo != null)
            {
                var info = $"Informacje o aplikacji RFEM:\n\n" +
                          $"Nazwa produktu: {ApplicationInfo.ProductName}\n" +
                          $"Wersja: {ApplicationInfo.ProductVersion}\n" +
                          $"Język: {ApplicationInfo.Language}\n" +
                          $"Adres serwera: {ApplicationInfo.ServerUrl}\n" +
                          $"Czas połączenia: {ApplicationInfo.ConnectionTime:yyyy-MM-dd HH:mm:ss}";

                _dialogService.ShowInformationDialog(info, "Informacje o RFEM");
            }
        }

        #endregion

        #region Private Methods

        private void UpdateConnectionStatus()
        {
            IsConnected = _rfemService.IsConnected;
            ConnectionStatus = IsConnected ? "Połączony" : "Rozłączony";
            LastError = _rfemService.LastError;

            if (IsConnected)
            {
                ApplicationInfo = _rfemService.GetApplicationInfo();
            }
        }

        private void UpdateCanExecuteStates()
        {
            OnPropertyChanged(nameof(CanConnect));
            OnPropertyChanged(nameof(CanDisconnect));

            // Wymuszenie odświeżenia komend
            ((RelayCommand)DisconnectCommand).RaiseCanExecuteChanged();
            ((AsyncRelayCommand)TestConnectionCommand).RaiseCanExecuteChanged();
            ((AsyncRelayCommand)ConnectCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ShowApplicationInfoCommand).RaiseCanExecuteChanged();
        }

        #endregion
    }
}