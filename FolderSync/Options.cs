namespace FolderSync
{
    internal class Options
    {
        public string SourceFolder { get; }
        public string ReplicaFolder { get;}
        public int SyncIntervalSeconds { get; }
        public string? LogFilePath { get; }

        private Options(string sourceFolder, string replicaFolder, int syncIntervalSeconds, string? logFilePath )
        {
            SourceFolder = sourceFolder;
            ReplicaFolder = replicaFolder;
            SyncIntervalSeconds = syncIntervalSeconds;
            LogFilePath = logFilePath;
        }
        public static Options FromArgs(string[] args)
        {
            if (args.Length < 3 || args.Length > 4)
                throw new ArgumentException("Usage: foldersync [source folder] [replica folder] [sync interval in seconds] [log file path(optional)]");

            string sourceFolder = args[0];
            string replicaFolder = args[1];
            string syncIntervalInput = args[2];
            string? logFile = args.Length == 4 ? args[3] : null;

            if (string.IsNullOrWhiteSpace(sourceFolder))
                throw new ArgumentException("Source folder must be specified.");

            if (string.IsNullOrWhiteSpace(replicaFolder))
                throw new ArgumentException("Replica folder must be specified.");

            if (!int.TryParse(syncIntervalInput, out int syncIntervalSeconds))
                throw new ArgumentException($"Sync interval must be a number: {syncIntervalInput}");

            if (syncIntervalSeconds <= 0)
                throw new ArgumentException("Sync interval must be greater than 0.");

            return new Options(sourceFolder, replicaFolder, syncIntervalSeconds, logFile);
        }
    }
}
