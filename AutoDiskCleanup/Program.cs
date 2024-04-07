using Serilog.Events;
using Serilog;

namespace AutoDiskCleanup
{
    public class Program
    {
        private const string SERVICENAME = "Auto Disk Cleanup";
        private static string serviceNameNoSpaces => new string(SERVICENAME.Where(x => !char.IsWhiteSpace(x)).ToArray());

        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = SERVICENAME;
            });

            builder.Services.AddHostedService<Worker>();

            var debugText = "";
#if DEBUG
            debugText = "Debug";
#endif

            builder.Logging
                .ClearProviders()
                .AddSerilog(
                    new LoggerConfiguration()
                       .MinimumLevel.Information()
                       .WriteTo.File($"C:/ProgramData/{serviceNameNoSpaces}{debugText}/logs/log.txt", rollingInterval: RollingInterval.Day)
                       .WriteTo.Console()
                       .CreateLogger()
                );

            var host = builder.Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Starting host");
                host.Run();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Host terminated unexpectedly");
            }
        }
    }
}