using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace KissU.DbMigrator
{
    class Program
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

                await CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Modules.Blogging.DbMigrator.DbMigratorHostedService>();
                    services.AddHostedService<Modules.Identity.DbMigrator.DbMigratorHostedService>();
                    services.AddHostedService<Modules.IdentityServer.DbMigrator.DbMigratorHostedService>();
                    services.AddHostedService<Modules.AuditLogging.DbMigrator.DbMigratorHostedService>();
                    services.AddHostedService<Modules.BackgroundJobs.DbMigrator.DbMigratorHostedService>();
                    services.AddHostedService<Modules.SettingManagement.DbMigrator.DbMigratorHostedService>();
                    services.AddHostedService<Modules.PermissionManagement.DbMigrator.DbMigratorHostedService>();
                    services.AddHostedService<Modules.FeatureManagement.DbMigrator.DbMigratorHostedService>();
                    services.AddHostedService<Modules.TenantManagement.DbMigrator.DbMigratorHostedService>();
                });
    }
}
