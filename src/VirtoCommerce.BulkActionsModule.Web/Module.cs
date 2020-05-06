using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.BulkActionsModule.Core;
using VirtoCommerce.BulkActionsModule.Core.Services;
using VirtoCommerce.BulkActionsModule.Data.Services;
using VirtoCommerce.BulkActionsModule.Web.Authorization;
using VirtoCommerce.BulkActionsModule.Web.BackgroundJobs;
using VirtoCommerce.BulkActionsModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.BulkActionsModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IBulkActionProviderStorage>(new BulkActionProviderStorage());
            serviceCollection.AddTransient<IBulkActionExecutor, BulkActionExecutor>();
            serviceCollection.AddTransient<IAuthorizationHandler, BulkActionsAuthorizationHandler>();
            serviceCollection.AddTransient<IBackgroundJobExecutor, BackgroundJobExecutor>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x => new Permission() { GroupName = "BulkActions", Name = x }).ToArray());

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcNewtonsoftJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new BulkActionContextJsonConverter());
        }

        public void Uninstall()
        {
        }
    }
}
