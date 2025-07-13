using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RfemApplication.Services
{
    public interface IRfemServerManager
    {
        Task<RfemServerResult> StartServerAsync(RfemServerConfig config);
        Task<bool> StopServerAsync();
        bool IsServerRunning();
        string GetServerStatus();
    }

    public class RfemServerManager : IRfemServerManager
    {
        private Process _serverProcess;
        private string _lastError;

        public async Task<RfemServerResult> StartServerAsync(RfemServerConfig config)
        {
            try
            {
                // Sprawdź czy server już działa
                if (IsServerRunning())
                {
                    return new RfemServerResult
                    {
                        IsSuccess = false,
                        Message = "RFEM Server już działa"
                    };
                }

                // Sprawdź czy plik RFEM6Server.exe istnieje
                if (!File.Exists(config.ServerExecutablePath))
                {
                    return new RfemServerResult
                    {
                        IsSuccess = false,
                        Message = $"Nie znaleziono pliku: {config.ServerExecutablePath}"
                    };
                }

                // Przygotuj argumenty
                var arguments = BuildServerArguments(config);

                // Uruchom proces
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = config.ServerExecutablePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(config.ServerExecutablePath)
                };

                _serverProcess = new Process { StartInfo = processStartInfo };

                // Obsługa zdarzeń
                _serverProcess.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Debug.WriteLine($"RFEM Server: {e.Data}");
                    }
                };

                _serverProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _lastError = e.Data;
                        Debug.WriteLine($"RFEM Server Error: {e.Data}");
                    }
                };

                // Uruchom proces
                var started = _serverProcess.Start();
                if (!started)
                {
                    return new RfemServerResult
                    {
                        IsSuccess = false,
                        Message = "Nie udało się uruchomić RFEM Server"
                    };
                }

                _serverProcess.BeginOutputReadLine();
                _serverProcess.BeginErrorReadLine();

                // Poczekaj chwilę na uruchomienie
                await Task.Delay(3000);

                // Sprawdź czy proces nadal działa
                if (_serverProcess.HasExited)
                {
                    return new RfemServerResult
                    {
                        IsSuccess = false,
                        Message = $"RFEM Server zakończył się z kodem: {_serverProcess.ExitCode}. Błąd: {_lastError}"
                    };
                }

                return new RfemServerResult
                {
                    IsSuccess = true,
                    Message = $"RFEM Server uruchomiony pomyślnie na porcie {config.SoapPort}",
                    ProcessId = _serverProcess.Id
                };
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return new RfemServerResult
                {
                    IsSuccess = false,
                    Message = $"Błąd uruchamiania serwera: {ex.Message}",
                    ErrorDetails = ex.ToString()
                };
            }
        }

        public async Task<bool> StopServerAsync()
        {
            try
            {
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    _serverProcess.Kill();
                    await Task.Run(() => _serverProcess.WaitForExit(5000));
                    _serverProcess.Dispose();
                    _serverProcess = null;
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }
        }

        public bool IsServerRunning()
        {
            return _serverProcess != null && !_serverProcess.HasExited;
        }

        public string GetServerStatus()
        {
            if (_serverProcess == null)
                return "Nie uruchomiony";

            if (_serverProcess.HasExited)
                return $"Zakończony (kod: {_serverProcess.ExitCode})";

            return $"Działa (PID: {_serverProcess.Id})";
        }

        private string BuildServerArguments(RfemServerConfig config)
        {
            var args = new List<string>();

            if (!string.IsNullOrEmpty(config.Email))
                args.Add($"{config.Email}");

            if (!string.IsNullOrEmpty(config.Password))
                args.Add($"--password={config.Password}");

            if (!string.IsNullOrEmpty(config.License))
                args.Add($"--license={config.License}");

            args.Add($"--start-soap-server={config.SoapPort}");
            args.Add($"--soap-number-of-model-server-ports={config.NumberOfModelPorts}");

            return string.Join(" ", args);
        }
    }

    public class RfemServerConfig
    {
        public string ServerExecutablePath { get; set; } = @"C:\Program Files\Dlubal\RFEM 6.10\bin\RFEM6Server.exe";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string License { get; set; } = "";
        public int SoapPort { get; set; } = 8081;
        public int NumberOfModelPorts { get; set; } = 10;

        // Alternatywnie - uruchamianie GUI z WebService
        public bool UseGuiMode { get; set; } = false;
        public string GuiExecutablePath { get; set; } = @"C:\Program Files\Dlubal\RFEM 6.10\bin\RFEM6.exe";

        /// <summary>
        /// Automatyczne wykrywanie ścieżki RFEM
        /// </summary>
        public static RfemServerConfig AutoDetect()
        {
            var config = new RfemServerConfig();

            // Sprawdź różne możliwe lokalizacje RFEM
            var possiblePaths = new[]
            {
                @"C:\Program Files\Dlubal\RFEM 6.10\bin\",
                @"C:\Program Files\Dlubal\RFEM 6.09\bin\",
                @"C:\Program Files\Dlubal\RFEM 6.08\bin\",
                @"C:\Program Files (x86)\Dlubal\RFEM 6.10\bin\",
                @"C:\Program Files (x86)\Dlubal\RFEM 6.09\bin\"
            };

            foreach (var path in possiblePaths)
            {
                var serverPath = Path.Combine(path, "RFEM6Server.exe");
                var guiPath = Path.Combine(path, "RFEM6.exe");

                if (File.Exists(serverPath))
                {
                    config.ServerExecutablePath = serverPath;
                    config.GuiExecutablePath = guiPath;
                    break;
                }
            }

            return config;
        }
    }

    public class RfemServerResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ErrorDetails { get; set; }
        public int ProcessId { get; set; }
    }
}