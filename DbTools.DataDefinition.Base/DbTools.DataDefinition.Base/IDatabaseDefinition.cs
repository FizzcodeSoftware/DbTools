﻿namespace FizzCode.DbTools.DataDefinition.Base
{
    using System.Collections.Generic;
    using FizzCode.DbTools;

    public interface IDatabaseDeclaration
    {
        void CreateRegisteredForeignKeys(SqlTable sqlTable);
        void AddAutoNaming(List<SqlTable> tables);
    }

    public interface IDatabaseDefinition
    {
        SqlEngineVersion MainVersion { get; }
        List<StoredProcedure> StoredProcedures { get; }
        Dictionary<SqlEngineVersion, AbstractTypeMapper> TypeMappers { get; set; }

        void AddTable(SqlTable sqlTable);
        void AddView(SqlView sqlTable);
        bool Contains(SchemaAndTableName schemaAndTableName);
        bool Contains(string schema, string tableName);
        IEnumerable<string> GetSchemaNames();
        SqlTable GetTable(SchemaAndTableName schemaAndTableName);
        SqlTable GetTable(string tableName);
        SqlTable GetTable(string schema, string tableName);
        List<SqlTable> GetTables();
        List<SqlView> GetViews();
        void SetVersions(AbstractTypeMapper mainTypeMapper, AbstractTypeMapper[] secondaryTypeMappers = null);
    }
}