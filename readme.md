## AutoDiskCleanup

This C# project automates disk cleanup by deleting old folders within a specified root directory.
> *Originally created to cleanup old snapshots left behind by SQL Server Replication.*

### Features

* Windows Service: Runs in the background to automatically delete old folders based on your configuration.
* Logging: Detailed logs are written to a central location for troubleshooting.
* Configurable Settings: Manage cleanup behavior through the `appsettings.json` file.
* Unit Tests: Ensure code functionality by testing application logic.

### Prerequisites

* Microsoft Visual Studio 2022  ([https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/))
* .NET Framework (version to be specified based on your target environment) ([https://dotnet.microsoft.com/en-us/download/dotnet-framework](https://dotnet.microsoft.com/en-us/download/dotnet-framework))

### Development

The source code for this project is written in C# and consists of two subprojects:

1. **AutoDiskCleanup**: This subproject contains the Windows service implementation responsible for deleting old folders.
2. **AutoDiskCleanup.Tests**: This subproject contains the units tests for the project above.

### Configuration

The service uses an `appsettings.json` file located in the same directory as the service executable (typically `C:\Program Files\AutoDiskCleanup`) to manage its behavior. You can edit this file to customize the cleanup process. Here are some of the configurable settings:

* **RunAtStart (bool):** Controls whether the service runs an initial cleanup on start (default: `true`).
* **FolderRootPath (string):** Specifies the root folder containing the subdirectories for cleanup (e.g., `C:\SomeData`).
* **RunHour (int):** Defines the hour of the day (in 24-hour format) when the service should run the cleanup task (default: `0` - runs at midnight).

**Example appsettings.json:**

```json
{
  "RunAtStart": true,
  "FolderRootPath": "C:\\SomeData",
  "RunHour": 2  // Run cleanup at 2:00 AM
}
```

**Note:** You will need to restart the service for configuration changes in `appsettings.json` to take effect.

### Logging

The service logs its activity to files located at `C:\ProgramData\AutoDiskCleanup\logs`. You can review these logs to monitor the service's operation.

