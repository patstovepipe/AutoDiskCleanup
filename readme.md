## AutoDiskCleanup

This C# project automates disk cleanup by deleting old folders within a specified root directory.
> *Originally created to cleanup old snapshots left behind by SQL Server Replication.*

### Features

* Windows Service: Runs in the background to automatically delete old folders based on your configuration.
* Easy Installation: Uses an MSI installer for simple deployment.
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
2. **AutoDiskCleanup.Installer**: This subproject contains the code for building the MSI installer which is then used to install the service.

### Building Installer

1. Open the solution file (`AutoDiskCleanup.sln`) in Visual Studio 2022.
2. Select the **Release** configuration.
3. Build the solution (**Build** -> **Build Solution**). 
4. Publish the **AutoDiskCleanup** subproject (**Build** -> **Publish Selection**).
5. Build the **AutoDiskCleanup.Installer** subproject. This will create the MSI installer in the `Release` folder of the **AutoDiskCleanup.Installer** subproject.

### Installation

1. Either use the latest installer from the **Installers** folder or create a new installer (documented above).
3. Double-click the MSI file (e.g. `AutoDiskCleanup.Installer.msi`) to install the service.

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

### Usage

The Windows service is not automatically started when installed. You can manage the service from the Windows Services window (**Start** -> **Run**, type `services.msc`, and press Enter). From there right click **Auto Disk Cleanup** and select **Start**

### Uninstallation

1. Open **Add or Remove Programs** (or **Programs and Features**) in the Control Panel.
2. Find and select **Auto Disk Cleanup**.
3. Click **Uninstall** and follow the on-screen instructions.

### Logging

The service logs its activity to files located at `C:\ProgramData\AutoDiskCleanup\logs`. You can review these logs to monitor the service's operation.

