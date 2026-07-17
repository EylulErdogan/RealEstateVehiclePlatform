namespace RealEstateVehiclePlatform.WebUI.Services
{
    public class LogService
    {
        private readonly IWebHostEnvironment _env;
        private readonly object _lock = new();

        public LogService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void Log(string level, string message, string? username = null)
        {
            lock (_lock)
            {
                try
                {
                    var logFolder = Path.Combine(_env.WebRootPath, "logs");
                    Directory.CreateDirectory(logFolder);

                    var logPath = Path.Combine(logFolder, "system_logs.txt");
                    var userStr = string.IsNullOrEmpty(username) ? "System/Guest" : username;
                    var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level.ToUpper()}] [User: {userStr}] {message}{Environment.NewLine}";

                    System.IO.File.AppendAllText(logPath, logLine);
                }
                catch
                {
                    // Loglama sırasındaki bir hata ana sistem akışını kesmesin.
                }
            }
        }

        public void LogInfo(string message, string? username = null) => Log("INFO", message, username);
        public void LogWarning(string message, string? username = null) => Log("WARNING", message, username);
        public void LogError(string message, Exception? ex = null, string? username = null)
        {
            var errMsg = ex != null ? $"{message} - Hata: {ex.Message} {ex.StackTrace}" : message;
            Log("ERROR", errMsg, username);
        }
    }
}