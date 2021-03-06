﻿namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using System;
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;
    using FizzCode.DbTools.DataDefinitionReader;
    using FizzCode.LightWeight.AdoNet;

    public class MsSql2016DataDefinitionReader : GenericDataDefinitionReader
    {
        public MsSql2016DataDefinitionReader(NamedConnectionString connectionString, Context context, SchemaNamesToRead schemaNames)
            : base(new MsSql2016Executer(connectionString, new MsSql2016Generator(context)), schemaNames)
        {
        }

        public override DatabaseDefinition GetDatabaseDefinition()
        {
            var dd = new DatabaseDefinition(new MsSql2016TypeMapper(), new[] { GenericVersion.Generic1.GetTypeMapper() });

            Log(LogSeverity.Debug, "Reading table definitions from database.");

            var module = "Reader/" + Executer.Generator.Version.UniqueName;
            var logTimer = new LogTimer(Logger, LogSeverity.Debug, "Reading table definitions from database.", module);

            foreach (var schemaAndTableName in GetSchemaAndTableNames())
                dd.AddTable(GetTableDefinition(schemaAndTableName, false));

            Log(LogSeverity.Debug, "Reading table documentation from database.");
            AddTableDocumentation(dd);
            Log(LogSeverity.Debug, "Reading table identities from database.");
            new MsSqlIdentityReader2016(Executer, SchemaNames).GetIdentity(dd);
            Log(LogSeverity.Debug, "Reading table indexes including primary keys and unique constraints from database.");
            new MsSqlIndexReader2016(Executer, SchemaNames).GetIndexes(dd);
            Log(LogSeverity.Debug, "Reading table foreign keys from database.", "Reader");
            new MsSqlForeignKeyReader2016(Executer, SchemaNames).GetForeignKeys(dd);

            logTimer.Done();

            return dd;
        }

        public override List<SchemaAndTableName> GetSchemaAndTableNames()
        {
            var sqlStatement = @"
SELECT ss.name schemaName, so.name tableName FROM sys.objects so
INNER JOIN sys.schemas ss ON ss.schema_id = so.schema_id
WHERE type = 'U'";

            AddSchemaNamesFilter(ref sqlStatement, "ss.name");

            return Executer.ExecuteQuery(sqlStatement).Rows
                .ConvertAll(row => new SchemaAndTableName(row.GetAs<string>("schemaName"), row.GetAs<string>("tableName")))
;
        }

        private MsSqlTableReader2016 _tableReader;
        private MsSqlTableReader2016 TableReader => _tableReader ??= new MsSqlTableReader2016(Executer, SchemaNames);

        private MsSqlColumnDocumentationReader2016 _columnDocumentationReader;
        private MsSqlColumnDocumentationReader2016 ColumnDocumentationReader => _columnDocumentationReader ??= new MsSqlColumnDocumentationReader2016(Executer);

        public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true)
        {
            var sqlTable = TableReader.GetTableDefinition(schemaAndTableName);

            if (fullDefinition)
            {
                new MsSqlIndexReader2016(Executer, SchemaNames).
                GetPrimaryKey(sqlTable);
                new MsSqlForeignKeyReader2016(Executer, SchemaNames).GetForeignKeys(sqlTable);
                AddTableDocumentation(sqlTable);
            }

            ColumnDocumentationReader.GetColumnDocumentation(sqlTable);

            sqlTable.SchemaAndTableName = GetSchemaAndTableNameAsToStore(sqlTable.SchemaAndTableName, Executer.Generator.Context);
            return sqlTable;
        }

        private readonly string SqlGetTableDocumentation = @"
SELECT
    t.name TableName, 
    p.value Property
FROM
    sys.tables AS t
    INNER JOIN sys.extended_properties AS p ON p.major_id = t.object_id AND p.minor_id = 0 AND p.class = 1
    WHERE p.name = 'MS_Description'";

        public void AddTableDocumentation(SqlTable table)
        {
            var reader = Executer.ExecuteQuery(new SqlStatementWithParameters(
            SqlGetTableDocumentation + " AND SCHEMA_NAME(t.schema_id) = @SchemaName AND t.name = @TableName", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName));

            foreach (var row in reader.Rows)
            {
                var description = row.GetAs<string>("Property");
                if (!string.IsNullOrEmpty(description))
                {
                    description = description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase).Trim();
                    var descriptionProperty = new SqlTableDescription(table, description);
                    table.Properties.Add(descriptionProperty);
                }
            }
        }

        public void AddTableDocumentation(DatabaseDefinition dd)
        {
            var reader = Executer.ExecuteQuery(@"
SELECT
    SCHEMA_NAME(t.schema_id) as SchemaName,
    t.name AS TableName, 
    p.value AS Property
FROM
    sys.tables AS t
    INNER JOIN sys.extended_properties AS p ON p.major_id = t.object_id AND p.minor_id = 0 AND p.class = 1
    AND p.name = 'MS_Description'");

            var tables = dd.GetTables();

            foreach (var row in reader.Rows)
            {
                // TODO SchemaAndTableName.Schema might be null on default schema?
                var table = tables.Find(t => t.SchemaAndTableName.Schema == row.GetAs<string>("SchemaName") && t.SchemaAndTableName.TableName == row.GetAs<string>("TableName"));
                if (table != null)
                {
                    var description = row.GetAs<string>("Property");
                    if (!string.IsNullOrEmpty(description))
                    {
                        description = description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase).Trim();
                        var descriptionProperty = new SqlTableDescription(table, description);
                        table.Properties.Add(descriptionProperty);
                    }
                }
            }
        }
    }
}
