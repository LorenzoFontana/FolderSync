using System.Security.Cryptography;

namespace FolderSync
{
    internal class FolderSynchronizer
    {
        readonly FolderSyncLogger _logger;
        public FolderSynchronizer(FolderSyncLogger logger)
        {
            _logger = logger;
        }

        public void Synchronize(string sourceFolder, string replicaFolder)
        {
            if (!Directory.Exists(replicaFolder))
            {
                _logger.LogInfo($"Replica folder not found - creating it at {replicaFolder}");
                Directory.CreateDirectory(replicaFolder);
            }

            foreach (string folderPath in Directory.GetDirectories(sourceFolder, "*", SearchOption.AllDirectories))
                CreateFolder(folderPath, sourceFolder, replicaFolder);

            foreach (string filePath in Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories))
                CopyIfNewOrDifferent(filePath, sourceFolder, replicaFolder);

            foreach(string entryPath in Directory.GetFileSystemEntries(replicaFolder, "*", SearchOption.AllDirectories))
                DeleteIfNotInSource(entryPath, sourceFolder, replicaFolder);
        }

        private void CreateFolder(string folderPath, string sourceFolder, string replicaFolder)
        {
            string relativeFolderPath = Path.GetRelativePath(sourceFolder, folderPath);
            string replicaFolderPath = Path.Combine(replicaFolder, relativeFolderPath);

            if (!Directory.Exists(replicaFolderPath))
            {
                try
                {
                    _logger.LogInfo($"Creating directory: {replicaFolderPath}");
                    Directory.CreateDirectory(replicaFolderPath);
                }
                catch(Exception e) 
                {
                    _logger.LogError($"Error creating directory {replicaFolderPath}:  {e.Message}");
                }
            }
        }
        private void CopyIfNewOrDifferent(string filePath, string sourceFolder, string replicaFolder)
        {

                string fileRelativePath = Path.GetRelativePath(sourceFolder, filePath);
                string replicaPath = Path.Combine(replicaFolder, fileRelativePath);
                string directory = Path.GetDirectoryName(replicaPath);

                if (!string.IsNullOrEmpty(directory))
                {
                    try
                    {
                        Directory.CreateDirectory(directory);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Error creating directory {directory}:  {e.Message}");
                        return;
                    }
                }

                try
                {
                    if (!File.Exists(replicaPath) || !FilesAreEqual(replicaPath, filePath))
                    {
                        _logger.LogInfo($"Copying file from {filePath} to {replicaPath}");
                        File.Copy(filePath, replicaPath, true);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error copying file from {filePath} to {replicaPath}: {e.Message}");
                }
        }
        private bool FilesAreEqual(string filePath1, string filePath2)
        {
            FileInfo fileInfo1 = new FileInfo(filePath1);
            FileInfo fileInfo2 = new FileInfo(filePath2);

            if (fileInfo1.Length != fileInfo2.Length ||
                fileInfo1.LastWriteTimeUtc != fileInfo2.LastWriteTimeUtc)
                return false;

            using var md5 = MD5.Create();
            using var fileStream1 = File.OpenRead(filePath1);
            using var fileStream2 = File.OpenRead(filePath2);

            var hash1 = md5.ComputeHash(fileStream1);
            var hash2 = md5.ComputeHash(fileStream2);

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }

            return true;
        }

        private void DeleteIfNotInSource(string path, string sourceFolder, string replicaFolder)
        {
            string fileRelativePath = Path.GetRelativePath(replicaFolder, path);
            string sourcePath = Path.Combine(sourceFolder, fileRelativePath);

            if (File.Exists(path))
            {
                if (!File.Exists(sourcePath))
                {
                    try
                    {
                        _logger.LogInfo($"Deleting file: {path}");
                        File.Delete(path);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Error deleting file {path}: {e.Message}");
                    }
                }
            }
            else if (Directory.Exists(path))
            {
                if (!Directory.Exists(sourcePath))
                {
                    try
                    {
                        _logger.LogInfo($"Deleting directory: {path}");
                        Directory.Delete(path, true);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Error deleting folder {path}: {e.Message}");
                    }
                }
            }
        }
    }
}
