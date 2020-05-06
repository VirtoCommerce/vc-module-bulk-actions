﻿namespace VirtoCommerce.BulkActionsModule.Web
{
    using System.Web.Http;

    using Hangfire.Common;

    using Microsoft.Practices.Unity;

    using VirtoCommerce.BulkActionsModule.Core;
    using VirtoCommerce.BulkActionsModule.Data.Security;
    using VirtoCommerce.BulkActionsModule.Data.Services;
    using VirtoCommerce.BulkActionsModule.Web.BackgroundJobs;
    using VirtoCommerce.BulkActionsModule.Web.JsonConverters;
    using VirtoCommerce.Platform.Core.Modularity;

    public class Module : ModuleBase
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void Initialize()
        {
            base.Initialize();

            // to shared module
            _container.RegisterInstance<IBulkActionProviderStorage>(new BulkActionProviderStorage());
            _container.RegisterType<IBulkActionExecutor, BulkActionExecutor>();
            _container.RegisterType<ISecurityHandlerFactory, SecurityHandlerFactory>();
            _container.RegisterType<IBackgroundJobExecutor, BackgroundJobExecutor>();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
            var httpConfiguration = _container.Resolve<HttpConfiguration>();
            JobHelper.SetSerializerSettings(httpConfiguration.Formatters.JsonFormatter.SerializerSettings);
            var converter = new BulkActionContextJsonConverter();
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(converter);
        }

        public override void SetupDatabase()
        {
            // idle
        }
    }
}