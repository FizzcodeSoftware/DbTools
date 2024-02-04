﻿using Index = FizzCode.DbTools.DataDefinition.Base.Index;

namespace FizzCode.DbTools.DataDeclaration;
public class IndexNamingDefaultStrategy : IIndexNamingStrategy
{
    public void SetIndexName(Index index)
    {
        if (index.SqlTableOrView.SchemaAndTableName.TableName == null)
            return;

        // TODO view index name?

        var indexNameColumnsPart = string.Join("_", index.SqlColumns.ConvertAll(co => co.SqlColumn.Name));
        index.Name = $"IX_{index.SqlTableOrView.SchemaAndTableName.TableName}_{indexNameColumnsPart}";
    }
}