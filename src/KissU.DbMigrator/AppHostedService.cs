using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Threading;

namespace KissU.DbMigrator
{
    public class AppHostedService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var application = AbpApplicationFactory.Create<AppModule>(options =>
            {
                options.UseAutofac();
                options.Services.AddLogging(c => c.AddSerilog());
            }))
            {
                application.Initialize();

                AsyncHelper.RunSync(async () =>
                {
                    using var scope = application.ServiceProvider.CreateScope();
                    await scope.ServiceProvider
                        .GetRequiredService<IDataSeeder>()
                        .SeedAsync();
                });

                application.Shutdown();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
