// Call this to register your module to main application
var moduleName = "virtoCommerce.bulkActionsModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $stateProvider
                .state('workspace.virtoCommerceBulkActionsModuleState', {
                    url: '/virtoCommerce.bulkActionsModule',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: [
                        '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                            var newBlade = {
                                id: 'blade1',
                                controller: 'virtoCommerce.bulkActionsModule.helloWorldController',
                                template: 'Modules/$(virtoCommerce.bulkActionsModule)/Scripts/blades/hello-world.html',
                                isClosingDisabled: true
                            };
                            bladeNavigationService.showBlade(newBlade);
                        }
                    ]
                });
        }
    ])
    .run([
        '$rootScope',
        'platformWebApp.mainMenuService',
        'platformWebApp.widgetService',
        '$state', 'platformWebApp.toolbarService',
        'platformWebApp.bladeUtils',
        'virtoCommerce.catalogModule.catalogBulkActionService',
        'platformWebApp.bladeNavigationService',
        function ($rootScope,
            mainMenuService,
            widgetService,
            $state,
            toolbarService,
            bladeUtils,
            catalogBulkActionService,
            bladeNavigationService) {

            //Register module in main menu
            //var menuItem = {
            //    path: 'browse/virtoCommerce.bulkActionsModule',
            //    icon: 'fa fa-cube',
            //    title: 'VirtoCommerce.bulkActionsModule',
            //    priority: 100,
            //    action: function () { $state.go('workspace.virtoCommerceBulkActionsModuleState'); },
            //    permission: 'virtoCommerce.bulkActionsModule.WebPermission'
            //};
            //mainMenuService.addMenuItem(menuItem);

        }
]);
