using System;
using System.Threading.Tasks;
using Dlubal.WS;
using Dlubal.WS.Rfem6.Application;
using Dlubal.WS.Rfem6.Model;

namespace RfemApplication.Services
{
    public interface IRfemConnectionService
    {
        Task<RfemConnectionResult> TestConnectionAsync(string serverUrl = "http://localhost:8081");
        Task<RfemConnectionResult> ConnectAsync(string serverUrl = "http://localhost:8081");
        void Disconnect();
        bool IsConnected { get; }
        string LastError { get; }
        RfemApplicationInfo GetApplicationInfo();
    }

    public class RfemConnectionService : IRfemConnectionService
    {
        private Dlubal.WS.Rfem6.Application.RfemApplicationClient _applicationClient;
        private RfemModelClient _modelClient;
        private bool _isConnected;
        private string _lastError;
        private string _currentServerUrl;
        private string _activeModelUrl;

        public bool IsConnected => _isConnected && _applicationClient != null;
        public string LastError => _lastError;

        public async Task<RfemConnectionResult> TestConnectionAsync(string serverUrl = "http://localhost:8081")
        {
            try
            {
                _lastError = null;

                // Utworzenie tymczasowego klienta aplikacji do testowania
                var testClient = new RfemApplicationClient(serverUrl);

                // Test połączenia - sprawdzenie informacji o aplikacji
                var appInfo = await Task.Run(() =>
                {
                    return testClient.get_information();
                });

                return new RfemConnectionResult
                {
                    IsSuccess = true,
                    Message = $"Połączenie udane z {appInfo.name} v{appInfo.version}",
                    ApplicationInfo = new RfemApplicationInfo
                    {
                        ProductName = appInfo.name,
                        ProductVersion = appInfo.version,
                        Language = appInfo.language_name,
                        ServerUrl = serverUrl
                    }
                };
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return new RfemConnectionResult
                {
                    IsSuccess = false,
                    Message = $"Błąd połączenia: {ex.Message}",
                    ErrorDetails = ex.ToString()
                };
            }
        }

        public async Task<RfemConnectionResult> ConnectAsync(string serverUrl = "http://localhost:8081")
        {
            try
            {
                // Rozłącz istniejące połączenie
                Disconnect();

                _lastError = null;
                _currentServerUrl = serverUrl;

                // Utworzenie klienta aplikacji
                _applicationClient = new RfemApplicationClient(serverUrl);

                // Test połączenia i pobranie informacji o aplikacji
                var appInfo = await Task.Run(() =>
                {
                    return _applicationClient.get_information();
                });

                // Sprawdzenie czy istnieje aktywny model
                try
                {
                    _activeModelUrl = await Task.Run(() =>
                    {
                        return _applicationClient.get_active_model();
                    });

                    // Jeśli istnieje aktywny model, połącz się z nim
                    if (!string.IsNullOrEmpty(_activeModelUrl))
                    {
                        _modelClient = new RfemModelClient(_activeModelUrl);
                    }
                }
                catch
                {
                    // Brak aktywnego modelu - to normalne
                    _activeModelUrl = null;
                    _modelClient = null;
                }

                _isConnected = true;

                var message = $"Połączono z {appInfo.name} v{appInfo.version}";
                if (!string.IsNullOrEmpty(_activeModelUrl))
                {
                    message += $"\nAktywny model: {_activeModelUrl}";
                }

                return new RfemConnectionResult
                {
                    IsSuccess = true,
                    Message = message,
                    ApplicationInfo = new RfemApplicationInfo
                    {
                        ProductName = appInfo.name,
                        ProductVersion = appInfo.version,
                        Language = appInfo.language_name,
                        ServerUrl = serverUrl,
                        ActiveModelUrl = _activeModelUrl
                    }
                };
            }
            catch (Exception ex)
            {
                _isConnected = false;
                _lastError = ex.Message;
                return new RfemConnectionResult
                {
                    IsSuccess = false,
                    Message = $"Błąd połączenia: {ex.Message}",
                    ErrorDetails = ex.ToString()
                };
            }
        }

        public void Disconnect()
        {
            try
            {
                // Zakończenie sesji modelu (jeśli istnieje)
                if (_modelClient != null)
                {
                    try
                    {
                        _modelClient.close_connection();
                    }
                    catch
                    {
                        // Ignoruj błędy przy zamykaniu modelu
                    }
                    _modelClient = null;
                }

                // Zakończenie sesji aplikacji
                if (_applicationClient != null)
                {
                    try
                    {
                        _applicationClient.close_application();
                    }
                    catch
                    {
                        // Ignoruj błędy przy zamykaniu aplikacji
                    }
                    _applicationClient = null;
                }

                _activeModelUrl = null;
            }
            catch (Exception ex)
            {
                _lastError = $"Błąd podczas rozłączania: {ex.Message}";
            }
            finally
            {
                _isConnected = false;
            }
        }

        public RfemApplicationInfo GetApplicationInfo()
        {
            if (!IsConnected)
                return null;

            try
            {
                var appInfo = _applicationClient.get_information();

                // Sprawdź czy jest aktywny model
                string currentActiveModel = null;
                try
                {
                    currentActiveModel = _applicationClient.get_active_model();
                }
                catch
                {
                    // Brak aktywnego modelu
                }

                return new RfemApplicationInfo
                {
                    ProductName = appInfo.name,
                    ProductVersion = appInfo.version,
                    Language = appInfo.language_name,
                    ServerUrl = _currentServerUrl,
                    ActiveModelUrl = currentActiveModel
                };
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Uzyskuje dostęp do aktywnego modelu
        /// </summary>
        /// <returns>Klient modelu lub null jeśli brak aktywnego modelu</returns>
        public RfemModelClient GetActiveModelClient()
        {
            if (!IsConnected)
                return null;

            try
            {
                var activeModelUrl = _applicationClient.get_active_model();
                if (!string.IsNullOrEmpty(activeModelUrl))
                {
                    return new RfemModelClient(activeModelUrl);
                }
            }
            catch
            {
                // Brak aktywnego modelu
            }

            return null;
        }
    }

    public class RfemConnectionResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ErrorDetails { get; set; }
        public RfemApplicationInfo ApplicationInfo { get; set; }
    }

    public class RfemApplicationInfo
    {
        public string ProductName { get; set; }
        public string ProductVersion { get; set; }
        public string Language { get; set; }
        public string ServerUrl { get; set; }
        public string ActiveModelUrl { get; set; }
        public DateTime ConnectionTime { get; set; } = DateTime.Now;

        public override string ToString()
        {
            var result = $"{ProductName} v{ProductVersion} ({Language})\nSerwer: {ServerUrl}";
            if (!string.IsNullOrEmpty(ActiveModelUrl))
            {
                result += $"\nAktywny model: {ActiveModelUrl}";
            }
            return result;
        }
    }
}