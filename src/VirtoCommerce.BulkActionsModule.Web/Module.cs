using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.BulkActionsModule.Core;
using VirtoCommerce.BulkActionsModule.Core.Models.BulkActions;
using VirtoCommerce.BulkActionsModule.Core.Services;
using VirtoCommerce.BulkActionsModule.Data.Services;
using VirtoCommerce.BulkActionsModule.Web.BackgroundJobs;
using VirtoCommerce.Platform.Core.JsonConverters;
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
            serviceCollection.AddTransient<IBackgroundJobExecutor, BackgroundJobExecutor>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x => new Permission() { GroupName = "BulkActions", Name = x }).ToArray());

            PolymorphJsonConverter.RegisterTypeForDiscriminator(typeof(BulkActionContext), nameof(BulkActionContext.ContextTypeName));
        }

        public void Uninstall()
        {
        }
    }
}
