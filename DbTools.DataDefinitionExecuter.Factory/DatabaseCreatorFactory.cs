﻿namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public static class DatabaseCreatorFactory
    {
        public static DatabaseCreator FromConnectionStringSettings(DatabaseDefinition databaseDefinition, ConnectionStringWithProvider connectionStringWithProvider, Context context)
        {
            var generator = SqlGeneratorFactory.CreateGenerator(connectionStringWithProvider.SqlEngineVersion, context);
            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);
            return new DatabaseCreator(databaseDefinition, executer);
        }
    }
}