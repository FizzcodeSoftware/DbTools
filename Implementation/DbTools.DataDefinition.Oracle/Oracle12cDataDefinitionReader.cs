﻿using System.Collections.Generic;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.Common.Logger;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinitionReader;
using FizzCode.DbTools.SqlExecuter.Oracle;
using FizzCode.LightWeight;

namespace FizzCode.DbTools.DataDefinition.Oracle12c;
public class Oracle12cDataDefinitionReader(NamedConnectionString connectionString, ContextWithLogger context, ISchemaNamesToRead? schemaNames)
    : GenericDataDefinitionReader(new Oracle12cExecuter(context, connectionString, new Oracle12cGenerator(context)), schemaNames)
{
    public override DatabaseDefinition GetDatabaseDefinition()
    {
        return GetDatabaseDefinition(new DatabaseDefinition(OracleVersion.Oracle12c, GenericVersion.Generic1));
    }

    public override DatabaseDefinition GetDatabaseDefinition(IDatabaseDefinition idd)
    {
        var dd = (DatabaseDefinition)idd;

        Log(LogSeverity.Debug, "Reading table definitions from database.");

        foreach (var schemaAndTableName in GetSchemaAndTableNames())
            dd.AddTable(GetTableDefinition(schemaAndTableName, false));

        Log(LogSeverity.Debug, "Reading table identities from database.");
        new OracleIdentityReader12c(Executer, SchemaNames).GetIdentity(dd);
        Log(LogSeverity.Debug, "Reading table primary keys from database.");
        new OraclePrimaryKeyReader12c(Executer, SchemaNames).GetPrimaryKey(dd);
        Log(LogSeverity.Debug, "Reading table foreign keys including unique constrints from database.", "Reader");
        new OracleForeignKeyReader12c(Executer, SchemaNames).GetForeignKeysAndUniqueConstrainsts(dd);
        Log(LogSeverity.Debug, "Reading indexes from database.");
        new OracleIndexReader12c(Executer, SchemaNames).GetIndexes(dd);

        Log(LogSeverity.Debug, "Reading views from database.");
        foreach (var schemaAndTableName in GetViews())
            dd.AddView(GetViewDefinition(schemaAndTableName, false));

        return dd;
    }

    public override List<SchemaAndTableName> GetSchemaAndTableNames()
    {
        return new OracleTablesReader(Executer, SchemaNames).GetSchemaAndTableNames();
    }

    private OracleTableReader12c _tableReader = null!;

    private OracleTableReader12c TableReader => _tableReader ??= new OracleTableReader12c(Executer, SchemaNames);

    public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition)
    {
        var sqlTable = TableReader.GetTableDefinition(schemaAndTableName);

        if (fullDefinition)
        {
            new OraclePrimaryKeyReader12c(Executer, SchemaNames).
            GetPrimaryKey(sqlTable);
            new OracleForeignKeyReader12c(Executer, SchemaNames).GetForeignKeys(sqlTable);
            // TODO
            //AddTableDocumentation(sqlTable);
        }
        // TODO 
        // ColumnDocumentationReader.GetColumnDocumentation(sqlTable);

        sqlTable.SchemaAndTableName = GetSchemaAndTableNameAsToStore(sqlTable.SchemaAndTableNameSafe, Executer.Context);

        return sqlTable;
    }

    public override List<SchemaAndTableName> GetViews()
    {
        return new OracleViewsReader(Executer, SchemaNames).GetSchemaAndTableNames();
    }

    private OracleViewReader12c _viewReader = null!;

    private OracleViewReader12c ViewReader => _viewReader ??= new OracleViewReader12c(Executer, SchemaNames);

    public override SqlView GetViewDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition)
    {
        var sqlView = ViewReader.GetViewDefinition(schemaAndTableName);

        sqlView.SchemaAndTableName = GetSchemaAndTableNameAsToStore(sqlView.SchemaAndTableNameSafe, Executer.Context);
        return sqlView;
    }
}
