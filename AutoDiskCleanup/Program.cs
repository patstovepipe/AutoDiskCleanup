using Serilog.Events;
using Serilog;
using CliWrap;
using System.ServiceProcess;

namespace AutoDiskCleanup
{
    public class Program
    {
        private const string SERVICENAME = "Auto Disk Cleanup";
        private static string serviceNameNoSpaces => new string(SERVICENAME.Where(x => !char.IsWhiteSpace(x)).ToArray());
        private const string SERVICEDESCRIPTION = "A service which deletes old folders within a specified root directory.";


        public static void Main(string[] args)
        {
            if (args is { Length: 1 })
            {
                try
                {
                    string executablePath =
                        Path.Combine(AppContext.BaseDirectory, $"{serviceNameNoSpaces}.exe");

                    if (args[0] is "/Install")
                    {
                        Cli.Wrap("sc")
                            .WithArguments(new[] { "create", SERVICENAME, $"binPath={executablePath}", "start=auto" })
                            .ExecuteAsync();
                        Cli.Wrap("sc")
                            .WithArguments(new[] { "description", SERVICENAME, SERVICEDESCRIPTION })
                            .ExecuteAsync();
                    }
                    else if (args[0] is "/Uninstall")
                    {
                        Cli.Wrap("sc")
                            .WithArguments(new[] { "stop", SERVICENAME })
                            .ExecuteAsync();
                        Cli.Wrap("sc")
                            .WithArguments(new[] { "delete", SERVICENAME })
                            .ExecuteAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return;
            }

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