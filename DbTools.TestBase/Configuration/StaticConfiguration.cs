﻿namespace FizzCode.DbTools.TestBase
{
    using FizzCode.LightWeight.AdoNet;
    using Microsoft.Extensions.Configuration;

    public static class StaticConfiguration
    {
        private static ConfigurationBase _configuration;

        public static void Initialize(string configurationFileName)
        {
            if (_configuration == null)
                _configuration = new ConfigurationBase(configurationFileName);
            else
                if (configurationFileName != _configuration.ConfigurationFileName)
                throw new System.InvalidOperationException("Already initialized.");
        }

        public static IConfigurationRoot Configuration => _configuration.Configuration;

        public static ConnectionStringCollection ConnectionStrings { get; } = new ConnectionStringCollection();
    }
}