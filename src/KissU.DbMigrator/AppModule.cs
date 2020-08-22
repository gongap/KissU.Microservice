using KissU.Modules.Identity.EntityFrameworkCore;
using KissU.Modules.IdentityServer.EntityFrameworkCore;
using KissU.Modules.PermissionManagement.EntityFrameworkCore;
using KissU.Modules.SettingManagement.EntityFrameworkCore;
using KissU.Modules.AuditLogging.EntityFrameworkCore;
using KissU.Modules.IdentityServer.AspNetIdentity;
using KissU.Modules.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace KissU.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpIdentityServerAspNetIdentityModule),
        typeof(AbpIdentityEntityFrameworkCoreModule),
        typeof(AbpIdentityServerEntityFrameworkCoreModule),
        typeof(AbpPermissionManagementEntityFrameworkCoreModule),
        typeof(AbpAuditLoggingEntityFrameworkCoreModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule)
    )]
    public class AppModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbContextOptions>(options =>
            {
                options.UseSqlServer();
            });
        }
    }
}