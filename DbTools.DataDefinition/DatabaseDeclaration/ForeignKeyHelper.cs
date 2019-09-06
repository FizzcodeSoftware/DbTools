﻿using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinition
{
    public static class ForeignKeyHelper
    {
        /// <summary>
        /// Sets an existing column as an FK, pointing to the PK of <paramref name="lazyReferredTable"/>.
        /// </summary>
        /// <param name="singleFkColumn"></param>
        /// <param name="lazyReferredTable"></param>
        /// <returns>The original <paramref name="singleFkColumn"/>.</returns>
        public static SqlColumn SetForeignKeyTo(this SqlColumn singleFkColumn, string referredTableName, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKey(singleFkColumn.Table, referredTableNameWithSchema, fkName);
            singleFkColumn.Table.Properties.Add(fk);

            fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk, singleFkColumn, null));

            return singleFkColumn;
        }

        public static SqlTable AddForeignKey(this SqlTable table, string referredTableName, bool isNullable = false, string prefix = null, string fkName = null, List<ForeignKeyGroup> map = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyToPrimaryKey(table, referredTableNameWithSchema, isNullable, prefix, fkName, map);
            table.Properties.Add(fk);

            return table;
        }
    }
}