angular.module('virtoCommerce.bulkActionsModule')
    .controller('virtoCommerce.bulkActionsModule.helloWorldController', ['$scope', 'virtoCommerce.bulkActionsModule.webApi', function ($scope, api) {
        var blade = $scope.blade;
        blade.title = 'VirtoCommerce.BulkActionsModule';

        blade.refresh = function () {
            api.get(function (data) {
                blade.title = 'virtoCommerce.bulkActionsModule.blades.hello-world.title';
                blade.data = data.result;
                blade.isLoading = false;
            });
        };

        blade.refresh();
    }]);
