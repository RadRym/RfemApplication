using System;
using System.Threading.Tasks;
using Dlubal.WS;

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
        private ApplicationClient _client;
        private bool _isConnected;
        private string _lastError;
        private string _currentServerUrl;

        public bool IsConnected => _isConnected && _client != null;
        public string LastError => _lastError;

        public async Task<RfemConnectionResult> TestConnectionAsync(string serverUrl = "http://localhost:8081")
        {
            try
            {
                _lastError = null;

                // Utworzenie tymczasowego klienta do testowania
                var testClient = new ApplicationClient(serverUrl);

                // Test połączenia - sprawdzenie informacji o aplikacji
                var appInfo = await Task.Run(() => testClient.get_information());

                return new RfemConnectionResult
                {
                    IsSuccess = true,
                    Message = $"Połączenie udane z RFEM {appInfo.product_name} v{appInfo.product_version}",
                    ApplicationInfo = new RfemApplicationInfo
                    {
                        ProductName = appInfo.product_name,
                        ProductVersion = appInfo.product_version,
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

                // Utworzenie klienta
                _client = new ApplicationClient(serverUrl);

                // Test połączenia
                var appInfo = await Task.Run(() => _client.get_information());

                _isConnected = true;

                return new RfemConnectionResult
                {
                    IsSuccess = true,
                    Message = $"Połączono z RFEM {appInfo.product_name} v{appInfo.product_version}",
                    ApplicationInfo = new RfemApplicationInfo
                    {
                        ProductName = appInfo.product_name,
                        ProductVersion = appInfo.product_version,
                        Language = appInfo.language_name,
                        ServerUrl = serverUrl
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
                if (_client != null)
                {
                    // Zakończenie sesji (jeśli API to obsługuje)
                    _client = null;
                }
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
                var appInfo = _client.get_information();
                return new RfemApplicationInfo
                {
                    ProductName = appInfo.product_name,
                    ProductVersion = appInfo.product_version,
                    Language = appInfo.language_name,
                    ServerUrl = _currentServerUrl
                };
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return null;
            }
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
        public DateTime ConnectionTime { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"{ProductName} v{ProductVersion} ({Language}) - {ServerUrl}";
        }
    }
}