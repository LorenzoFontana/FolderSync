# FolderSync

Command line tool to **one-way synchronize two folders**. After each synchronization run, the content of the replica folder will match the content of the source folder. The synchronization happens at intervals specified by the user.

## Usage

```bash
foldersync [source folder] [replica folder] [sync interval in seconds] [log file path (optional)]
```

### Parameters

* **source folder**: Path of the folder that the replica folder will match after every synchronization.
* **replica folder**: Path of the folder into which the source folder will be replicated.
* **sync interval in seconds**: Sets the delay in seconds between each synchronization.
* **log file path (optional)**: Optional path to a log file. If not provided, the current folder and a default log file name will be used as the log destination.

## Design notes for the reviewer

While the current implementation satisfies the requirements of this assignment, in a production environment, several enhancements could be considered:

* Persist synchronization settings, such as folder paths, interval, and log location, in a configuration file or database. Avoiding the need to re-run the application with CLI parameters each time.
* Replace `Thread.Sleep` with a robust scheduling framework or timer to allow more flexible and sophisticated scheduling.
* Implement logging with configurable severity levels, log rotation, and external sinks.
* Consider real-time monitoring to provide immediate feedback on synchronization events.
* Abstract the file comparison to support alternative hashing algorithms or delta-based synchronization for large files, reducing I/O overhead.
* Introduce retry mechanisms for transient errors and detailed error reporting
* Utilize asynchronous processing to handle large directories efficiently.
* Ensure proper handling of file system permissions and access restrictions to prevent failures during synchronization.
* Adapt the design for future enhancements such as selective or bidirectional synchronization.
