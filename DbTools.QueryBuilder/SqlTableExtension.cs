﻿using System.Linq;
using System.Reflection;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
public static class SqlTableExtension
{
    public static T Alias<T>(this T table, string? alias) where T : SqlTable, new()
    {
        var newTable = new T
        {
            DatabaseDefinition = table.DatabaseDefinition,
            SchemaAndTableName = table.SchemaAndTableName
        };

        newTable.Properties.AddRange(table.Properties);

        foreach (var column in table.Columns)
        {
            var newSqlColumn = new SqlColumn();
            column.CopyTo(newSqlColumn);
            newSqlColumn.Table = newTable;
            newTable.Columns.Add(newSqlColumn);
        }

        SqlTableHelper.SetAlias(newTable, alias);
        UpdateDeclaredColumns(newTable);

        // TODO
        /*
        AddDeclaredForeignKeys(table);
        UpdateDeclaredIndexes(table);
        UpdateDeclaredCustomProperties(table);
        */

        return newTable;
    }

    private static void UpdateDeclaredColumns<T>(T table) where T : SqlTable
    {
        var properties = table.GetType()
           .GetProperties(BindingFlags.Public | BindingFlags.Instance)
           .Where(pi => typeof(SqlColumn).IsAssignableFrom(pi.PropertyType) && pi.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var columnDeclared = property.GetValueSafe<SqlColumn>(table);
            var column = table[property.Name];

            //column.CopyTo(columnDeclared);
            columnDeclared.Name = column.Name;

            //Types.CopyTo(column.Types);
            foreach (var kvp in column.Types)
            {
                columnDeclared.Types[kvp.Key] = (SqlType)kvp.Value.Copy();
            }
            columnDeclared.SqlTableOrView = column.SqlTableOrView;

        }
    }

    public static T AliasView<T>(this T table, string? alias) where T : SqlView, new()
    {
        var newView = new T
        {
            DatabaseDefinition = table.DatabaseDefinition,
            SchemaAndTableName = table.SchemaAndTableName
        };

        newView.Properties.AddRange(table.Properties);

        foreach (var column in table.Columns)
        {
            var newSqlColumn = new SqlViewColumn();
            column.CopyTo(newSqlColumn);
            newSqlColumn.View = newView;
            newView.Columns.Add(newSqlColumn);
        }

        SqlTableHelper.SetAlias(newView, alias);

        // TODO as above with table

        return newView;
    }
}
