﻿namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public class CSharpTypedGenerator : AbstractCSharpGenerator
    {
        public CSharpTypedGenerator(AbstractCSharpTypedWriter writer, SqlEngineVersion version, string databaseName, string @namespace)
            : base(writer, version, databaseName, @namespace)
        {
        }

        protected override void GenerateTable(StringBuilder sb, SqlTable table)
        {
            var tableName = Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture));

            sb
                .Append(1, "public class ")
                .Append(tableName)
                .AppendLine(" : SqlTable")
                .AppendLine(1, "{");

            var pkColumns = table.Columns
                .Where(column => column.Table.Properties.OfType<PrimaryKey>().Any(x => x.SqlColumns.Any(y => y.SqlColumn == column)))
                .ToList();

            foreach (var column in pkColumns)
            {
                var columnAnnotation = OnColumnAnnotation?.Invoke(column);
                var columnComment = OnColumnComment?.Invoke(column);

                var columnCreation = Writer.GetColumnCreation(column, Helper, columnAnnotation, columnComment);
                sb.AppendLine(columnCreation);
            }

            var regularColumns = table.Columns
                .Where(x => !pkColumns.Contains(x))
                .ToList();

            foreach (var column in regularColumns)
            {
                var columnAnnotation = OnColumnAnnotation?.Invoke(column);
                var columnComment = OnColumnComment?.Invoke(column);

                var columnCreation = Writer.GetColumnCreation(column, Helper, columnAnnotation, columnComment);
                sb.AppendLine(columnCreation);
            }

            GenerateTableProperties(sb, table);
            sb.AppendLine(1, "}");
        }

        protected void GenerateTableInDbClass(StringBuilder sb, SqlTable table)
        {
            // TODO move to table class declaration
            var pks = table.Properties.OfType<PrimaryKey>().ToList();
            if (pks.Count == 0)
            {
                sb.AppendLine(2, "// no primary key");
            }

            var tableComment = OnTableComment?.Invoke(table);
            if (!string.IsNullOrEmpty(tableComment))
            {
                sb.Append(2, "// ").AppendLine(tableComment);
            }

            var tableName = Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture));

            sb
                .Append(2, "public ")
                .Append(tableName)
                .Append(" ")
                .Append(tableName)
                .Append(" { get; } = new ")
                .Append(tableName)
                .AppendLine("();");

            GenerateTableProperties(sb, table);
        }

        protected override void GenerateIndex(StringBuilder sb, Index index)
        {
            //public Index _i { get; } = Generic1Columns.AddIndex(true, nameof(Name));
            sb
                .Append(2, "public Index ")
                .Append(index.Name)
                .Append(" { get; } = Generic1Columns.AddIndex(")
                .Append(index.Unique ? "true" : "")
                .Append(index.SqlColumns.Select(c => "nameof(" + c.SqlColumn.Name + ")"))
                .AppendLine(");");
        }

        protected override void GenerateUniqueConstraint(StringBuilder sb, UniqueConstraint uniqueConstraint)
        {
            //public UniqueConstraint _uc1 { get; } = Generic1Columns.AddUniqueConstraint(nameof(Id));
            sb
                .Append(2, "public UniqueConstraint ")
                .Append(uniqueConstraint.Name)
                .Append(" { get; } = Generic1Columns.AddUniqueConstraint(")
                .Append(uniqueConstraint.SqlColumns.Select(c => "nameof(" + c.SqlColumn.Name + ")"))
                .AppendLine(");");
        }

        protected override void WriteSingleFileFooter(StringBuilder sb)
        {
            sb.AppendLine("}");
        }

        protected override void WriteSingleFileHeader(StringBuilder sb, List<SqlTable> tables, bool partialClass = false)
        {
            WriteSingleFileHeaderCommon(sb, partialClass);

            sb.AppendLine("");

            foreach (var table in tables)
                GenerateTableInDbClass(sb, table);

            sb.AppendLine(1, "}");
        }
    }
}