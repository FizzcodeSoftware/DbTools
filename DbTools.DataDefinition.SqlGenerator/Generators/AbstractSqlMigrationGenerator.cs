﻿namespace FizzCode.DbTools.DataDefinition.SqlGenerator
{
    using System;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Migration;

    public abstract class AbstractSqlMigrationGenerator : ISqlMigrationGenerator
    {
        public Context Context { get; }
        public ISqlGenerator Generator { get; }

        protected AbstractSqlMigrationGenerator(Context context, ISqlGenerator generator)
        {
            Context = context;
            Generator = generator;
        }

        public virtual string DropTable(TableDelete tableDelete)
        {
            // Remove indexes, Remove FKs
            // + normal DropTable

            return Generator.DropTable(tableDelete);
        }

        public virtual string RenameTable(TableRename tableRename)
        {
            throw new NotImplementedException();
        }

        public virtual string CreateTable(TableNew tableNew)
        {
            // TODO Properties (PKs, FKs, Indexes, Defaults, Descriptions)
            return Generator.CreateTable(tableNew);
        }

        public virtual string DropColumns(params ColumnDelete[] columnDeletes)
        {
            var tableName = CheckSameTable(columnDeletes);

            var columnsToDelete = columnDeletes.Select(c => Generator.GuardKeywords(c.Name)).ToList();
            return @$"
ALTER TABLE {Generator.GetSimplifiedSchemaAndTableName(tableName)}
DROP COLUMN { string.Join(", ", columnsToDelete) }";
        }

        public virtual string CreateColumns(params ColumnNew[] columnNews)
        {
            var tableName = CheckSameTable(columnNews);

            var columnsToAdd = columnNews.Select(c => c.Name).ToList();

            var sb = new StringBuilder();
            sb.Append("ALTER TABLE ").AppendLine(Generator.GetSimplifiedSchemaAndTableName(tableName));
            sb.Append("ADD ");

            var idx = 0;
            foreach (var columnNew in columnNews)
            {
                if (idx++ > 0)
                    sb.AppendLine(",");

                sb.AppendLine(Generator.GenerateCreateColumn(columnNew));
            }

            return sb.ToString();
        }

        protected static SchemaAndTableName CheckSameTable(ColumnMigration[] columnNews)
        {
            var tableNames = columnNews.Select(c => c.Table.SchemaAndTableName).Distinct();

            if (tableNames.Count() != 1)
                throw new ArgumentOutOfRangeException(nameof(columnNews), "All columns should be on the same table.");

            return tableNames.First();
        }

        public virtual SqlStatementWithParameters ChangeColumns(params ColumnChange[] columnChanges)
        {
            var tableName = CheckSameTable(columnChanges);

            if (columnChanges.Length == 1)
            {
                return $@"
ALTER TABLE {Generator.GetSimplifiedSchemaAndTableName(tableName)}
ALTER COLUMN {Generator.GenerateCreateColumn(columnChanges[0].NewNameAndType)}";
            }

            var sbStatements = new StringBuilder();
            // TODO Options ShouldMigrateColumnChangesAllAtOnce
            // TODO multiple -> temp table
            // TODO drop constraints then re add them
            foreach (var columnChange in columnChanges)
            {
                sbStatements.AppendLine(ChangeColumns(columnChange).Statement);
            }

            return new SqlStatementWithParameters(sbStatements.ToString());
        }
    }
}