﻿namespace FizzCode.DbTools.DataDefinition.Base;
public static class PrimaryKeyHelper
{
    public static void SetPK(this SqlTable table, SqlColumn column, AscDesc order = AscDesc.Asc, string? name = null)
    {
        var pk = table.Properties.OfType<PrimaryKey>().FirstOrDefault();
        if (pk is null)
        {
            pk = new PrimaryKey(table, name);
            table.Properties.Add(pk);
        }

        pk.SqlColumns.Add(new ColumnAndOrder(column, order));
    }
}