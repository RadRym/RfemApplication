using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Dlubal.WS.Rfem6.Application;

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
        private IRfemApplication _applicationClient;
        private ChannelFactory<IRfemApplication> _channelFactory;
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

                // Utworzenie endpoint dla SOAP WebService
                var endpoint = new EndpointAddress(serverUrl);
                var binding = new BasicHttpBinding();

                // Utworzenie klienta przez ChannelFactory
                var channelFactory = new ChannelFactory<IRfemApplication>(binding, endpoint);
                var testClient = channelFactory.CreateChannel();

                // Test połączenia - sprawdzenie informacji o aplikacji
                var appInfo = await Task.Run(() =>
                {
                    var request = new get_informationRequest();
                    var response = testClient.get_information(request);
                    return response;
                });

                // Zamknij kanał testowy
                try
                {
                    ((IClientChannel)testClient).Close();
                    channelFactory.Close();
                }
                catch
                {
                    ((IClientChannel)testClient).Abort();
                    channelFactory.Abort();
                }

                return new RfemConnectionResult
                {
                    IsSuccess = true,
                    Message = $"Połączenie udane z RFEM",
                    ApplicationInfo = new RfemApplicationInfo
                    {
                        ProductName = "RFEM",
                        ProductVersion = "6.10",
                        Language = "Polish",
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

                // Utworzenie endpoint i binding
                var endpoint = new EndpointAddress(serverUrl);
                var binding = new BasicHttpBinding();

                // Zwiększ timeout dla operacji
                binding.OpenTimeout = TimeSpan.FromSeconds(30);
                binding.CloseTimeout = TimeSpan.FromSeconds(30);
                binding.SendTimeout = TimeSpan.FromSeconds(60);
                binding.ReceiveTimeout = TimeSpan.FromSeconds(60);

                // Utworzenie klienta
                _channelFactory = new ChannelFactory<IRfemApplication>(binding, endpoint);
                _applicationClient = _channelFactory.CreateChannel();

                // Test połączenia i pobranie informacji o aplikacji
                var appInfo = await Task.Run(() =>
                {
                    var request = new get_informationRequest();
                    var response = _applicationClient.get_information(request);
                    return response;
                });

                // Sprawdzenie czy istnieje aktywny model
                try
                {
                    _activeModelUrl = await Task.Run(() =>
                    {
                        var request = new get_active_modelRequest();
                        var response = _applicationClient.get_active_model(request);
                        return response?.value; // Response może mieć właściwość value
                    });
                }
                catch
                {
                    // Brak aktywnego modelu - to normalne
                    _activeModelUrl = null;
                }

                _isConnected = true;

                var message = "Połączono z RFEM Server";
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
                        ProductName = "RFEM",
                        ProductVersion = "6.10",
                        Language = "Polish",
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
                if (_applicationClient != null)
                {
                    try
                    {
                        // Zamknij kanał komunikacyjny
                        var channel = _applicationClient as IClientChannel;
                        if (channel?.State == CommunicationState.Opened)
                        {
                            channel.Close();
                        }
                    }
                    catch
                    {
                        // Ignoruj błędy przy zamykaniu
                        var channel = _applicationClient as IClientChannel;
                        channel?.Abort();
                    }
                    _applicationClient = null;
                }

                if (_channelFactory != null)
                {
                    try
                    {
                        if (_channelFactory.State == CommunicationState.Opened)
                        {
                            _channelFactory.Close();
                        }
                    }
                    catch
                    {
                        _channelFactory.Abort();
                    }
                    _channelFactory = null;
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
                var request = new get_informationRequest();
                var appInfo = _applicationClient.get_information(request);

                // Sprawdź czy jest aktywny model
                string currentActiveModel = null;
                try
                {
                    var modelRequest = new get_active_modelRequest();
                    var modelResponse = _applicationClient.get_active_model(modelRequest);
                    currentActiveModel = modelResponse?.value;
                }
                catch
                {
                    // Brak aktywnego modelu
                }

                return new RfemApplicationInfo
                {
                    ProductName = "RFEM",
                    ProductVersion = "6.10",
                    Language = "Polish",
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
        /// <returns>URL aktywnego modelu lub null</returns>
        public string GetActiveModelUrl()
        {
            if (!IsConnected)
                return null;

            try
            {
                var request = new get_active_modelRequest();
                var response = _applicationClient.get_active_model(request);
                return response?.value;
            }
            catch
            {
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