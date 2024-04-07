using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace AutoDiskCleanup
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        private readonly bool _runAtStart;
        private readonly string _folderRootPath;
        private readonly int _runHour;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _runAtStart = Convert.ToBoolean(_configuration["RunAtStart"]);
            _folderRootPath = _configuration["FolderRootPath"];
            _runHour = Convert.ToInt32(_configuration["RunHour"]);

            _logger.LogInformation("Worker intializing.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (_runAtStart)
                {
                    _logger.LogInformation("Running once-off folder cleanup at start of service.");
                    CleanupFolder();
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(getMillisecondsToNextRun(), stoppingToken);
                    _logger.LogInformation("Running scheduled folder cleanup.");
                    CleanupFolder();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Worker stopping.");

                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during service execution.");

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(2);
            }
        }

        private void CleanupFolder()
        {
            if (!Directory.Exists(_folderRootPath))
            {
                _logger.LogWarning($"Folder root path doesn't exist at {_folderRootPath}");
                return;
            }

            foreach (var subFolder in Directory.GetDirectories(_folderRootPath))
            {
                _logger.LogInformation($"Scanning {subFolder}");
                var dirInfo = new DirectoryInfo(subFolder);

                foreach (var folder in dirInfo.GetDirectories().OrderByDescending(x => x.LastWriteTime).Skip(1))
                {
                    _logger.LogInformation($"Deleting folder at {folder.FullName}");
                    folder.Delete(true);
                }
            }
        }

        private int getMillisecondsToNextRun()
        {
            var currentTime = DateTime.Now.ToLocalTime();
            var nextRunTime = currentTime.Date.AddHours(_runHour);

            if (currentTime >= nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1);
            }

            var millisecondsToNextRun = (int)(nextRunTime - currentTime).TotalMilliseconds;
            _logger.LogInformation($"Milliseconds to next scheduled run: {millisecondsToNextRun}");

            return millisecondsToNextRun;
        }
    }
}
