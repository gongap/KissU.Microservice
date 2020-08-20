using KissU.Abp.Business;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace KissU.ConsoleClient.Host
{
    [DependsOn(
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class AppModule : AbpModule, IAbpStartupModule
    {
    }
}
