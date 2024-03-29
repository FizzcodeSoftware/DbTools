﻿using FizzCode.DbTools.DataDeclaration;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition.MsSql2016;
public class UniqueConstraintNamingMsSqlDefaultStrategy : IUniqueConstraintNamingStrategy
{
    public void SetUniqueConstraintName(UniqueConstraint uniqueConstraint)
    {
        if (uniqueConstraint.SqlTable.SchemaAndTableNameSafe.TableName is null)
            return;

        var indexNameColumnsPart = string.Join("_", uniqueConstraint.SqlColumns.ConvertAll(co => co.SqlColumn.Name));
        uniqueConstraint.Name = $"IX_{uniqueConstraint.SqlTable.SchemaAndTableNameSafe.TableName}_{indexNameColumnsPart}";
    }
}