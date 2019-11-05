# Bulk Actions Module

Contractions: 

_Bulk Actions Module_ – **BAM** 


## BAM 

This module contains the general logic of the bulk actions pipeline. 
In the BAM core lays the logic of the template method. 
The API of BAM provides 4 methods(endpoints) for working template method logic: 

`GET /api/bulk/actions` - Gets the list of all registered actions 

`POST /api/bulk/actions` - Starts bulk action background job. 

`DELETE /api/bulk/actions` - Attempts to cancel running job. 

`POST /api/bulk/actions/data` - Gets action initialization data (could be used to initialize UI) 


Further, the BAM provides following interfaces for your custom implementations: 

**IBulkAction** – provides methods that will be invoked by the BAM core.  

**IDataSource** – should be implemented to get data in your custom way. 

**IDataSourceFactory** – interface to implement your custom data sources which will be registered for each bulk action. 

**IBulkActionFactory** – this factory will help you create your bulk action instance dynamically. 

**IBulkActionProvider** - container to the encapsulation of all previously mentioned entities. You have to instantiate the BulkActionProvider and pass into it your implementations and other parameters. Further, this provider should be added into BulkActionStorage instance that is already implemented in BAM. 

**IBulkActionStorage** - should be resolved in your custom module via Ioc for registration of custom providers. 

**IBulkActionExecutor** – implemented in the BAM (named BulkActionExecutor). If you needed it can be overriden. 

 
## How to register your custom bulk actions module:

For the beginning you should implement following interfaces: 

**IDataSourceFactory**, **IBulkActionFactory**  

Eg let these ones be called: CustomDataSourceFactory, CustomBulkActionFactory. 

Then, these implementations should be registered in IoC Unity container like that: 

`_container.RegisterType<IDataSourceFactory, CustomDataSourceFactory>();` 

`_container.RegisterType<IBulkActionFactory, CustomBulkActionFactory>();`


To register new bulk action, you should instantiate an object of BulkActionProvider class, passing your own parameters to the class constructor. Then add created instance to resolved via IoC the ActionProviderStorage:  

Example: 

`var your_bulk_action_provider = new BulkActionProvider(name, contextTypeName, applicableTypes, dataSourceFactory, actionFactory, permissions);`  

where  

**name** - a unique action name. 

**contextTypeName**  - a type name of your data context. The instance of this type will be passed into your registered DataSourceFactory.

**applicableTypes** – reserved. Not used yet. 

**dataSourceFactory** - the factory for the creation of your DataSource. You can check the type of the passed context and create your own data source in accordance with your logic.

**actionFactory** – the factory for the creation of your bulk action. Here you also can check the type of the passed context and create your bulk action.

**permissions** – an array of strings with permission descriptions. These permissions will be checked by BAM for current authorized user. Permissions can be managed in admin UI.

Further, resolve your BulkActionProviderStorage and pass into it your bulk action provider instance:

`var actionProviderStorage = _container.Resolve<IBulkActionProviderStorage>(); actionProviderStorage.Add(your_bulk_action_provider);`

See additional examples here:

[_Catalog Bulk Actions Module_](https://github.com/VirtoCommerce/vc-module-catalog-bulk-action/blob/dev/VirtoCommerce.CatalogBulkActionsModule/VirtoCommerce.CatalogBulkActionsModule.Web/Module.cs)
