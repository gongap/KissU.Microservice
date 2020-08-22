using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.UI.Navigation.Urls;
using Localization.Resources.AbpUi;
using KissU.Abp.Autofac;
using KissU.ApplicationParts.Account.IdentityServer;
using KissU.AuthServer.Host.Localization;
using KissU.Modules.Identity.EntityFrameworkCore;
using KissU.Modules.IdentityServer.EntityFrameworkCore;
using KissU.Shared.MultiTenancy;

namespace KissU.AuthServer.Host
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpAccountWebIdentityServerModule),
        typeof(AbpEntityFrameworkCoreSqlServerModule),
        typeof(AbpIdentityEntityFrameworkCoreModule),
        typeof(AbpIdentityServerEntityFrameworkCoreModule)
    )]
    public class AppModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(typeof(AuthServerResource), typeof(AppModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            Configure<AbpMultiTenancyOptions>(options =>
            {
                options.IsEnabled = IsEnabledMultiTenancy();
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseSqlServer();
            });

            Configure<AbpAuditingOptions>(options =>
            {
                options.IsEnabledForGetRequests = true;
                options.ApplicationName = "AuthServer";
            });

            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            }); 
            
            Configure<AbpBackgroundJobOptions>(options =>
            {
                options.IsJobExecutionEnabled = false;
            });

            ConfigureLocalizationServices();
            ConfigureCache(configuration);
            ConfigureVirtualFileSystem(context);
            ConfigureCors(context, configuration);

            context.Services.ConfigureNonBreakingSameSiteCookies();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseErrorPage();
            }

            app.UseCorrelationId();
            app.UseVirtualFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseCors(DefaultCorsPolicyName);
            app.UseAuthentication();

            if (IsEnabledMultiTenancy())
            {
                app.UseMultiTenancy();
            }

            app.UseAbpRequestLocalization();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseAuditing();
            app.UseConfiguredEndpoints();

            RunDataSeeder(context);
        }

        private bool IsEnabledMultiTenancy()
        {
            return MultiTenancyConsts.IsEnabled;
        }

        /// <summary>
        /// 运行种子数据
        /// </summary>
        /// <param name="context">The context.</param>
        private static void RunDataSeeder(ApplicationInitializationContext context)
        {
            AsyncHelper.RunSync(async () =>
            {
                using var scope = context.ServiceProvider.CreateScope();
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            });
        }

        /// <summary>
        /// 配置本地化服务
        /// </summary>
        private void ConfigureLocalizationServices()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<AuthServerResource>("en")
                    .AddBaseTypes(typeof(AbpUiResource))
                    .AddVirtualJson("/Localization/Resources");

                options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
                options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
                options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
                options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            });
        }

        /// <summary>
        /// 配置虚拟文件系统
        /// </summary>
        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AppModule>("KissU.AuthServer.Host");
            });

            var hostingEnvironment = context.Services.GetHostingEnvironment();
            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<AppModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}KissU.AuthServer.Host"));
                });
            }
        }

        /// <summary>
        ///配置缓存
        /// </summary>
        private void ConfigureCache(IConfiguration configuration)
        {
            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.KeyPrefix = "AuthServer:";
            });
        }

        /// <summary>
        /// 配置跨域源.
        /// </summary>
        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
    }
}