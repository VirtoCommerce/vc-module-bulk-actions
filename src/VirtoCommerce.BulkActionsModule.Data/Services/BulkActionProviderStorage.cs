using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.BulkActionsModule.Core.Services;

namespace VirtoCommerce.BulkActionsModule.Data.Services
{
    public class BulkActionProviderStorage : IBulkActionProviderStorage
    {
        private const string StorageName = nameof(BulkActionProviderStorage);

        private readonly ConcurrentDictionary<string, IBulkActionProvider> _providers =
            new ConcurrentDictionary<string, IBulkActionProvider>();

        public IBulkActionProvider Add(IBulkActionProvider provider)
        {
            var name = provider.Name;

            if (_providers.ContainsKey(name))
            {
                throw new ArgumentException($"Action \"{name}\" is already registered.");
            }

            if (_providers.TryAdd(name, provider))
            {
                return _providers[name];
            }

            throw new ArgumentException($"Action \"{name}\" adding fail.");
        }

        public IBulkActionProvider Get(string name)
        {
            if (_providers.TryGetValue(name, out var action))
            {
                return action;
            }

            throw new ArgumentException($"Action \"{name}\" is not found in \"{StorageName}\".");
        }

        public IEnumerable<IBulkActionProvider> GetAll()
        {
            return _providers.Values.ToArray();
        }
    }
}
