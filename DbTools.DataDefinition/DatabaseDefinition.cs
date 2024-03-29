﻿using System.Collections.Generic;
using System.Linq;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.Factory;
using FizzCode.DbTools.Factory.Interfaces;

namespace FizzCode.DbTools.DataDefinition;
public class DatabaseDefinition : IDatabaseDefinition
{
    public TypeMappers TypeMappers { get; }
    public SqlEngineVersion MainVersion { get; protected set; }
    public List<SqlEngineVersion>? SecondaryVersions { get; protected set; }
    public Tables Tables { get; } = [];
    protected Views Views { get; } = [];

    protected IFactoryContainer FactoryContainer;

    public List<StoredProcedure> StoredProcedures { get; } = [];

    public DatabaseDefinition(SqlEngineVersion mainVersion, params SqlEngineVersion[] secondaryVersions)
        : this(new Root(), mainVersion, secondaryVersions)
    {
    }

    public DatabaseDefinition(IFactoryContainer factoryContainer, SqlEngineVersion mainVersion, params SqlEngineVersion[]? secondaryVersions)
    {
        FactoryContainer = factoryContainer;

        MainVersion = mainVersion;
        SecondaryVersions = secondaryVersions?.ToList();
        var typeMapperFactory = FactoryContainer.Get<ITypeMapperFactory>();
        TypeMappers = new TypeMappers(typeMapperFactory);
    }

    public void AddTable(SqlTable sqlTable)
    {
        sqlTable.DatabaseDefinition = this;

        foreach (var column in sqlTable.Columns)
        {
            SqlColumnHelper.MapFromGen1(column);
        }

        Tables.Add(sqlTable);
    }

    public void AddView(SqlView sqlTable)
    {
        sqlTable.DatabaseDefinition = this;

        foreach (var column in sqlTable.Columns)
        {
            SqlColumnHelper.MapFromGen1(column);
        }

        Views.Add(sqlTable);
    }

    public virtual List<SqlTable> GetTables()
    {
        return Tables.ToList();
    }

    public virtual List<SqlView> GetViews()
    {
        return Views.ToList();
    }

    public SqlTable GetTable(string schema, string tableName)
    {
        return Tables[SchemaAndTableName.Concat(schema, tableName)];
    }

    public SqlTable GetTable(string tableName)
    {
        return Tables[tableName];
    }

    public SqlTable GetTable(SchemaAndTableName schemaAndTableName)
    {
        return Tables[schemaAndTableName.SchemaAndName];
    }

    public bool Contains(SchemaAndTableName schemaAndTableName)
    {
        return Tables.ContainsKey(schemaAndTableName.SchemaAndName);
    }

    public bool Contains(string schema, string tableName)
    {
        return Contains(SchemaAndTableName.Concat(schema, tableName));
    }

    public IEnumerable<string> GetSchemaNames()
    {
        var schemas = GetTables()
            .Select(t => t.SchemaAndTableNameSafe.Schema)
            .Distinct()
            .Where(sn => !string.IsNullOrEmpty(sn));

        return schemas!;
    }
}