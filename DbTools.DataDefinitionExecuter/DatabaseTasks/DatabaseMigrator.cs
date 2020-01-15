﻿namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Migration;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class DatabaseMigrator : DatabaseTask
    {
        public DatabaseMigrator(SqlExecuter sqlExecuter, ISqlMigrationGenerator migrationGenerator) : base(sqlExecuter)
        {
            MigrationGenerator = migrationGenerator;
        }

        protected ISqlMigrationGenerator MigrationGenerator { get; }

        public static DatabaseMigrator FromConnectionStringSettings(ConnectionStringWithProvider connectionStringWithProvider, Context context)
        {
            var sqlDialect = SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName);

            // TODO version detection?
            var version = SqlEngines.GetLatestVersion(sqlDialect);

            var generator = SqlGeneratorFactory.CreateGenerator(version, context);
            var migrationGenerator = SqlGeneratorFactory.CreateMigrationGenerator(version, context);

            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);

            return new DatabaseMigrator(executer, migrationGenerator);
        }

        public void NewTable(TableNew tableNew)
        {
            var sql = MigrationGenerator.CreateTable(tableNew);
            Executer.ExecuteNonQuery(sql);
        }

        public void DeleteTable(TableDelete tableDelete)
        {
            var sql = MigrationGenerator.DropTable(tableDelete);
            Executer.ExecuteNonQuery(sql);
        }
    }
}