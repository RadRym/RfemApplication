using System;
using System.ServiceModel;
using System.Threading.Tasks;
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
        string GetActiveModelUrl();
        IRfemModel GetModelClient();
    }

    public class RfemConnectionService : IRfemConnectionService
    {
        private IRfemApplication _applicationClient;
        private IRfemModel _modelClient;
        private ChannelFactory<IRfemApplication> _applicationChannelFactory;
        private ChannelFactory<IRfemModel> _modelChannelFactory;
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

                // Zwiększ timeout dla operacji
                binding.OpenTimeout = TimeSpan.FromSeconds(10);
                binding.CloseTimeout = TimeSpan.FromSeconds(10);
                binding.SendTimeout = TimeSpan.FromSeconds(30);
                binding.ReceiveTimeout = TimeSpan.FromSeconds(30);

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
                    Message = $"Połączenie udane z RFEM Server",
                    ApplicationInfo = new RfemApplicationInfo
                    {
                        ProductName = "RFEM",
                        ProductVersion = appInfo?.value?.name ?? "6.x",
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

                // Utworzenie endpoint i binding dla aplikacji
                var appEndpoint = new EndpointAddress(serverUrl);
                var binding = new BasicHttpBinding();

                // Zwiększ timeout dla operacji
                binding.OpenTimeout = TimeSpan.FromSeconds(30);
                binding.CloseTimeout = TimeSpan.FromSeconds(30);
                binding.SendTimeout = TimeSpan.FromSeconds(60);
                binding.ReceiveTimeout = TimeSpan.FromSeconds(60);

                // Utworzenie klienta aplikacji
                _applicationChannelFactory = new ChannelFactory<IRfemApplication>(binding, appEndpoint);
                _applicationClient = _applicationChannelFactory.CreateChannel();

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
                        return response?.value;
                    });

                    // Jeśli jest aktywny model, połącz się z nim
                    if (!string.IsNullOrEmpty(_activeModelUrl))
                    {
                        await ConnectToModelAsync(_activeModelUrl);
                    }
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
                else
                {
                    message += "\nBrak aktywnego modelu";
                }

                return new RfemConnectionResult
                {
                    IsSuccess = true,
                    Message = message,
                    ApplicationInfo = new RfemApplicationInfo
                    {
                        ProductName = "RFEM",
                        ProductVersion = appInfo?.value?.name ?? "6.x",
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

        private async Task ConnectToModelAsync(string modelUrl)
        {
            try
            {
                var modelEndpoint = new EndpointAddress(modelUrl);
                var binding = new BasicHttpBinding();

                // Zwiększ timeout dla operacji modelu
                binding.OpenTimeout = TimeSpan.FromSeconds(30);
                binding.CloseTimeout = TimeSpan.FromSeconds(30);
                binding.SendTimeout = TimeSpan.FromSeconds(60);
                binding.ReceiveTimeout = TimeSpan.FromSeconds(60);

                _modelChannelFactory = new ChannelFactory<IRfemModel>(binding, modelEndpoint);
                _modelClient = _modelChannelFactory.CreateChannel();

                // Test połączenia z modelem
                await Task.Run(() =>
                {
                    var request = new Dlubal.WS.Rfem6.Model.get_session_idRequest();
                    var response = _modelClient.get_session_id(request);
                    return response;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd połączenia z modelem: {ex.Message}");
                _modelClient = null;
                if (_modelChannelFactory != null)
                {
                    try
                    {
                        _modelChannelFactory.Close();
                    }
                    catch
                    {
                        _modelChannelFactory.Abort();
                    }
                    _modelChannelFactory = null;
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                // Zamknij model client
                if (_modelClient != null)
                {
                    try
                    {
                        var channel = _modelClient as IClientChannel;
                        if (channel?.State == CommunicationState.Opened)
                        {
                            channel.Close();
                        }
                    }
                    catch
                    {
                        var channel = _modelClient as IClientChannel;
                        channel?.Abort();
                    }
                    _modelClient = null;
                }

                if (_modelChannelFactory != null)
                {
                    try
                    {
                        if (_modelChannelFactory.State == CommunicationState.Opened)
                        {
                            _modelChannelFactory.Close();
                        }
                    }
                    catch
                    {
                        _modelChannelFactory.Abort();
                    }
                    _modelChannelFactory = null;
                }

                // Zamknij application client
                if (_applicationClient != null)
                {
                    try
                    {
                        var channel = _applicationClient as IClientChannel;
                        if (channel?.State == CommunicationState.Opened)
                        {
                            channel.Close();
                        }
                    }
                    catch
                    {
                        var channel = _applicationClient as IClientChannel;
                        channel?.Abort();
                    }
                    _applicationClient = null;
                }

                if (_applicationChannelFactory != null)
                {
                    try
                    {
                        if (_applicationChannelFactory.State == CommunicationState.Opened)
                        {
                            _applicationChannelFactory.Close();
                        }
                    }
                    catch
                    {
                        _applicationChannelFactory.Abort();
                    }
                    _applicationChannelFactory = null;
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
                string currentActiveModel = GetActiveModelUrl();

                return new RfemApplicationInfo
                {
                    ProductName = "RFEM",
                    ProductVersion = appInfo?.value?.name ?? "6.x",
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

        public string GetActiveModelUrl()
        {
            if (!IsConnected)
                return null;

            try
            {
                var request = new get_active_modelRequest();
                var response = _applicationClient.get_active_model(request);

                string newActiveModelUrl = response?.value;

                // Jeśli aktywny model się zmienił, połącz się z nowym
                if (newActiveModelUrl != _activeModelUrl && !string.IsNullOrEmpty(newActiveModelUrl))
                {
                    _activeModelUrl = newActiveModelUrl;
                    Task.Run(() => ConnectToModelAsync(_activeModelUrl));
                }

                return newActiveModelUrl;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return null;
            }
        }

        public IRfemModel GetModelClient()
        {
            // Sprawdź czy jest aktywny model i czy jesteśmy połączeni
            if (_modelClient == null && !string.IsNullOrEmpty(_activeModelUrl))
            {
                Task.Run(() => ConnectToModelAsync(_activeModelUrl)).Wait();
            }

            return _modelClient;
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
            else
            {
                result += "\nBrak aktywnego modelu";
            }
            return result;
        }
    }
}