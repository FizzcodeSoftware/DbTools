﻿namespace FizzCode.DbTools.Configuration
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    public class ConnectionStringCollection
    {
        private readonly Dictionary<string, ConnectionStringWithProvider> _connectionStrings = new Dictionary<string, ConnectionStringWithProvider>();
        public IEnumerable<ConnectionStringWithProvider> All => _connectionStrings.Values;

        public void LoadFromConfiguration(IConfigurationRoot configuration, string sectionKey = "ConnectionStrings")
        {
            var children = configuration
                .GetSection(sectionKey)
                .GetChildren();

            foreach (var child in children)
            {
                Add(new ConnectionStringWithProvider(
                    name: child.Key,
                    providerName: child.GetValue<string>("ProviderName"),
                    connectionString: child.GetValue<string>("ConnectionString")));
            }
        }

        public void Add(ConnectionStringWithProvider connectionString)
        {
            _connectionStrings[connectionString.Name.ToLowerInvariant()] = connectionString;
        }

        public ConnectionStringWithProvider this[string name]
        {
            get
            {
                name = name.ToLowerInvariant();
                _connectionStrings.TryGetValue(name, out var value);
                return value;
            }
        }
    }
}