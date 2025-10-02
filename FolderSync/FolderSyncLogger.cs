using System.Diagnostics;

namespace FolderSync
{
    internal class FolderSyncLogger
    {
        private const string CurrentDir = ".";
        private readonly string DefaultLogFile = $"foldersync_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

        public FolderSyncLogger(string? logFilePath = null)
        {
            string logPath = string.IsNullOrWhiteSpace(logFilePath)
                ? Path.Combine(CurrentDir, DefaultLogFile)
                : logFilePath;

            string? directory = Path.GetDirectoryName(logPath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            Trace.Listeners.Clear();
            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.Listeners.Add(new TextWriterTraceListener(logPath));
            Trace.AutoFlush = true;
        }

        public void LogInfo(string message)  
        {
            Log($"INFO: {message}");
        }

        public void LogWarning(string message)
        {
            Log($"WARNING: {message}");
        }

        public void LogError(string message)
        {
            Log($"ERROR: {message}");
        }
        private void Log(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Trace.WriteLine(logMessage);
        }
    }
}
