﻿namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class SqlExecuterTestAdapter
    {
        private readonly Dictionary<string, (SqlExecuter SqlExecuter, SqlDialect SqlDialect)> sqlExecutersAndDialects = new Dictionary<string, (SqlExecuter, SqlDialect)>();

        public ConnectionStringSettings Initialize(string connectionStringKey)
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringKey];

            var sqlDialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);

            if (!sqlExecutersAndDialects.ContainsKey(connectionStringKey))
            {
                var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect);
                var sqlExecuter = SqlExecuterFactory.CreateSqlExecuter(connectionStringSettings, generator);
                sqlExecutersAndDialects.Add(connectionStringKey, (sqlExecuter, sqlDialect));

                var shouldCreate = Helper.ShouldRunIntegrationTest(sqlDialect);
                if (shouldCreate)
                {
                    sqlExecuter.CreateDatabase(true);
                }
            }

            return connectionStringSettings;
        }

        public void Cleanup()
        {
            var exceptions = new List<Exception>();
            foreach (var sqlExecuterAndDialect in sqlExecutersAndDialects.Values)
            {
                try
                {
                    var shouldDrop = Helper.ShouldRunIntegrationTest(sqlExecuterAndDialect.SqlDialect);
                    if (shouldDrop)
                    {
                        sqlExecuterAndDialect.SqlExecuter.DropDatabase();
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        public string ExecuteNonQuery(string connectionStringKey, string query)
        {
            var connectionStringSettings = Initialize(connectionStringKey);

            var sqlDialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);

            if (!Helper.ShouldRunIntegrationTest(sqlDialect))
                return "Query execution is skipped, integration tests are not running.";

            sqlExecutersAndDialects[connectionStringKey].SqlExecuter.ExecuteNonQuery(query);
            return null;
        }

        public SqlExecuter GetExecuter(string connectionStringKey)
        {
            return sqlExecutersAndDialects[connectionStringKey].SqlExecuter;
        }
    }
}