using System;
using System.IO;
using Autofac;
using KissU.Caching;
using KissU.Caching.Configurations;
using KissU.CPlatform.Configurations;
using KissU.Dependency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using KissU.CPlatform;
using KissU.Extensions;
using KissU.ServiceProxy;

namespace KissU.PublicWebSite.Host
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Async(c => c.File(Path.Combine(Directory.GetCurrentDirectory(), "logs/logs.txt")))
                .CreateLogger();

            try
            {
                Log.Information("Starting PublicWebSite.Host.");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "PublicWebSite.Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddCPlatformFile("servicesettings.json", false, true);
                    builder.AddCacheFile("cachesettings.json", false, true);
                })
                .UseServiceHostBuilder(builder =>
                {
                    builder.AddMicroService(service => { service.AddClient().AddCache(); });
                    builder.Register(p => new CPlatformContainer(ServiceLocator.Current));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseClient()
                .UseSerilog();
    }
}
