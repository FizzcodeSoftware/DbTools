﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.Interfaces;

namespace FizzCode.DbTools.DataDefinition.SqlGenerator;
public abstract class AbstractSqlGenerator : ISqlGenerator
{
    public Context Context { get => SqlGeneratorBase.Context; }

    public abstract SqlEngineVersion SqlVersion { get; }

    protected ISqlGeneratorBase SqlGeneratorBase { get; }

    protected AbstractSqlGenerator(ISqlGeneratorBase sqlGeneratorBase)
    {
        SqlGeneratorBase = sqlGeneratorBase;
    }

    public virtual string CreateTable(SqlTable table)
    {
        return CreateTableInternal(table, false);
    }

    public virtual SqlStatementWithParameters CreateSchema(string schemaName)
    {
        if (!string.IsNullOrEmpty(schemaName))
            return $"CREATE SCHEMA {schemaName}";

        return "";
    }

    protected string CreateTableInternal(SqlTable table, bool withForeignKey)
    {
        var sb = new StringBuilder();
        sb.Append("CREATE TABLE ")
            .Append(GetSimplifiedSchemaAndTableName(table.SchemaAndTableNameSafe))
            .AppendLine(" (");

        var idx = 0;
        foreach (var column in table.Columns)
        {
            if (idx++ > 0)
                sb.AppendLine(",");

            sb.Append(GenerateCreateColumn(column));
        }

        sb.AppendLine();

        CreateTablePrimaryKey(table, sb);
        if (withForeignKey)
            CreateTableForeignKey(table, sb);

        sb.AppendLine(")");

        return sb.ToString();
    }

    public virtual SqlStatementWithParameters? CreateDbColumnDescription(SqlColumn column)
    {
        var sqlColumnDescription = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
        if (sqlColumnDescription is null)
            return null;

        var sqlStatementWithParameters = new SqlStatementWithParameters("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value = @Description, @level0type=N'SCHEMA', @level0name=@SchemaName, @level1type=N'TABLE', @level1name = @TableName, @level2type=N'COLUMN', @level2name= @ColumnName");

        sqlStatementWithParameters.Parameters.Add("@Description", sqlColumnDescription.Description);
        sqlStatementWithParameters.Parameters.Add("@SchemaName", column.Table.SchemaAndTableName!.Schema);
        sqlStatementWithParameters.Parameters.Add("@TableName", column.Table.SchemaAndTableName!.TableName);
        sqlStatementWithParameters.Parameters.Add("@ColumnName", column.Name);

        return sqlStatementWithParameters;
    }

    public virtual SqlStatementWithParameters? CreateDbTableDescription(SqlTable table)
    {
        var sqlTableDescription = table.Properties.OfType<SqlTableDescription>().FirstOrDefault();
        if (sqlTableDescription is null)
            return null;

        var sqlStatementWithParameters = new SqlStatementWithParameters("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value = @Description, @level0type=N'SCHEMA', @level0name=@SchemaName, @level1type=N'TABLE', @level1name = @TableName");

        sqlStatementWithParameters.Parameters.Add("@Description", sqlTableDescription.Description);
        sqlStatementWithParameters.Parameters.Add("@SchemaName", table.SchemaAndTableName!.Schema);
        sqlStatementWithParameters.Parameters.Add("@TableName", table.SchemaAndTableName!.TableName);

        return sqlStatementWithParameters;
    }

    public string CreateIndexes(SqlTable table)
    {
        var sb = new StringBuilder();
        foreach (var index in table.Properties.OfType<Index>().ToList())
            sb.AppendLine(CreateIndex(index));

        return sb.ToString();
    }

    public virtual string CreateIndex(Index index)
    {
        var clusteredPrefix = index.Clustered != null
            ? index.Clustered == true
            ? "CLUSTERED "
            : "NONCLUSTERED "
            : null;

        var sb = new StringBuilder();
        sb.Append("CREATE ")
            .Append(index.Unique ? "UNIQUE " : "")
            .Append(clusteredPrefix)
            .Append("INDEX ")
            .Append(GuardKeywords(index.Name!))
            .Append(" ON ")
            .Append(GetSimplifiedSchemaAndTableName(index.SqlTable.SchemaAndTableName!))
            .AppendLine(" (")
            .AppendJoin(", \r\n", index.SqlColumns.Select(c => $"{GuardKeywords(c.SqlColumn.Name!)} {c.OrderAsKeyword}")).AppendLine() // Index column list + asc desc
            .AppendLine(");");

        return sb.ToString();
    }

    protected virtual void CreateTablePrimaryKey(SqlTable table, StringBuilder sb)
    {
        // example: CONSTRAINT [PK_dbo.AddressShort] PRIMARY KEY CLUSTERED ([Id] ASC)
        var pk = table.Properties.OfType<PrimaryKey>().FirstOrDefault();
        if (pk == null || pk.SqlColumns.Count == 0)
            return;

        var clusteredPrefix = pk.Clustered != null
            ? pk.Clustered == true
                ? "CLUSTERED "
                : "NONCLUSTERED "
            : null;

        sb.Append(", CONSTRAINT ")
            .Append(GuardKeywords(pk.Name!))
            .Append(" PRIMARY KEY ")
            .AppendLine(clusteredPrefix)
            .AppendLine("(")
            .AppendJoin(", \r\n", pk.SqlColumns.Select(c => $"{GuardKeywords(c.SqlColumn.Name!)} {c.OrderAsKeyword}")).AppendLine() // PK column list + asc desc
            .Append(')');
    }

    public virtual string? CreateForeignKeys(SqlTable table)
    {
        var allFks = table.Properties.OfType<ForeignKey>().ToList();

        if (allFks.Count == 0)
            return null;

        var sb = new StringBuilder();

        foreach (var fk in allFks)
        {
            sb.Append(CreateForeignKey(fk));
        }

        return sb.ToString();
    }

    public abstract string CreateForeignKey(ForeignKey fk);

    protected string FKConstraint(ForeignKey fk)
    {
        var sb = new StringBuilder();

        sb.Append("CONSTRAINT ")
            .Append(GuardKeywords(fk.Name!))
            .Append(" FOREIGN KEY ")
            .Append('(')
            .AppendJoin(", \r\n", fk.ForeignKeyColumns.Select(fkc => $"{GuardKeywords(fkc.ForeignKeyColumn.Name!)}"))
            .Append(')')
            .Append(" REFERENCES ")
            .Append(GetSimplifiedSchemaAndTableName(fk.ReferredTable!.SchemaAndTableName!))
            .Append(" (")
            .AppendJoin(", \r\n", fk.ForeignKeyColumns.Select(pkc => $"{GuardKeywords(pkc.ReferredColumn.Name!)}"))
            .Append(')');

        return sb.ToString();
    }

    private void CreateTableForeignKey(SqlTable table, StringBuilder sb)
    {
        // example: CONSTRAINT [FK_Training_TrainingCatalog] FOREIGN KEY ([TrainingCatalogId]) REFERENCES [dbo].[TrainingCatalog] ([Id])

        var allFks = table.Properties.OfType<ForeignKey>().ToList();

        if (allFks.Count == 0)
            return;

        sb.AppendLine();

        foreach (var fk in allFks)
        {
            sb.Append(", ")
                .Append(FKConstraint(fk));
        }
    }

    public string CreateUniqueConstrainsts(SqlTable table)
    {
        var sb = new StringBuilder();
        foreach (var uniqueConstraint in table.Properties.OfType<UniqueConstraint>().ToList())
            sb.AppendLine(CreateUniqueConstraint(uniqueConstraint));

        return sb.ToString();
    }

    public string CreateUniqueConstraint(UniqueConstraint uniqueConstraint)
    {
        var clusteredPrefix = uniqueConstraint.Clustered != null
            ? uniqueConstraint.Clustered == true
            ? "CLUSTERED "
            : "NONCLUSTERED "
            : null;

        var sb = new StringBuilder();
        sb.Append("ALTER TABLE ")
            .Append(GetSimplifiedSchemaAndTableName(uniqueConstraint.SqlTable.SchemaAndTableName!))
            .Append(" ADD CONSTRAINT ")
            .Append(GuardKeywords(uniqueConstraint.Name!))
            .Append(" UNIQUE ")
            .Append(clusteredPrefix)

            .AppendLine(" (")
            .AppendJoin(", \r\n", uniqueConstraint.SqlColumns.Select(c => $"{GuardKeywords(c.SqlColumn.Name!)}")).AppendLine() // Index column list
            .AppendLine(");");

        return sb.ToString();
    }

    public string DropTable(SqlTable table)
    {
        return $"DROP TABLE {GetSimplifiedSchemaAndTableName(table.SchemaAndTableName!)}";
    }

    public string GenerateCreateColumn(SqlColumn column)
    {
        var type = column.Types[SqlVersion];

        var sb = new StringBuilder();
        sb.Append(GuardKeywords(column.Name!))
            .Append(' ');

        sb.Append(GenerateType(type));

        var identity = column.Properties.OfType<Identity>().FirstOrDefault();
        if (identity != null)
        {
            GenerateCreateColumnIdentity(sb, identity);
        }

        sb.Append(GenerateDefault(column));

        if (type.IsNullable)
            sb.Append(" NULL");
        else
            sb.Append(" NOT NULL");

        return sb.ToString();
    }

    public virtual string GenerateDefault(SqlColumn column)
    {
        var sb = new StringBuilder();
        var defaultValue = column.Properties.OfType<DefaultValue>().FirstOrDefault();
        if (defaultValue != null)
        {
            sb.Append(" DEFAULT(")
                .Append(defaultValue.Value)
                .Append(')');
        }

        return sb.ToString();
    }

    public virtual string GenerateType(ISqlType type)
    {
        var sb = new StringBuilder();
        sb.Append(type.SqlTypeInfo.SqlDataType);

        if (type.Scale.HasValue)
        {
            if (type.Length != null)
            {
                sb.Append('(')
                    .Append(type.Length?.ToString("D", CultureInfo.InvariantCulture))
                    .Append(", ")
                    .Append(type.Scale?.ToString("D", CultureInfo.InvariantCulture))
                    .Append(')');
            }
            else
            {
                sb.Append('(')
                    .Append(type.Scale?.ToString("D", CultureInfo.InvariantCulture))
                    .Append(')');
            }
        }
        else if (type.Length.HasValue)
        {
            sb.Append('(');
            if (type.Length == -1)
                sb.Append("MAX");
            else
                sb.Append(type.Length?.ToString("D", CultureInfo.InvariantCulture));

            sb.Append(')');
        }

        return sb.ToString();
    }

    public virtual void GenerateCreateColumnIdentity(StringBuilder sb, Identity identity)
    {
        sb.Append(" IDENTITY(")
            .Append(identity.Seed)
            .Append(',')
            .Append(identity.Increment)
            .Append(')');
    }

    public virtual string CreateStoredProcedure(StoredProcedure sp)
    {
        var sb = new StringBuilder();
        sb.Append("CREATE PROCEDURE ")
            .AppendLine(GetSimplifiedSchemaAndTableName(sp.SchemaAndSpName!));

        var idx = 0;
        foreach (var p in sp.SpParameters)
        {
            if (idx++ > 0)
                sb.AppendLine(",");

            sb.Append('@')
                .Append(p.Name)
                .Append(' ');
            sb.Append(GenerateType(p.Type!));
        }

        // EXECUTE AS
        sb.AppendLine();
        sb.AppendLine("AS");

        sb.Append(sp.StoredProcedureBodies[SqlVersion]);

        return sb.ToString();
    }

    public virtual string CreateView(SqlView view)
    {
        var sb = new StringBuilder();
        sb.Append("CREATE VIEW ")
            .AppendLine(GetSimplifiedSchemaAndTableName(view.SchemaAndTableName!));

        sb.AppendLine();
        sb.AppendLine("AS");

        sb.Append(view.SqlViewBodies[SqlVersion]);

        return sb.ToString();
    }

    public abstract string DropAllForeignKeys();

    public abstract string DropAllViews();

    public abstract string DropAllTables();

    public abstract string DropAllIndexes();

    public abstract SqlStatementWithParameters DropSchemas(List<string> schemaNames, bool hard = false);

    public virtual SqlStatementWithParameters TableExists(SqlTable table)
    {
        Throw.InvalidOperationExceptionIfNull(table.SchemaAndTableName);
        Throw.InvalidOperationExceptionIfNull(table.SchemaAndTableName.Schema);

        return new SqlStatementWithParameters(@"
SELECT
    CASE WHEN EXISTS(SELECT * FROM information_schema.tables WHERE table_schema = @ShemaName AND table_name = @TableName)
        THEN 1
        ELSE 0
    END", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName);
    }

    public SqlStatementWithParameters SchmaExists(SqlTable table)
    {
        return new SqlStatementWithParameters(@"
SELECT
    CASE WHEN EXISTS(SELECT schema_name FROM information_schema.schemata WHERE schema_name = @SchemaName)
        THEN 1
        ELSE 0
    END", GetSchema(table));
    }

    public string TableNotEmpty(SqlTable table)
    {
        return $"SELECT COUNT(*) FROM (SELECT TOP 1 * FROM {GetSimplifiedSchemaAndTableName(table.SchemaAndTableNameSafe)}) t";
    }

    public string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName)
    {
        return SqlGeneratorBase.GetSimplifiedSchemaAndTableName(schemaAndTableName);
    }

    public string? GetSchema(SqlTable table)
    {
        return SqlGeneratorBase.GetSchema(table);
    }

    public string GuardKeywords(string name)
    {
        return SqlGeneratorBase.GuardKeywords(name);
    }

    public string GuardKeywordsImplementation(string name)
    {
        throw new System.NotImplementedException();
    }
}