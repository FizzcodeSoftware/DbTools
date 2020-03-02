﻿namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Configuration;

    public class DatabaseDefinition
    {
        public Dictionary<SqlEngineVersion, AbstractTypeMapper> TypeMappers { get; set; } = new Dictionary<SqlEngineVersion, AbstractTypeMapper>();
        public SqlEngineVersion MainVersion { get; private set; }
        internal Tables Tables { get; } = new Tables();

        public DatabaseDefinition(AbstractTypeMapper mainTypeMapper, AbstractTypeMapper[] secondaryTypeMappers = null)
        {
            SetVersions(mainTypeMapper, secondaryTypeMappers);
        }

        public void SetVersions(AbstractTypeMapper mainTypeMapper, AbstractTypeMapper[] secondaryTypeMappers = null)
        {
            TypeMappers.Clear();
            MainVersion = mainTypeMapper?.SqlVersion;

            if (mainTypeMapper != null && (secondaryTypeMappers?.Contains(mainTypeMapper) != true))
                TypeMappers.Add(mainTypeMapper.SqlVersion, mainTypeMapper);

            if (secondaryTypeMappers != null)
            {
                foreach (var mapper in secondaryTypeMappers)
                {
                    TypeMappers.Add(mapper.SqlVersion, mapper);
                }
            }
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

        public virtual List<SqlTable> GetTables()
        {
            return Tables.ToList();
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

        public IEnumerable<string> GetSchemaNames()
        {
            var schemas = GetTables()
                .Select(t => t.SchemaAndTableName.Schema)
                .Distinct()
                .Where(sn => !string.IsNullOrEmpty(sn));

            return schemas;
        }
    }
}