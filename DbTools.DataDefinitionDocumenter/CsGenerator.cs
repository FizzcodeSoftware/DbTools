﻿namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    public class CsGenerator
    {
        private readonly string _namespace;
        private readonly string _databaseName;
        private readonly ITableCustomizer _tableCustomizer;

        private readonly string _fileName;

        public CsGenerator(string databaseName, string @namespace, ITableCustomizer tableCustomizer = null, string fileName = null)
        {
            _databaseName = databaseName;
            _namespace = @namespace;
            _tableCustomizer = tableCustomizer ?? new EmptyTableCustomizer();
            _fileName = fileName;
        }

        private readonly List<KeyValuePair<string, SqlTable>> _sqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();
        private readonly List<KeyValuePair<string, SqlTable>> _skippedSqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();

        public void Generate(DatabaseDefinition databaseDefinition)
        {
            GenerateMainFile();

            foreach (var table in databaseDefinition.GetTables())
            {
                if (!_tableCustomizer.ShouldSkip(table.Name))
                    _sqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(_tableCustomizer.Category(table.Name), table));
                else
                    _skippedSqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(_tableCustomizer.Category(table.Name), table));
            }

            foreach (var tableKvp in _sqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.Name))
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                GenerateTable(category, table);
            }

            /*var fileName = _fileName ?? (_databaseName?.Length == 0 ? "Database.xlsx" : _databaseName + ".xlsx");

            var path = ConfigurationManager.AppSettings["WorkingDirectory"];

            File.WriteAllBytes(path + fileName, content)*/;
        }

        public void GenerateMainFile()
        {
            var sb = new StringBuilder();
            sb.Append("namespace ")
                .AppendLine(_namespace)
                .AppendLine("{")
                .AppendLine("\tusing FizzCode.DbTools.DataDefinition;")
                .AppendLine("");

            sb.AppendLine($"\tpublic partial class {_databaseName} : DatabaseDeclaration");
            sb.AppendLine("\t{");

            sb.AppendLine("\t}");
            sb.AppendLine("}");

            var path = ConfigurationManager.AppSettings["WorkingDirectory"]
                + _databaseName + "/" + _databaseName + ".cs";

            var fileInfo = new FileInfo(path);
            fileInfo.Directory.Create();
            File.WriteAllText(path, sb.ToString());
        }

        protected void GenerateTable(string category, SqlTable table)
        {
            var sb = new StringBuilder();
            sb.Append("namespace ")
                .AppendLine(_namespace)
                .AppendLine("{")
                .AppendLine("\tusing FizzCode.DbTools.DataDefinition;")
                .AppendLine("");

            sb.AppendLine($"\tpublic partial class {_databaseName}")
                .AppendLine("\t{");


            sb.AppendLine($"\t\tpublic static LazySqlTable {table.Name}  = new LazySqlTable(() =>")
                .AppendLine("\t\t{")
                .AppendLine("\t\t\tvar table = new SqlTableDeclaration();");

            var pks = table.Properties.OfType<PrimaryKey>().ToList();

            foreach (var column in table.Columns.Values)
            {
                // TODO Type as ISqlTypeMapper
                
                var descriptionProperty = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
                var description = "";
                if (descriptionProperty != null)
                    description = descriptionProperty.Description;

                var isPk = pks.Any(pk => pk.SqlColumns.Any(cao => cao.SqlColumn == column));
                
                sb.AppendLine(ColumnCreationHelper.GetColumnCreation(column));

                // DocumenterWriter.Write(table.Name, category, table.Name, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);

                /*if (isPk)
                    DocumenterWriter.Write(table.Name, true);
                else
                    DocumenterWriter.Write(table.Name, "");

                var identity = column.Value.Properties.OfType<Identity>().FirstOrDefault();

                if(identity != null)
                    DocumenterWriter.Write(table.Name, $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    DocumenterWriter.Write(table.Name, "");

                var defaultValue = column.Value.Properties.OfType<DefaultValue>().FirstOrDefault();

                if (defaultValue != null)
                    DocumenterWriter.Write(table.Name, defaultValue);
                else
                    DocumenterWriter.Write(table.Name, "");

                DocumenterWriter.WriteLine(table.Name, description.Trim());*/

            }

            /*DocumenterWriter.WriteLine(table.Name);
            DocumenterWriter.WriteLine(table.Name, "Foreign keys");

            foreach (var fk in table.Properties.OfType<ForeignKey>())
            {
                DocumenterWriter.Write(table.Name, fk.Name);
                foreach (var fkColumn in fk.ForeignKeyColumns)
                    DocumenterWriter.Write(table.Name, fkColumn.ForeignKeyColumn.Name);

                DocumenterWriter.Write(table.Name, "");
                DocumenterWriter.Write(table.Name, fk.PrimaryKey.SqlTable.Name);
                DocumenterWriter.Write(table.Name, "");

                foreach (var fkColumn in fk.ForeignKeyColumns)
                    DocumenterWriter.Write(table.Name, fkColumn.PrimaryKeyColumn.Name);
            }

            DocumenterWriter.WriteLine(table.Name);
            DocumenterWriter.WriteLine(table.Name, "Indexes");

            foreach (var index in table.Properties.OfType<Index>())
            {
                DocumenterWriter.Write(table.Name, index.Name);
                foreach (var indexColumn in index.SqlColumns)
                {
                    DocumenterWriter.Write(table.Name, indexColumn.SqlColumn.Name);
                    DocumenterWriter.Write(table.Name, indexColumn);
                }

                DocumenterWriter.Write(table.Name, "");
                DocumenterWriter.Write(table.Name, "Includes:");
                foreach (var includeColumn in index.Includes)
                    DocumenterWriter.Write(table.Name, includeColumn.Name);
            }*/

            sb.AppendLine("\t\t\treturn table;");
            sb.AppendLine("\t\t});");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            // TODO handle illegal chars
            var categoryInPath = category;
            /*if (categoryInPath == "?")
                categoryInPath = "QuestionMark";*/

            if (string.IsNullOrEmpty(categoryInPath))
                categoryInPath = "_no_category_";

            categoryInPath = categoryInPath.Replace('?', '？');

            var path = ConfigurationManager.AppSettings["WorkingDirectory"]
                + _databaseName + "/" + categoryInPath + "/" + table.Name + ".cs";

            var fileInfo = new FileInfo(path);
            fileInfo.Directory.Create();
            File.WriteAllText(path, sb.ToString());
        }
    }
}
