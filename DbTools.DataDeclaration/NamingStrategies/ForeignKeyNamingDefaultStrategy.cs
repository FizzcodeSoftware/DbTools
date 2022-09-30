﻿namespace FizzCode.DbTools.DataDeclaration
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;

    public class ForeignKeyNamingDefaultStrategy : IForeignKeyNamingStrategy
    {
        public virtual void SetFKName(ForeignKey fk)
        {
            if (fk.SqlTable.SchemaAndTableName == null)
                return;

            fk.Name = $"FK_{fk.SqlTable.SchemaAndTableName.TableName}__{string.Join("__", fk.ForeignKeyColumns.Select(y => y.ForeignKeyColumn.Name))}";
        }

        public virtual string GetFkToPkColumnName(SqlColumn referredColumn, string prefix)
        {
            if (prefix != null)
            {
                return prefix + referredColumn.Name;
            }

            return $"{referredColumn.Table.SchemaAndTableName}{referredColumn.Name}";
        }
    }
}