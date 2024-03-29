﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.Common.Logger;
using FizzCode.LightWeight;
using FizzCode.LightWeight.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.TestBase;
public static class TestHelper
{
    private static readonly bool _forceIntegrationTests;
    private static readonly IConfigurationRoot _configuration;

#pragma warning disable CA1810 // Initialize reference type static fields inline
    static TestHelper()
#pragma warning restore CA1810 // Initialize reference type static fields inline
    {
        _configuration = ConfigurationLoader.LoadFromJsonFile("testconfig", true);
        var forceIntegrationTests = _configuration["forceIntegrationTests"];
        _forceIntegrationTests = forceIntegrationTests == "true";
    }

    public static bool ShouldForceIntegrationTests()
    {
#if INTEGRATION
        return true;
#endif
        return _forceIntegrationTests;
    }

    public static bool ShouldRunIntegrationTest(string providerName)
    {
        if (ShouldForceIntegrationTests())
            return true;

        return providerName switch
        {
            "Microsoft.Data.SqlClient" => false,
            "Oracle.ManagedDataAccess.Client" => false,
            _ => true,
        };
    }

    public static bool ShouldRunIntegrationTest(SqlEngineVersion version)
    {
        if (ShouldForceIntegrationTests())
            return true;

        if (version is MsSqlVersion)
            return false;

        if (version is OracleVersion)
            return false;

        return true;
    }

    public static Settings GetDefaultTestSettings(SqlEngineVersion version)
    {
        var settings = Helper.GetDefaultSettings(version, _configuration);

        if (version is SqLiteVersion)
        {
            settings.SqlVersionSpecificSettings["ShouldCreateAutoincrementAsPrimaryKey"] = true;
        }

        if (version is OracleVersion)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var callerAssemblies = new StackTrace().GetFrames()
                        .Select(f => f.GetMethod()?.ReflectedType?.Assembly).Distinct()
                        .Where(a => a?.GetReferencedAssemblies().Any(a2 => a2.FullName == executingAssembly.FullName) == true);
            var initialAssembly = callerAssemblies.Last();

            var assemblyName = initialAssembly?.GetName().Name;
            if (assemblyName?.StartsWith("FizzCode.DbTools.", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                assemblyName = assemblyName["FizzCode.DbTools.".Length..];
            }

            var schemaName = assemblyName?.Replace(".", "_", StringComparison.InvariantCultureIgnoreCase);
            if (schemaName is not null)
                settings.SqlVersionSpecificSettings["DefaultSchema"] = schemaName;
        }

        return settings;
    }

    public static void CheckFeature(SqlEngineVersion version, string feature, string? message = null)
    {
        var additionalMessage = message == null ? null : " " + message;
        var featureSupport = Features.GetSupport(version, feature);
        if (featureSupport.Support == Support.NotSupported)
            Assert.Inconclusive($"Test is skipped, feature {feature} is not supported. ({featureSupport.Description}){additionalMessage}");

        if (featureSupport.Support == Support.NotImplementedYet)
            Assert.Inconclusive($"Test is skipped, feature {feature} is not implemented (yet). ({featureSupport.Description}){additionalMessage}");
    }

    public static void CheckProvider(SqlEngineVersion version, IEnumerable<NamedConnectionString> connectionStrings)
    {
        RegisterProviders();
        var usedVersions = GetSqlVersionsWithConfiguredConnectionStrting(connectionStrings);
        if (!usedVersions.Contains(version))
            Assert.Inconclusive($"Test is skipped, .Net Framework Data Provider is not usabe for {version} engine version, provider name: {version.ProviderName}. No valid connection string is configured.");
    }

    private static List<SqlEngineVersion>? _sqlVersionsWithConfiguredConnectionStrting;

    private static List<SqlEngineVersion> GetSqlVersionsWithConfiguredConnectionStrting(IEnumerable<NamedConnectionString> connectionStringCollection)
    {
        if (_sqlVersionsWithConfiguredConnectionStrting is null)
        {
            _sqlVersionsWithConfiguredConnectionStrting = [];
            foreach (var connectionString in connectionStringCollection)
            {
                if (!string.IsNullOrEmpty(connectionString.ConnectionString))
                {
                    var sqlEngineVersion = Throw.IfNull(connectionString.GetSqlEngineVersion());
                    _sqlVersionsWithConfiguredConnectionStrting.Add(sqlEngineVersion);
                }
            }
        }

        return _sqlVersionsWithConfiguredConnectionStrting;
    }

    private static bool _areDbProviderFactoriesRegistered;

    private static void RegisterProviders()
    {
        if (!_areDbProviderFactoriesRegistered)
        {
            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("System.Data.SQLite", System.Data.SQLite.SQLiteFactory.Instance);
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("Oracle.ManagedDataAccess.Client", Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);

            _areDbProviderFactoriesRegistered = true;
        }
    }

    public static Logger CreateLogger()
    {
        var logger = new Logger();

        var configuration = ConfigurationLoader.LoadFromJsonFile("testconfig", true);
        var logConfiguration = configuration?.GetSection("Log").Get<LogConfiguration>();

        var iLogger = SerilogConfigurator.CreateLogger(logConfiguration);

        var debugLogger = new DebugLogger(iLogger);

        logger.LogEvent += debugLogger.OnLog;

        return logger;
    }
}