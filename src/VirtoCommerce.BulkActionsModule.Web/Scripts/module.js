// Call this to register your module to main application
var moduleName = "virtoCommerce.bulkActionsModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .run(['platformWebApp.pushNotificationTemplateResolver', '$state', 'platformWebApp.bladeNavigationService',
        function (pushNotificationTemplateResolver, $state, bladeNavigationService) {

            // register notification template
            pushNotificationTemplateResolver.register({
                priority: 900,
                satisfy: (notify, place) => place === 'header-notification' && notify.notifyType === 'BulkActionPushNotification',
                template: 'Modules/$(VirtoCommerce.BulkActionsModule)/Scripts/notifications/headerNotification.tpl.html',
                action: function (notify) { $state.go('workspace.pushNotificationsHistory', notify) }
            });
        }
    ]);
