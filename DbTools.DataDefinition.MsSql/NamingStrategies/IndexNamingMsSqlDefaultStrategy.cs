﻿namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    public class IndexNamingMsSqlDefaultStrategy : IIndexNamingStrategy
    {
        public void SetIndexName(Index index)
        {
            if (index.SqlTable.SchemaAndTableName.TableName == null)
                return;

            var indexNameColumnsPart = string.Join("_", index.SqlColumns.ConvertAll(co => co.SqlColumn.Name));
            index.Name = $"IX_{index.SqlTable.SchemaAndTableName.TableName}_{indexNameColumnsPart}";
        }
    }
}