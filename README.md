# Bulk Actions Module

[![CI status](https://github.com/VirtoCommerce/vc-module-bulk-actions/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-bulk-actions/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-bulk-actions&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-bulk-actions) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-bulk-actions&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-bulk-actions) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-bulk-actions&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-bulk-actions) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-bulk-actions&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-bulk-actions)

Contractions: 

_Bulk Actions Module_ – **BAM** 


## BAM 

This module contains the general logics of the bulk actions pipeline. 
[The BAM has the template method logics](https://en.wikipedia.org/wiki/Template_method_pattern).
The API of BAM provides 4 methods(endpoints): 

`GET /api/bulk/actions` - Gets the list of all registered actions 

`POST /api/bulk/actions` - Starts bulk action background job. 

`DELETE /api/bulk/actions` - Attempts to cancel running job. 

`POST /api/bulk/actions/data` - Gets action initialization data (can be used for UI initialization) 


Further, the BAM provides the following interfaces for your custom implementations: 

**IBulkAction** – provides methods that will be invoked by the BAM core.  

**IDataSource** – should be implemented to get data in your custom way. 

**IDataSourceFactory** – interface to implement your custom data sources which will be registered for each bulk action. 

**IBulkActionFactory** – this factory will help you create your bulk action instance dynamically. 

**IBulkActionProvider** - container for encapsulation of all previously mentioned entities. You have to instantiate your version of the BulkActionProvider and add into the BulkActionStorage instance that is already implemented in BAM.

**IBulkActionStorage** - should be resolved in your custom module via Ioc for registration of custom providers. 

**IBulkActionExecutor** – implemented in the BAM (named BulkActionExecutor). It also can be overriden in your code.

 
## How to register your custom bulk actions module:

For the beginning you should implement the following interfaces: 

**IDataSourceFactory**, **IBulkActionFactory**  

Let's name them for instance: CustomDataSourceFactory, CustomBulkActionFactory. 

Then, these implementations should be registered in IoC Unity container as follows: 

`_container.RegisterType<IDataSourceFactory, CustomDataSourceFactory>();` 

`_container.RegisterType<IBulkActionFactory, CustomBulkActionFactory>();`


To register new bulk action, you should instantiate an object of BulkActionProvider class, passing your own parameters to the class constructor. Then add created instance to resolved via IoC the ActionProviderStorage:  

Example: 

`var your_bulk_action_provider = new BulkActionProvider(name, contextTypeName, applicableTypes, dataSourceFactory, actionFactory, permissions);`  

where  

**name** - a unique action name. 

**contextTypeName**  - a type name of your data context. The instance of this type will be passed into your registered DataSourceFactory.

**applicableTypes** – reserved. Not used yet. 

**dataSourceFactory** - the factory for the creation of your DataSource. You can check the type of the passed context and create your own data source in accordance with your logics.

**actionFactory** – the factory for the creation of your bulk action. Here you also can check the type of the passed context and create your bulk action.

**permissions** – an array of strings with permission descriptions. These permissions will be checked by BAM for the current authorized user. Permissions can be managed in admin UI.

Further, resolve your BulkActionProviderStorage and pass into it your bulk action provider instance:

`var actionProviderStorage = _container.Resolve<IBulkActionProviderStorage>(); actionProviderStorage.Add(your_bulk_action_provider);`

See additional examples here:

[_Catalog Bulk Actions Module_](https://github.com/VirtoCommerce/vc-module-catalog-bulk-action/blob/dev/VirtoCommerce.CatalogBulkActionsModule/VirtoCommerce.CatalogBulkActionsModule.Web/Module.cs)
