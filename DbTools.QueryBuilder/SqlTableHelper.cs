﻿using System.Linq;
using System.Reflection;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
public static class SqlTableHelper
{
    internal static void SetAliasProperty(this SqlTable sqlTable, string alias)
    {
        var aliasProperty = sqlTable.Properties.OfType<AliasTableProperty>().FirstOrDefault();
        if (aliasProperty is null)
        {
            aliasProperty = new AliasTableProperty(alias);
            sqlTable.Properties.Add(aliasProperty);
        }
        else
        {
            aliasProperty.Alias = alias;
        }
    }

    internal static void SetAliasProperty(this SqlView sqlView, string alias)
    {
        var aliasProperty = sqlView.Properties.OfType<AliasViewProperty>().FirstOrDefault();
        if (aliasProperty is null)
        {
            aliasProperty = new AliasViewProperty(alias);
            sqlView.Properties.Add(aliasProperty);
        }
        else
        {
            aliasProperty.Alias = alias;
        }
    }

    public static string? GetAlias(this SqlTableOrView sqlTableOrView)
    {
        return sqlTableOrView switch
        {
            SqlTable sqlTable => GetAlias(sqlTable),
            SqlView sqlView => GetAlias(sqlView),
            _ => throw new System.ArgumentException("Unknown SqlTableOrView Type.")
        };
    }

    public static string? GetAlias(this SqlTable sqlTable)
    {
        var aliasTableProperty = sqlTable.Properties.OfType<AliasTableProperty>().FirstOrDefault();
        return aliasTableProperty?.Alias;
    }

    public static string? GetAlias(this SqlView sqlTable)
    {
        var aliasProperty = sqlTable.Properties.OfType<AliasViewProperty>().FirstOrDefault();
        return aliasProperty?.Alias;
    }

    internal static void SetAlias(SqlTable table, string? alias)
    {
        if (!string.IsNullOrEmpty(alias))
        {
            table.SetAliasProperty(alias);
            return;
        }

        var tableName = table.SchemaAndTableNameSafe.TableName;
        var capitals = new string(tableName.Where(c => char.IsUpper(c)).ToArray());

#pragma warning disable CA1308 // Normalize strings to uppercase
        table.SetAliasProperty(capitals.Length > 0 ? capitals.ToLowerInvariant()
            : alias ?? tableName.Substring(0, 1).ToLowerInvariant());
#pragma warning restore CA1308 // Normalize strings to uppercase
    }

    internal static void SetAlias(SqlView table, string? alias)
    {
        if (!string.IsNullOrEmpty(alias))
        {
            table.SetAliasProperty(alias);
            return;
        }

        var tableName = table.SchemaAndTableNameSafe.TableName;
        var capitals = new string(tableName.Where(c => char.IsUpper(c)).ToArray());

#pragma warning disable CA1308 // Normalize strings to uppercase
        table.SetAliasProperty(capitals.Length > 0 ? capitals.ToLowerInvariant()
            : alias ?? tableName.Substring(0, 1).ToLowerInvariant());
#pragma warning restore CA1308 // Normalize strings to uppercase
    }

    internal static void SetupDeclaredTable(SqlTable table)
    {
        AddDeclaredColumns(table);
        AddDeclaredForeignKeys(table);
        UpdateDeclaredIndexes(table);
        UpdateDeclaredCustomProperties(table);
    }

    private static void AddDeclaredColumns(SqlTable table)
    {
        var properties = table.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi => typeof(SqlColumn).IsAssignableFrom(pi.PropertyType) && pi.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var column = property.GetValueSafe<SqlColumn>(table);
            column.Name = property.Name;
            column.Table = table;
        }
    }

    private static void AddDeclaredForeignKeys(SqlTable table)
    {
        var properties = table.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi =>
                typeof(ForeignKey).IsAssignableFrom(pi.PropertyType)
                && pi.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var fk = property.GetValueSafe<ForeignKey>(table);

            if (!property.Name.StartsWith('_'))
                fk.Name = property.Name;

            fk.SqlTableOrView = table;
        }
    }

    private static void UpdateDeclaredIndexes(SqlTable table)
    {
        var properties = table.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi =>
                (typeof(Index).IsAssignableFrom(pi.PropertyType)
                || typeof(UniqueConstraint).IsAssignableFrom(pi.PropertyType))
                && pi.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var index = property.GetValueSafe<Index>(table);

            if (!property.Name.StartsWith('_'))
                index.Name = property.Name;

            index.SqlTableOrView = table;

            var registeredIdexes = index.SqlColumnRegistrations.ToList();

            foreach (var cr in registeredIdexes)
            {
                index.SqlColumnRegistrations.Remove(cr);
                index.SqlColumns.Add(new ColumnAndOrder(table.Columns[cr.ColumnName], cr.Order));
            }
        }
    }

    private static void UpdateDeclaredCustomProperties(SqlTable table)
    {
        var properties = table.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi =>
                typeof(SqlTableCustomProperty).IsAssignableFrom(pi.PropertyType)
                && pi.GetIndexParameters().Length == 0);

        // TODO ?
        foreach (var property in properties)
        {
            var customProperty = property.GetValueSafe<SqlTableCustomProperty>(table);
            customProperty.SqlTableOrView = table;
        }
    }
}
