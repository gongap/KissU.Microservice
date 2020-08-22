using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace KissU.DbMigrator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
                .MinimumLevel.Override("KissU.DbMigrator", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("KissU.DbMigrator", LogEventLevel.Information)
#endif
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "logs/logs.txt"))
                .WriteTo.Console()
                .CreateLogger();

            Console.WriteLine("Initializing KissU DbMigrator ... ");
            await RunMigrations();
            Console.WriteLine("\n\nPress ENTER to exit ...");
            Console.ReadLine();
        }

        private static async Task RunMigrations()
        {
            Console.Write("\nThis program updates an existing database or creates a new one if not exists.\n" +
                          "Are you sure you want to run the migration? (y/n) ");

            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.WriteLine("\n\nMigrating database...");

                try
                {
                    var cts = new CancellationTokenSource();
                    await new Modules.Blogging.DbMigrator.DbMigratorHostedService().StartAsync(cts.Token);
                    await new Modules.Identity.DbMigrator.DbMigratorHostedService().StartAsync(cts.Token);
                    await new Modules.IdentityServer.DbMigrator.DbMigratorHostedService().StartAsync(cts.Token);
                    await new Modules.AuditLogging.DbMigrator.DbMigratorHostedService().StartAsync(cts.Token);
                    await new Modules.BackgroundJobs.DbMigrator.DbMigratorHostedService().StartAsync(cts.Token);
                    await new Modules.TenantManagement.DbMigrator.DbMigratorHostedService().StartAsync(cts.Token);
                    await new Modules.SettingManagement.DbMigrator.DbMigratorHostedService().StartAsync(cts.Token);
                    await new Modules.FeatureManagement.DbMigrator.DbMigratorHostedService().StartAsync(cts.Token);
                    await new Modules.PermissionManagement.DbMigrator.DbMigratorHostedService().StartAsync(cts.Token);
                    await new AppHostedService().StartAsync(cts.Token);
                    Console.WriteLine("Migration completed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        Console.WriteLine(ex.Message);
                    }

                    Console.Write("\nThere was problem while applying migrations. " );
                }
            }
        }
    }
}
