namespace VirtoCommerce.BulkActionsModule.Core.BulkActionImplementations
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using VirtoCommerce.BulkActionsModule.Core.BulkActionAbstractions;
    using VirtoCommerce.Platform.Core.Common;

    public class BulkActionProviderStorage : IBulkActionProviderStorage
    {
        private readonly ConcurrentDictionary<string, IBulkActionProvider> _providers =
            new ConcurrentDictionary<string, IBulkActionProvider>();

        public IBulkActionProvider Add(IBulkActionProvider provider)
        {
            var name = provider.Name;

            if (_providers.ContainsKey(name))
            {
                // idle
            }
            else
            {
                _providers.TryAdd(name, provider);
            }

            return _providers[name];
        }

        public IBulkActionProvider Get(string name)
        {
            var action = _providers.Values.FirstOrDefault(value => value.Name.EqualsInvariant(name));
            if (action == null)
            {
                var message = $"Action \"{name}\" is not found in \"{nameof(IBulkActionProviderStorage)}\".";
                throw new ArgumentException(message);
            }

            return action;
        }

        public IEnumerable<IBulkActionProvider> GetAll()
        {
            return _providers.Values.ToArray();
        }
    }
}