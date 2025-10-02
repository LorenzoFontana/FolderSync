namespace FolderSync
{
    class FolderSync
    {
        static void Main(string[] args)
        {
            FolderSyncLogger? logger = null;

            try
            {
                var options = Options.FromArgs(args);
                
                logger = new FolderSyncLogger(options.LogFilePath);

                Console.CancelKeyPress += (sender, e) =>
                {
                    logger.LogInfo("Stopping foldersync...");
                    Environment.Exit(0);
                };

                logger.LogInfo("Starting foldersync.");

                logger.LogInfo($"Source folder: {options.SourceFolder}");
                logger.LogInfo($"Replica folder: {options.ReplicaFolder}");
                logger.LogInfo($"Sync interval(s): {options.SyncIntervalSeconds}");
                logger.LogInfo($"Log file path: {options.LogFilePath}");

                var synchronizer = new FolderSynchronizer(logger);

                while (true)
                {
                    logger.LogInfo("Synchronization in progress...");
                    synchronizer.Synchronize(options.SourceFolder, options.ReplicaFolder);
                    logger.LogInfo("Synchronization done.");
                    
                    Thread.Sleep(TimeSpan.FromSeconds(options.SyncIntervalSeconds));
                }
            }
            catch (ArgumentException e)
            {
                string message = $"Invalid options were provided: {e.Message}\n{e.StackTrace}";
                if (logger != null)
                    logger.LogError(message);
                else
                    Console.WriteLine(message);
            }
            catch (Exception e)
            {
                string message = $"Unexpected exception: {e.Message}\n{e.StackTrace}";
                if (logger != null)
                    logger.LogError(message);
                else
                    Console.WriteLine(message);
            }
        }
    }
}
