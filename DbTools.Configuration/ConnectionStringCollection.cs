﻿namespace FizzCode.DbTools.Configuration
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    public class ConnectionStringCollection
    {
        private readonly Dictionary<string, ConnectionStringWithProvider> _connectionStrings = new Dictionary<string, ConnectionStringWithProvider>();
        public IEnumerable<ConnectionStringWithProvider> All => _connectionStrings.Values;

        public void LoadFromConfiguration(IConfigurationRoot configuration, string section = "ConnectionStrings")
        {
            var connectionStrings = configuration
                .GetSection(section)
                .Get<ConnectionStringWithProvider[]>();

            if (connectionStrings == null)
                return;

            foreach (var connectionString in connectionStrings)
            {
                Add(connectionString);
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